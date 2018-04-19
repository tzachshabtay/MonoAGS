using System;
using System.Diagnostics;
using AGS.API;
using AGS.Engine;
using Autofac;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public static class Program
    {
        public static void Run()
        {
            GameLoader.SetupResolver();

            IGame game = AGSGame.CreateEmpty();

            //Rendering the text at a 4 time higher resolution than the actual game, so it will still look sharp when maximizing the window.
            GLText.TextResolutionFactorX = 4;
            GLText.TextResolutionFactorY = 4;

            game.Events.OnLoad.Subscribe(async () =>
            {
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new FileSystemResourcePack(AGSGame.Device.FileSystem), 0));
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(AGSGame.Device.Assemblies.EntryAssembly), 1));
                game.Factory.Fonts.InstallFonts("../../Assets/Fonts/Font Awesome 5 Free-Solid-900.otf");
                FontIcons.Init(game.Factory.Fonts);

                AGSGameSettings.CurrentSkin = null;

                //addDebugLabels(game);
                WelcomeScreen screen = new WelcomeScreen(game);
                screen.Load();
                screen.Show();

                var room = game.Factory.Room.GetRoom("MainEditorRoom");
                await game.State.ChangeRoomAsync(room);
            });

            game.Start(new AGSGameSettings("MonoAGS Editor", new AGS.API.Size(1280, 800),
               windowSize: new AGS.API.Size(1280, 800), windowState: WindowState.Normal, preserveAspectRatio: false));
        }

        [Conditional("DEBUG")]
        private static void addDebugLabels(IGame game)
        {
            var resolution = new Size(1200, 800);
            ILabel fpsLabel = game.Factory.UI.GetLabel("FPS Label", "", 30, 25, resolution.Width, 2, config: new AGSTextConfig(alignment: Alignment.TopLeft,
                autoFit: AutoFit.LabelShouldFitText));
            fpsLabel.Pivot = new PointF(1f, 0f);
            fpsLabel.RenderLayer = new AGSRenderLayer(-99999, independentResolution: resolution);
            fpsLabel.Enabled = true;
            fpsLabel.MouseEnter.Subscribe(_ => fpsLabel.Tint = Colors.Indigo);
            fpsLabel.MouseLeave.Subscribe(_ => fpsLabel.Tint = Colors.IndianRed.WithAlpha(125));
            fpsLabel.Tint = Colors.IndianRed.WithAlpha(125);
            FPSCounter fps = new FPSCounter(game, fpsLabel);
            fps.Start();

            ILabel label = game.Factory.UI.GetLabel("Mouse Position Label", "", 1, 1, resolution.Width, 32, config: new AGSTextConfig(alignment: Alignment.TopRight,
                autoFit: AutoFit.LabelShouldFitText));
            label.Tint = Colors.SlateBlue.WithAlpha(125);
            label.Pivot = new PointF(1f, 0f);
            label.RenderLayer = fpsLabel.RenderLayer;
            MousePositionLabel mouseLabel = new MousePositionLabel(game, label);
            mouseLabel.Start();

            ILabel debugHotspotLabel = game.Factory.UI.GetLabel("Debug Hotspot Label", "", 1f, 1f, resolution.Width, 62, config: new AGSTextConfig(alignment: Alignment.TopRight,
              autoFit: AutoFit.LabelShouldFitText));
            debugHotspotLabel.Tint = Colors.DarkSeaGreen.WithAlpha(125);
            debugHotspotLabel.Pivot = new PointF(1f, 0f);
            debugHotspotLabel.RenderLayer = fpsLabel.RenderLayer;
            HotspotLabel hotspot = new HotspotLabel(game, debugHotspotLabel) { DebugMode = true };
            hotspot.Start();
        }
    }
}