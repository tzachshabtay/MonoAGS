using System;
using Engine;
using System.Threading.Tasks;
using API;

namespace DemoGame
{
	class MainClass
	{
		[STAThread]
		public static void Main()
		{
			IGame game = AGSGame.CreateEmpty();

			game.Events.OnLoad.Subscribe((sender, e) =>
			{
				EmptyStreet emptyStreet = new EmptyStreet (game.State.Player, new AGSViewport(), game.Events);
				Rooms.EmptyStreet = emptyStreet.Load(game.Factory.Graphics);

				BrokenCurbStreet brokenCurbStreet = new BrokenCurbStreet(game.State.Player, new AGSViewport(), game.Events);
				Rooms.BrokenCurbStreet = brokenCurbStreet.Load(game.Factory.Graphics);

				game.State.Rooms.Add(Rooms.EmptyStreet);
				game.State.Rooms.Add(Rooms.BrokenCurbStreet);

				Cris cris = new Cris ();
				ICharacter character = cris.Load(game.Factory.Graphics);

				game.State.Player.Character = character;
				character.ChangeRoom(Rooms.EmptyStreet, 50, 30);

				TwoButtonsInputScheme inputScheme = new TwoButtonsInputScheme(game.State, game.Input, new GLText(""));
				inputScheme.Start();
			});

			game.Start("Demo Game", 320, 200);
		}
	}
}
