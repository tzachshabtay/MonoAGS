using System;
using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Collections.Generic;

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
				AGSFontLoader fontLoader = new AGSFontLoader(new ResourceLoader());
				AGSGameSettings.DefaultSpeechFont = new Font(fontLoader.LoadFontFamily("../../Assets/Fonts/pf_ronda_seven.ttf"), 14f);
				AGSGameSettings.DefaultTextFont = new Font(fontLoader.LoadFontFamily("../../Assets/Fonts/Pixel_Berry_08_84_Ltd.Edition.TTF"), 14f);

				loadRooms(game);
				loadCharacters(game);
				loadUi(game);

				DefaultInteractions defaults = new DefaultInteractions(game.State.Player, game.Events);
				defaults.Load();
			});

			game.Start("Demo Game", 320, 200);
		}

		private static void loadUi(IGame game)
		{
			InventoryItems items = new InventoryItems ();
			items.Load(game.Factory);

			MouseCursors cursors = new MouseCursors();
			cursors.Load(game);

			InventoryPanel inventory = new InventoryPanel (cursors.Scheme);
			inventory.Load(game);

			OptionsPanel options = new OptionsPanel (cursors.Scheme);
			options.Load(game);

			TopBar topBar = new TopBar(cursors.Scheme, inventory, options);
			topBar.Load(game.Factory);

			ILabel label = game.Factory.GetLabel("", 80, 25, 160, 10, new AGSTextConfig(brush: Brushes.WhiteSmoke,
				alignment: ContentAlignment.MiddleCenter, outlineBrush: Brushes.DarkSlateBlue, outlineWidth: 2f,
				autoFit: AutoFit.LabelShouldFitText, paddingBottom: 5f));
			label.Tint = Color.FromArgb(120, Color.IndianRed);
			label.Anchor = new AGSPoint(0.5f, 0f);
			VerbOnHotspotLabel hotspotLabel = new VerbOnHotspotLabel(() => cursors.Scheme.CurrentMode, game, label);
			hotspotLabel.Start();

			addDebugLabels(game);
		}

		private static void loadCharacters(IGame game)
		{
			Cris cris = new Cris ();
			ICharacter character = cris.Load(game.Factory);

			game.State.Player.Character = character;
			character.ChangeRoom(Rooms.EmptyStreet, 50, 30);

			Beman beman = new Beman ();
			character = beman.Load(game.Factory);
			character.ChangeRoom(Rooms.BrokenCurbStreet, 100, 110);
		}

		private static void loadRooms(IGame game)
		{
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
			game.State.Rooms.Add(Rooms.TrashcanStreet);
		}

		[Conditional("DEBUG")]
		private static void addDebugLabels(IGame game)
		{
			ILabel fpsLabel = game.Factory.GetLabel("", 30, 25, 320, 25, config: new AGSTextConfig(alignment: ContentAlignment.TopLeft,
				autoFit: AutoFit.LabelShouldFitText));
			fpsLabel.Anchor = new AGSPoint (1f, 0f);
			fpsLabel.ScaleBy(0.7f, 0.7f);
			FPSCounter fps = new FPSCounter(game.Events, fpsLabel);
			fps.Start();

			ILabel label = game.Factory.GetLabel("", 30, 25, 320, 5, config: new AGSTextConfig(alignment: ContentAlignment.TopRight,
				autoFit: AutoFit.LabelShouldFitText));
			label.Anchor = new AGSPoint (1f, 0f);
			label.ScaleBy(0.7f, 0.7f);
			MousePositionLabel mouseLabel = new MousePositionLabel(game.Input, label);
			mouseLabel.Start();
		}
	}
}
