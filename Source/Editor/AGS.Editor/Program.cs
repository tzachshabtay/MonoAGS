using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    public static class Program
    {
        public static void Run()
        {
            Resolver resolver = new Resolver(AGSGame.Device, new AGSGameSettings("MonoAGS Editor", new AGS.API.Size(1280, 800),
               windowSize: new AGS.API.Size(1280, 800), windowState: WindowState.Normal, preserveAspectRatio: false));
            IGame game = AGSGame.Create(resolver);

            //Rendering the text at a 4 time higher resolution than the actual game, so it will still look sharp when maximizing the window.
            GLText.TextResolutionFactorX = 4;
            GLText.TextResolutionFactorY = 4;

            game.Events.OnLoad.Subscribe(async () =>
            {
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new FileSystemResourcePack(AGSGame.Device.FileSystem, AGSGame.Device.Assemblies.EntryAssembly), 0));
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(AGSGame.Device.Assemblies.EntryAssembly), 1));
                game.Factory.Fonts.InstallFonts("Fonts/Font Awesome 5 Free-Solid-900.otf", "Fonts/Fira/FiraSans-Regular.ttf");
                FontIcons.Init(game.Factory.Fonts);
                game.Settings.Defaults.TextFont = game.Factory.Fonts.LoadFontFromPath("Fonts/Fira/FiraSans-Regular.ttf", 14f, FontStyle.Regular);
                game.Settings.Defaults.SpeechFont = game.Settings.Defaults.TextFont;

                AGSEditor editor = resolver.Container.Resolve<AGSEditor>();
                editor.Editor = game;

                game.Settings.Defaults.Skin = null;

                WelcomeScreen screen = new WelcomeScreen(editor);
                screen.Load();
                screen.Show();

                var room = game.Factory.Room.GetRoom("MainEditorRoom");
                ReflectionCache.Refresh();
                await game.State.ChangeRoomAsync(room);
            });

            game.Start();
        }
    }
}
