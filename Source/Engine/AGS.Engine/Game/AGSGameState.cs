using System;
using System.Linq;
using AGS.API;
using Autofac;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSGameState : IGameState
	{
		private Lazy<ICutscene> _cutscene;
        private IAGSRoomTransitions _roomTransitions;
        private Task[] _emptyTaskArray = new Task[] { };

        public AGSGameState (ICustomProperties globalVariables, IAGSRoomTransitions roomTransitions, 
                             Resolver resolver, IFocusedUI focusedUi, IViewport viewport)
		{
			Speed = 100;
			Rooms = new AGSBindingList<IRoom>(10);
			UI = new AGSConcurrentHashSet<IObject> ();
            SecondaryViewports = new AGSBindingList<IViewport>(5);
            viewport.RoomProvider = this;
            viewport.Camera.Target = () => Player;
            Viewport = viewport;
			GlobalVariables = globalVariables;
            FocusedUI = focusedUi;
			_cutscene = new Lazy<ICutscene> (() => resolver.Container.Resolve<ICutscene>());
			_roomTransitions = roomTransitions;
		}

		#region IGameState implementation

        public ICharacter Player { get; set; }

        public IRoom Room { get; private set; }

		public IAGSBindingList<IRoom> Rooms { get; private set; }

        public IViewport Viewport { get; }

        public IAGSBindingList<IViewport> SecondaryViewports { get; }

		public IConcurrentHashSet<IObject> UI { get; private set; }

        public IFocusedUI FocusedUI { get; }

		public ICustomProperties GlobalVariables { get; }

        public ICutscene Cutscene => _cutscene.Value;

        public IRoomTransitions RoomTransitions => _roomTransitions;

        public bool Paused { get; set; }

		public int Speed { get; set; }

        #endregion

        public async Task ChangeRoomAsync(IRoom newRoom, Action afterTransitionFadeOut = null)
        {
            if (Room != null)
            {
                if (_roomTransitions.State != RoomTransitionState.NotInTransition) //Room is already changing, need to wait for previous transition to complete before starting a new transition!
                {
                    while (_roomTransitions.State != RoomTransitionState.NotInTransition)
                    {
                        Task.WaitAll(_emptyTaskArray, 10); //Busy waiting, agghh!
                    }
                    if (Room == newRoom) return; //somebody already changed to this room, no need to change again.
                }
                Room.Events.OnBeforeFadeOut.Invoke();
                _roomTransitions.State = RoomTransitionState.BeforeLeavingRoom;
                if (_roomTransitions.Transition != null)
                    await _roomTransitions.OnStateChanged.WaitUntilAsync(canContinueRoomTransition);
            }
            afterTransitionFadeOut?.Invoke();
            Room = newRoom;
        }

		public void CopyFrom(IGameState state)
		{
			Rooms = state.Rooms;
			Player = state.Player;
			UI = state.UI;
			GlobalVariables.CopyFrom(state.GlobalVariables);
			Cutscene.CopyFrom(state.Cutscene);
		}

		public void Clean()
		{
			foreach (var room in Rooms)
			{
				room.Dispose();
			}
			foreach (var ui in UI)
			{
				ui.Dispose();
			}
		}

		public TEntity Find<TEntity>(string id) where TEntity : class, IEntity
		{
			//Naive implementation, if this becomes a bottleneck, we'll need to maintain a dictionary of all objects
			if (typeof(TEntity) == typeof(IObject) || typeof(TEntity) == typeof(ICharacter))
			{
				return findInRooms<TEntity>(id) ?? findUi<TEntity>(id);
			}
			else
			{
				return findUi<TEntity>(id) ?? findInRooms<TEntity>(id);
			}
		}

		private TEntity findUi<TEntity>(string id) where TEntity : class, IEntity
		{
			return (UI.FirstOrDefault(o => o.ID == id)) as TEntity;
		}

		private TEntity findInRooms<TEntity>(string id) where TEntity : class, IEntity
		{
			return (Rooms.SelectMany(r => r.Objects).FirstOrDefault(o => o.ID == id)) as TEntity;
		}

        private bool canContinueRoomTransition()
        {
            return _roomTransitions.State == RoomTransitionState.PreparingTransition ||
                   _roomTransitions.State == RoomTransitionState.NotInTransition;
        }
    }
}

