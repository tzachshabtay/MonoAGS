using System;
using AGS.API;
using AGS.Engine;
using AGS.Engine.Desktop;
using Autofac;

namespace AGS.Editor.Desktop
{
    public class DesktopEditorPlatform : IEditorPlatform
    {
        private readonly HostingGameDesktopWindowSize _windowSize;

        public DesktopEditorPlatform()
        {
            DotnetProject = new DotnetProject();
            _windowSize = new HostingGameDesktopWindowSize();
            SetHostedGameWindow(new Rectangle(800, 200, 1180, 800));
        }

        public IDotnetProject DotnetProject { get; private set; }

        public void SetResolverForGame(Resolver resolver)
        {
            HostingGameDesktopWindow hostedGame = new HostingGameDesktopWindow(_windowSize);
            resolver.Builder.RegisterInstance(_windowSize).As<IGameWindowSize>();
            resolver.Builder.RegisterInstance(hostedGame).As<IHostingWindow>();
        }

        public void SetHostedGameWindow(Rectangle windowSize)
        {
            _windowSize.Window = windowSize;
        }
    }
}