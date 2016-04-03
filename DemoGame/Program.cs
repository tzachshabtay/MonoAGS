using System;
using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

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

				DefaultInteractions defaults = new DefaultInteractions(game, game.Events);
				defaults.Load();
			});

			game.Start(new AGSGameSettings("Demo Game", new Size(320, 200), 
				windowSize: new Size(640, 480), windowState: WindowState.Normal));
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
			topBar.Load(game);

			ILabel label = game.Factory.UI.GetLabel("Hotspot Label", "", 80, 25, 160, 10, new AGSTextConfig(brush: Brushes.WhiteSmoke,
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
			ICharacter character = cris.Load(game);

			game.State.Player.Character = character;
			character.ChangeRoom(Rooms.EmptyStreet, 50, 30);

			Beman beman = new Beman ();
			character = beman.Load(game);
			character.ChangeRoom(Rooms.BrokenCurbStreet, 100, 110);

			Characters.Init(game);
		}

		private static void loadRooms(IGame game)
		{
			EmptyStreet emptyStreet = new EmptyStreet (game.State.Player);
			Rooms.EmptyStreet = emptyStreet.Load(game);

			BrokenCurbStreet brokenCurbStreet = new BrokenCurbStreet();
			Rooms.BrokenCurbStreet = brokenCurbStreet.Load(game);

			TrashcanStreet trashcanStreet = new TrashcanStreet();
			Rooms.TrashcanStreet = trashcanStreet.Load(game);

			DarsStreet darsStreet = new DarsStreet();
			Rooms.DarsStreet = darsStreet.Load(game);

			game.State.Rooms.Add(Rooms.EmptyStreet);
			game.State.Rooms.Add(Rooms.BrokenCurbStreet);
			game.State.Rooms.Add(Rooms.DarsStreet);
			game.State.Rooms.Add(Rooms.TrashcanStreet);

			Rooms.Init(game);
		}

		[Conditional("DEBUG")]
		private static void addDebugLabels(IGame game)
		{
			ILabel fpsLabel = game.Factory.UI.GetLabel("FPS Label", "", 30, 25, 320, 25, config: new AGSTextConfig(alignment: ContentAlignment.TopLeft,
				autoFit: AutoFit.LabelShouldFitText));
			fpsLabel.Anchor = new AGSPoint (1f, 0f);
			fpsLabel.ScaleBy(0.7f, 0.7f);
			FPSCounter fps = new FPSCounter(game, fpsLabel);
			fps.Start();

			ILabel label = game.Factory.UI.GetLabel("Mouse Position Label", "", 30, 25, 320, 5, config: new AGSTextConfig(alignment: ContentAlignment.TopRight,
				autoFit: AutoFit.LabelShouldFitText));
			label.Anchor = new AGSPoint (1f, 0f);
			label.ScaleBy(0.7f, 0.7f);
			MousePositionLabel mouseLabel = new MousePositionLabel(game, label);
			mouseLabel.Start();
		}
	}
}
