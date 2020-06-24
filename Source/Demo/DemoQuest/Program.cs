﻿using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;
using System.Diagnostics;
using DemoQuest;

namespace DemoGame
{
    public class DemoStarter : IGameStarter
	{
        public IGameSettings Settings => new AGSGameSettings("Demo Game", new Size(320, 200),
                windowSize: new Size(640, 400), windowState: WindowState.Normal);

        public static string CustomAssemblyName;
        
        public void StartGame(IGame game)
        {
            //Rendering the text at a 4 time higher resolution than the actual game, so it will still look sharp when maximizing the window.
            GLText.TextResolutionFactorX = 4;
            GLText.TextResolutionFactorY = 4;

            game.Events.OnLoad.Subscribe(async () =>
            {
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new FileSystemResourcePack(AGSGame.Device.FileSystem, AGSGame.Device.Assemblies.EntryAssembly), 0));
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(AGSGame.Device.Assemblies.EntryAssembly, CustomAssemblyName), 1));
                game.Factory.Fonts.InstallFonts("Fonts/pf_ronda_seven.ttf", "Fonts/Pixel_Berry_08_84_Ltd.Edition.TTF");
                game.Settings.Defaults.Fonts.Speech = game.Factory.Fonts.LoadFontFromPath("Fonts/pf_ronda_seven.ttf", 14f);
                var font = game.Factory.Fonts.LoadFontFromPath("Fonts/Pixel_Berry_08_84_Ltd.Edition.TTF", 14f);
                game.Settings.Defaults.Fonts.Text = font;
                game.Settings.Defaults.Fonts.Dialogs = game.Factory.Fonts.LoadFont(font.FontFamily, 6f, font.Style);
                game.Settings.Defaults.Skin = null;
                game.State.RoomTransitions.Transition = AGSRoomTransitions.Fade();
                setKeyboardEvents(game);
                Shaders.SetStandardShader();
                game.Audio.AudioRules.Add(new ReduceWhenSpeechAudioRule(game.Audio));

                addDebugLabels(game);
                Debug.WriteLine("Startup: Loading Assets");
                await loadPlayerCharacter(game);
                Debug.WriteLine("Startup: Loaded Player Character");
                await loadSplashScreen(game);
            });
        }

		public static void Run()
		{
            DemoStarter starter = new DemoStarter();
            var game = AGSGame.Create(starter.Settings);

            starter.StartGame(game);
            game.Start();
		}

        private void setKeyboardEvents(IGame game)
        {
            game.State.Cutscene.SkipTrigger = SkipCutsceneTrigger.AnyKey;
            game.Input.KeyDown.Subscribe(args =>
            {
                if (args.Key == Key.Enter && (game.Input.IsKeyDown(Key.AltLeft) || game.Input.IsKeyDown(Key.AltRight)))
                {
                    if (game.Settings.WindowState == WindowState.FullScreen ||
                        game.Settings.WindowState == WindowState.Maximized)
                    {
                        game.Settings.WindowState = WindowState.Normal;
                        game.Settings.WindowBorder = WindowBorder.Resizable;
                    }
                    else
                    {
                        game.Settings.WindowBorder = WindowBorder.Hidden;
                        game.Settings.WindowState = WindowState.Maximized;
                    }
                }
                else if (args.Key == Key.Escape)
                {
                    if (game.State.Cutscene.IsRunning) return;
                    game.Quit();
                }
            });
        }

		private async Task loadUi(IGame game)
		{
			MouseCursors cursors = new MouseCursors();
			await cursors.LoadAsync(game);
            Debug.WriteLine("Startup: Loaded Cursors");

			InventoryPanel inventory = new InventoryPanel (cursors.Scheme);
			await inventory.LoadAsync(game);
            Debug.WriteLine("Startup: Loaded Inventory Panel");

			OptionsPanel options = new OptionsPanel (cursors.Scheme);
			await options.LoadAsync(game);
            Debug.WriteLine("Startup: Loaded Options Panel");

            FeaturesTopWindow features = new FeaturesTopWindow(cursors.Scheme);
            features.Load(game);
            Debug.WriteLine("Startup: Loaded Features Panel");

            TopBar topBar = new TopBar(cursors.Scheme, inventory, options, features);
			await topBar.LoadAsync(game);
            Debug.WriteLine("Startup: Loaded Top Bar");
		}

        private async Task loadPlayerCharacter(IGame game)
        { 
            Cris cris = new Cris();
            ICharacter character = await cris.LoadAsync(game);

            game.State.Player = character;
        }

		private async Task loadCharacters(IGame game)
		{
            InventoryItems items = new InventoryItems();
            await items.LoadAsync(game.Factory);

            Beman beman = new Beman ();
			var character = await beman.LoadAsync(game);
			var room = await Rooms.BrokenCurbStreet;
			await character.ChangeRoomAsync(room, 100, 110);

			Characters.Init (game);
		}

        private async Task loadSplashScreen(IGame game)
        { 
            AGSSplashScreen splashScreen = new AGSSplashScreen();
            Rooms.SplashScreen = splashScreen.Load(game);

            var splashBackground = game.Factory.Object.GetObject("Splash Background");
            splashBackground.Image = await game.Factory.Graphics.LoadImageAsync("Rooms/Splash/bg.png");
            Rooms.SplashScreen.Background = splashBackground;

            game.State.Rooms.Add(Rooms.SplashScreen);
            Rooms.SplashScreen.Events.OnAfterFadeIn.SubscribeToAsync(async () => 
            { 
                await loadRooms(game);
                Debug.WriteLine("Startup: Loaded Rooms");
                Task charactersLoaded = loadCharacters(game);
                await loadUi(game);
                Debug.WriteLine("Startup: Loaded UI");
                DefaultInteractions defaults = new DefaultInteractions(game, game.Events);
                defaults.Load();
                await charactersLoaded;
                Debug.WriteLine("Startup: Loaded Characters");
                await game.State.Player.ChangeRoomAsync(Rooms.EmptyStreet.Result, 50, 30);
            });
            await game.State.ChangeRoomAsync(Rooms.SplashScreen);
            Debug.WriteLine("Startup: Loaded splash screen");
        }

		private async Task loadRooms(IGame game)
		{
            Debug.WriteLine("Startup: Loading Rooms");
			EmptyStreet emptyStreet = new EmptyStreet (game.State.Player);
			Rooms.EmptyStreet = emptyStreet.LoadAsync(game);
            await waitForRoom(game, Rooms.EmptyStreet);
            Debug.WriteLine("Startup: Loaded empty street");

			BrokenCurbStreet brokenCurbStreet = new BrokenCurbStreet();
			Rooms.BrokenCurbStreet = brokenCurbStreet.LoadAsync(game);
            await waitForRoom(game, Rooms.BrokenCurbStreet);
            Debug.WriteLine("Startup: Loaded broken curb street");

			TrashcanStreet trashcanStreet = new TrashcanStreet();
			Rooms.TrashcanStreet = trashcanStreet.LoadAsync(game);
            await waitForRoom(game, Rooms.TrashcanStreet);
            Debug.WriteLine("Startup: Loaded trashcan street");

			DarsStreet darsStreet = new DarsStreet();
			Rooms.DarsStreet = darsStreet.LoadAsync(game);
            await waitForRoom(game, Rooms.DarsStreet);
            Debug.WriteLine("Startup: Loaded Dars street");

			Rooms.Init(game);
            Debug.WriteLine("Startup: Initialized rooms");
		}

        private async Task waitForRoom(IGame game, Task<IRoom> task)
        {
            var room = await task;
            game.State.Rooms.Add(room);
        }

		[Conditional("DEBUG")]
		private void addDebugLabels(IGame game)
		{
            var resolution = new Size(1200, 800);
            ILabel fpsLabel = game.Factory.UI.GetLabel("FPS Label", "", 30, 25, resolution.Width, 2, config: game.Factory.Fonts.GetTextConfig(alignment: Alignment.TopLeft,
				autoFit: AutoFit.LabelShouldFitText));
			fpsLabel.Pivot = (1f, 0f);
            fpsLabel.RenderLayer = new AGSRenderLayer(-99999, independentResolution: resolution);
            fpsLabel.Enabled = true;
            fpsLabel.MouseEnter.Subscribe(_ => fpsLabel.Tint = Colors.Indigo);
            fpsLabel.MouseLeave.Subscribe(_ => fpsLabel.Tint = Colors.IndianRed.WithAlpha(125));
            fpsLabel.Tint = Colors.IndianRed.WithAlpha(125);
			FPSCounter fps = new FPSCounter(game, fpsLabel);
			fps.Start();

            ILabel label = game.Factory.UI.GetLabel("Mouse Position Label", "", 1, 1, resolution.Width, 32, config: game.Factory.Fonts.GetTextConfig(alignment: Alignment.TopRight,
				autoFit: AutoFit.LabelShouldFitText));
            label.Tint = Colors.SlateBlue.WithAlpha(125);
			label.Pivot = (1f, 0f);
            label.RenderLayer = fpsLabel.RenderLayer;
            MousePositionLabel mouseLabel = new MousePositionLabel(game, label);
			mouseLabel.Start();

            ILabel debugHotspotLabel = game.Factory.UI.GetLabel("Debug Hotspot Label", "", 1f, 1f, resolution.Width, 62, config: game.Factory.Fonts.GetTextConfig(alignment: Alignment.TopRight,
              autoFit: AutoFit.LabelShouldFitText));
            debugHotspotLabel.Tint = Colors.DarkSeaGreen.WithAlpha(125);
            debugHotspotLabel.Pivot = (1f, 0f);
            debugHotspotLabel.RenderLayer = fpsLabel.RenderLayer;
            HotspotLabel hotspot = new HotspotLabel(game, debugHotspotLabel) { DebugMode = true };
            hotspot.Start();
		}
    }
}
