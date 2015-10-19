using System;
using AGS.API;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameState : IGameState
	{
		private Lazy<ICutscene> _cutscene;

		public AGSGameState (IPlayer player, ICustomProperties globalVariables, Resolver resolver)
		{
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

		#endregion
	}
}

