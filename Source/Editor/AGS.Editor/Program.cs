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
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new FileSystemResourcePack(AGSGame.Device.FileSystem), 0));
                game.Factory.Resources.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(AGSGame.Device.Assemblies.EntryAssembly), 1));
                game.Factory.Fonts.InstallFonts("../../Assets/Fonts/Font Awesome 5 Free-Solid-900.otf");
                FontIcons.Init(game.Factory.Fonts);

                AGSEditor editor = resolver.Container.Resolve<AGSEditor>();
                editor.Editor = game;

                AGSGameSettings.CurrentSkin = null;

                WelcomeScreen screen = new WelcomeScreen(editor);
                screen.Load();
                screen.Show();

                var room = game.Factory.Room.GetRoom("MainEditorRoom");
                await game.State.ChangeRoomAsync(room);
            });

            game.Start();
        }
    }
}