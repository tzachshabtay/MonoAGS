using System;
using System.Linq;
using AGS.API;
using Autofac;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSGameState : IAGSGameState
	{
		private Lazy<ICutscene> _cutscene;
        private readonly ViewportCollection _viewports;

        public AGSGameState (ICustomProperties globalVariables, IRoomTransitions roomTransitions,
                             Resolver resolver, IFocusedUI focusedUi, IViewport viewport, IEvent<RoomTransitionEventArgs> onRoomChangeRequired)
		{
			Speed = 100;
			Rooms = new AGSBindingList<IRoom>(10);
			UI = new AGSConcurrentHashSet<IObject> ();
            SecondaryViewports = new AGSBindingList<IViewport>(5);
            RoomTransitions = roomTransitions;
            viewport.RoomProvider = this;
            viewport.Camera.Target = () => Player;
            Viewport = viewport;
			GlobalVariables = globalVariables;
            FocusedUI = focusedUi;
			_cutscene = new Lazy<ICutscene> (() => resolver.Container.Resolve<ICutscene>());
            OnRoomChangeRequired = onRoomChangeRequired;
            _viewports = new ViewportCollection(this);
		}

		#region IGameState implementation

        public IEvent<RoomTransitionEventArgs> OnRoomChangeRequired { get; private set; }

        public ICharacter Player { get; set; }

        public IRoom Room { get; private set; }

		public IAGSBindingList<IRoom> Rooms { get; private set; }

        public IViewport Viewport { get; }

        public IAGSBindingList<IViewport> SecondaryViewports { get; }

        public List<IViewport> GetSortedViewports() => _viewports.SortedViewports;

		public IConcurrentHashSet<IObject> UI { get; private set; }

        public IFocusedUI FocusedUI { get; }

		public ICustomProperties GlobalVariables { get; }

        public ICutscene Cutscene => _cutscene.Value;

        public IRoomTransitions RoomTransitions { get; }

        public bool Paused { get; set; }

        public bool DuringRoomTransition { get; private set; }

		public int Speed { get; set; }

        #endregion

        public async Task ChangeRoomAsync(IRoom newRoom, Action afterTransitionFadeOut = null)
        {
            DuringRoomTransition = true;
            await OnRoomChangeRequired.InvokeAsync(new RoomTransitionEventArgs(Room, newRoom, () =>
            {
                afterTransitionFadeOut?.Invoke();
                Room = newRoom;
            }));
            DuringRoomTransition = false;
            await newRoom?.Events.OnAfterFadeIn.InvokeAsync();
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
    }
}