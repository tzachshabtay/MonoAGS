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

        public void SetResolverForGame(Resolver gameResolver, Resolver editorResolver)
        {
            var nativeWindw = editorResolver.Container.Resolve<OpenTK.INativeWindow>();
            HostingGameDesktopWindow hostedGame = new HostingGameDesktopWindow(_windowSize, nativeWindw);
            gameResolver.Builder.RegisterInstance(_windowSize).As<IGameWindowSize>();
            gameResolver.Builder.RegisterInstance(hostedGame).As<IWindowInfo>().As<IAGSWindowInfo>();
            gameResolver.Builder.RegisterType<AGSInput>().SingleInstance().As<IInput>().As<AGSInput>().As<IAGSInput>().OnActivated(e => e.Instance.Init(hostedGame));
        }

        public void SetHostedGameWindow(Rectangle windowSize)
        {
            _windowSize.Window = windowSize;
        }
    }
}