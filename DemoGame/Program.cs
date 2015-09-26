using System;
using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;
using System.Drawing;
using System.Diagnostics;

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
				DefaultInteractions defaults = new DefaultInteractions(game.State.Player, game.Events);
				defaults.Load();

				MouseCursors cursors = new MouseCursors();
				cursors.Load(game);

				EmptyStreet emptyStreet = new EmptyStreet (game.State.Player);
				Rooms.EmptyStreet = emptyStreet.Load(game.Factory);

				BrokenCurbStreet brokenCurbStreet = new BrokenCurbStreet(game.State.Player);
				Rooms.BrokenCurbStreet = brokenCurbStreet.Load(game.Factory);

				TrashcanStreet trashcanStreet = new TrashcanStreet(game.State.Player);
				Rooms.TrashcanStreet = trashcanStreet.Load(game.Factory);

				DarsStreet darsStreet = new DarsStreet(game.State.Player);
				Rooms.DarsStreet = darsStreet.Load(game.Factory);

				game.State.Rooms.Add(Rooms.EmptyStreet);
				game.State.Rooms.Add(Rooms.BrokenCurbStreet);
				game.State.Rooms.Add(Rooms.DarsStreet);

				Cris cris = new Cris ();
				ICharacter character = cris.Load(game.Factory);

				game.State.Player.Character = character;
				character.ChangeRoom(Rooms.EmptyStreet, 50, 30);

				ILabel label = game.Factory.GetLabel("", 80, 25, 160, 10, new AGSTextConfig(brush: Brushes.WhiteSmoke,
					alignment: ContentAlignment.MiddleCenter, outlineBrush: Brushes.DarkSlateBlue, outlineWidth: 2f,
					autoFit: AutoFit.LabelShouldFitText, paddingBottom: 5f));
				label.Tint = Color.FromArgb(120, Color.IndianRed);
				label.Anchor = new AGSPoint(0.5f, 0f);
				HotspotLabel hotspotLabel = new HotspotLabel(game, label);
				hotspotLabel.Start();

				addDebugLabels(game);
			});

			game.Start("Demo Game", 320, 200);
		}

		[Conditional("DEBUG")]
		private static void addDebugLabels(IGame game)
		{
			ILabel fpsLabel = game.Factory.GetLabel("", 30, 25, 0, 25);
			fpsLabel.ScaleBy(0.7f, 0.7f);
			FPSCounter fps = new FPSCounter(game.Events, fpsLabel);
			fps.Start();

			ILabel label = game.Factory.GetLabel("", 30, 25, 0, 5);
			label.ScaleBy(0.7f, 0.7f);
			MousePositionLabel mouseLabel = new MousePositionLabel(game.Input, label);
			mouseLabel.Start();
		}
	}
}
