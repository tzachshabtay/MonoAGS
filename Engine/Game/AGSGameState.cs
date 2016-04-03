using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;
using Autofac;
using ProtoBuf;
using System.Collections.Concurrent;

namespace AGS.Engine
{
	public class AGSGameState : IGameState
	{
		private Lazy<ICutscene> _cutscene;

		public AGSGameState (IPlayer player, ICustomProperties globalVariables, Resolver resolver)
		{
			Speed = 100;
			Rooms = new List<IRoom> ();
			UI = new AGSConcurrentHashSet<IObject> ();
			Player = player;
			GlobalVariables = globalVariables;
			_cutscene = new Lazy<ICutscene> (() => resolver.Container.Resolve<ICutscene>());
		}

		#region IGameState implementation

		public IPlayer Player { get; set; }

		public IList<IRoom> Rooms { get; private set; }

		public IConcurrentHashSet<IObject> UI { get; private set; }

		public ICustomProperties GlobalVariables { get; private set; }

		public ICutscene Cutscene { get { return _cutscene.Value; } }

		public bool Paused { get; set; }

		public int Speed { get; set; }

		#endregion

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

