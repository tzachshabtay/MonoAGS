using System;
using AGS.API;

namespace DemoGame
{
	public static class Characters
	{
		public static ICharacter Cris { get; set; }
		public static ICharacter Beman { get; set; }

		public static void Init(IGame game)
		{
			game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSaveGameLoaded(game.State));
		}

		private static void onSaveGameLoaded(IGameState state)
		{
			Cris = state.Find<ICharacter>(Cris.ID);
			Beman = state.Find<ICharacter>(Beman.ID);
		}
	}
}

