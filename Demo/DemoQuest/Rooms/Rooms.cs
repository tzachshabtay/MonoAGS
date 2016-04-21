using System;
using System.Linq;
using AGS.API;

namespace DemoGame
{
	public static class Rooms
	{
		public static IRoom EmptyStreet { get; set; }
		public static IRoom BrokenCurbStreet { get; set; }
		public static IRoom TrashcanStreet { get; set; }
		public static IRoom DarsStreet { get; set; }

		public static void Init(IGame game)
		{
			game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSaveGameLoaded(game.State));
		}

		public static IRoom Find(IGameState state, IRoom oldRoom)
		{
			return state.Rooms.First(r => r.ID == oldRoom.ID);
		}

		private static void onSaveGameLoaded(IGameState state)
		{
			EmptyStreet = Find(state, EmptyStreet);
			BrokenCurbStreet = Find(state, BrokenCurbStreet);
			TrashcanStreet = Find(state, TrashcanStreet);
			DarsStreet = Find(state, DarsStreet);
		}


	}
}

