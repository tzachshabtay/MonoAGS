using System;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;

namespace DemoGame
{
	public static class Rooms
	{
		public static IRoom SplashScreen { get; set; }
		public static Task<IRoom> EmptyStreet { get; set; }
		public static Task<IRoom> BrokenCurbStreet { get; set; }
		public static Task<IRoom> TrashcanStreet { get; set; }
		public static Task<IRoom> DarsStreet { get; set; }

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
			EmptyStreet = Task.FromResult(Find(state, EmptyStreet.Result));
			BrokenCurbStreet = Task.FromResult(Find(state, BrokenCurbStreet.Result));
			TrashcanStreet = Task.FromResult(Find(state, TrashcanStreet.Result));
			DarsStreet = Task.FromResult(Find(state, DarsStreet.Result));
		}


	}
}

