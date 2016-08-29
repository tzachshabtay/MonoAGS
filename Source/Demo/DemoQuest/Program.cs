using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;
using System.Diagnostics;

namespace DemoGame
{
	public class DemoStarter
	{
		public static void Run()
		{
			IGame game = AGSGame.CreateEmpty();            

			game.Events.OnLoad.Subscribe((sender, e) =>
			{
				Hooks.FontLoader.InstallFonts("../../Assets/Fonts/pf_ronda_seven.ttf", "../../Assets/Fonts/Pixel_Berry_08_84_Ltd.Edition.TTF");
				AGSGameSettings.DefaultSpeechFont = Hooks.FontLoader.LoadFontFromPath("../../Assets/Fonts/pf_ronda_seven.ttf", 14f, FontStyle.Regular);
				AGSGameSettings.DefaultTextFont = Hooks.FontLoader.LoadFontFromPath("../../Assets/Fonts/Pixel_Berry_08_84_Ltd.Edition.TTF", 14f, FontStyle.Regular);
                AGSGameSettings.CurrentSkin = null;
				game.State.RoomTransitions.Transition = AGSRoomTransitions.Fade();

				addDebugLabels (game);

				Task roomsLoaded = loadRooms(game);
				Task charactersLoaded = loadCharacters(game);

				var topPanelTask = loadUi(game);

				Task.WhenAll(roomsLoaded, charactersLoaded).ContinueWith(_ => 
				{
					DefaultInteractions defaults = new DefaultInteractions (game, game.Events);
					defaults.Load ();

					game.State.Player.Character.ChangeRoom (Rooms.EmptyStreet.Result, 50, 30);
					topPanelTask.ContinueWith(topPanel => topPanel.Result.Visible = true);
				});
			});

			game.Start(new AGSGameSettings("Demo Game", new AGS.API.Size(320, 200), 
				windowSize: new AGS.API.Size(640, 480), windowState: WindowState.Normal));
		}

		private static async Task<IPanel> loadUi(IGame game)
		{
			MouseCursors cursors = new MouseCursors();
			await cursors.LoadAsync(game);

			InventoryPanel inventory = new InventoryPanel (cursors.Scheme);
			await inventory.LoadAsync(game);

			OptionsPanel options = new OptionsPanel (cursors.Scheme);
			await options.LoadAsync(game);

			TopBar topBar = new TopBar(cursors.Scheme, inventory, options);
			var topPanel = await topBar.LoadAsync(game);

			ILabel label = game.Factory.UI.GetLabel("Hotspot Label", "", 80, 25, 160, 10, new AGSTextConfig(brush: Hooks.BrushLoader.LoadSolidBrush(Colors.WhiteSmoke),
				alignment: Alignment.MiddleCenter, outlineBrush: Hooks.BrushLoader.LoadSolidBrush(Colors.DarkSlateBlue), outlineWidth: 2f,
				autoFit: AutoFit.LabelShouldFitText, paddingBottom: 5f));
			AGS.API.Color red = Colors.IndianRed;
			label.Tint = AGS.API.Color.FromArgb(120, red.R, red.G, red.B);
			label.Anchor = new AGS.API.PointF(0.5f, 0f);
			VerbOnHotspotLabel hotspotLabel = new VerbOnHotspotLabel(() => cursors.Scheme.CurrentMode, game, label);
			hotspotLabel.Start();

			return topPanel;
		}

		private static async Task loadCharacters(IGame game)
		{
			Cris cris = new Cris ();
			ICharacter character = await cris.LoadAsync(game);

			game.State.Player.Character = character;
			KeyboardMovement movement = new KeyboardMovement (character, game.Input, KeyboardMovementMode.Pressing);
			movement.AddArrows();
			movement.AddWASD();
			character.ChangeRoom (Rooms.SplashScreen);

            InventoryItems items = new InventoryItems();
            await items.LoadAsync(game.Factory);

            Beman beman = new Beman ();
			character = await beman.LoadAsync(game);
			var room = await Rooms.BrokenCurbStreet;
			character.ChangeRoom(room, 100, 110);

			Characters.Init (game);
		}

		private static Task loadRooms(IGame game)
		{
			AGSSplashScreen splashScreen = new AGSSplashScreen ();
			Rooms.SplashScreen = splashScreen.Load (game);
			game.State.Rooms.Add (Rooms.SplashScreen);

			EmptyStreet emptyStreet = new EmptyStreet (game.State.Player);
			Rooms.EmptyStreet = emptyStreet.LoadAsync(game);
			addRoomWhenLoaded(game, Rooms.EmptyStreet);

			BrokenCurbStreet brokenCurbStreet = new BrokenCurbStreet();
			Rooms.BrokenCurbStreet = brokenCurbStreet.LoadAsync(game);
			addRoomWhenLoaded(game, Rooms.BrokenCurbStreet);

			TrashcanStreet trashcanStreet = new TrashcanStreet();
			Rooms.TrashcanStreet = trashcanStreet.LoadAsync(game);
			addRoomWhenLoaded (game, Rooms.TrashcanStreet);

			DarsStreet darsStreet = new DarsStreet();
			Rooms.DarsStreet = darsStreet.LoadAsync(game);
			addRoomWhenLoaded(game, Rooms.DarsStreet);

			Rooms.Init(game);

			return Rooms.DarsStreet;
		}

		private static void addRoomWhenLoaded (IGame game, Task<IRoom> task)
		{
			task.ContinueWith(room => game.State.Rooms.Add (room.Result));
		}

		[Conditional("DEBUG")]
		private static void addDebugLabels(IGame game)
		{
			ILabel fpsLabel = game.Factory.UI.GetLabel("FPS Label", "", 30, 25, 320, 25, config: new AGSTextConfig(alignment: Alignment.TopLeft,
				autoFit: AutoFit.LabelShouldFitText));
			fpsLabel.Anchor = new AGS.API.PointF (1f, 0f);
			fpsLabel.ScaleBy(0.7f, 0.7f);
            fpsLabel.RenderLayer = new AGSRenderLayer(-99999);
			FPSCounter fps = new FPSCounter(game, fpsLabel);
			fps.Start();

			ILabel label = game.Factory.UI.GetLabel("Mouse Position Label", "", 30, 25, 320, 5, config: new AGSTextConfig(alignment: Alignment.TopRight,
				autoFit: AutoFit.LabelShouldFitText));
			label.Anchor = new AGS.API.PointF (1f, 0f);
			label.ScaleBy(0.7f, 0.7f);
            label.RenderLayer = new AGSRenderLayer(-99999);
            MousePositionLabel mouseLabel = new MousePositionLabel(game, label);
			mouseLabel.Start();
		}
	}
}
