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
			//var engine = new MyClass ();
			//engine.Run ();

			IGame game = AGSGame.CreateEmpty();

			game.OnLoad.Subscribe((sender, e) =>
			{
				EmptyStreet emptyStreet = new EmptyStreet (game.State.Player, new AGSViewport());

				IRoom room = emptyStreet.Load(game.Factory.Graphics);

				game.State.Rooms.Add(room);

				Cris cris = new Cris ();
				ICharacter character = cris.Load(game.Factory.Graphics);

				game.State.Player.Character = character;
				character.Room = room;
				character.PlaceOnWalkableArea();
				room.Objects.Add(character);

				TwoButtonsInputScheme inputScheme = new TwoButtonsInputScheme(game.State, game.Input, new GLText(""));
				inputScheme.Start();
			});

			game.Start("Demo Game", 320, 200);
		}
	}
}
