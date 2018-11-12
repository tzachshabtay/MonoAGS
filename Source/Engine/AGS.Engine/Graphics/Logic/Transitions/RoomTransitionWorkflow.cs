using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class RoomTransitionWorkflow : IRoomTransitionWorkflow
    {
        private Func<bool> _render;
        private readonly IRoomTransitions _transitions;
        private readonly IWindowInfo _window;
        private readonly IRendererLoop _rendererLoop;
        private readonly Resolver _resolver;
        private readonly IGameEvents _events;
        private readonly IGameLoop _loop;
        private readonly IAGSRenderPipeline _pipeline;
        private readonly IGLUtils _glUtils;
        private readonly DummyWindow _dummyWindow;
        private readonly IGameSettings _noAspectRatioSettings, _settings;
        private readonly IAGSGameState _state;
        private readonly IDisplayList _displayList;

        private IFrameBuffer _fromTransitionBuffer, _toTransitionBuffer;        

        public RoomTransitionWorkflow(IRoomTransitions transitions, IWindowInfo window, IRendererLoop rendererLoop, 
                                      Resolver resolver, IGameEvents events, IGameLoop loop, IAGSRenderPipeline pipeline, 
                                      IGameSettings settings, IAGSGameState state, IDisplayList displayList, IGLUtils glUtils)
        {
            _glUtils = glUtils;
            _dummyWindow = new DummyWindow();
            _transitions = transitions;
            _transitions.Transition = new RoomTransitionInstant();
            _window = window;
            _rendererLoop = rendererLoop;
            _resolver = resolver;
            _events = events;
            _loop = loop;
            _pipeline = pipeline;
            _state = state;
            _displayList = displayList;
            _settings = settings;
            _noAspectRatioSettings = new AGSGameSettings(settings.Title, settings.VirtualResolution, preserveAspectRatio: false);
            state.OnRoomChangeRequired.SubscribeToAsync(onRoomChangeRequired);
        }

        public bool Render()
        {
            var render = _render;
            if (render == null) return false;
            return render();
        }

        private async Task onRoomChangeRequired(RoomTransitionEventArgs args)
        {
            var from = args.From;
            var to = args.To;
            var afterTransitionFadeout = args.AfterTransitionFadeOut;

            from?.Events.OnBeforeFadeOut.Invoke();

            var transition = _transitions.Transition;

            await waitForRender(() =>
            {
                if (transition == null || from == null || _state.Cutscene.IsSkipping) return false;
                if (!transition.RenderBeforeLeavingRoom(_displayList.GetDisplayList(_state.Viewport)))
                {
                    if (_fromTransitionBuffer == null) _fromTransitionBuffer = renderToBuffer();
                    return false;
                }
                return true;
            });

            afterTransitionFadeout?.Invoke(); //this places player in new position and change state's room
            from?.Events.OnAfterFadeOut.Invoke();
            to?.Events.OnBeforeFadeIn.Invoke();
            _events.OnRoomChanging.Invoke();

            _loop.Update(true);
            _pipeline.Update();

            await waitForRender(() => 
            {
                if (transition == null || from == null || to == null || _state.Cutscene.IsSkipping) return false;
                _glUtils.AdjustResolution(_settings.VirtualResolution.Width, _settings.VirtualResolution.Height);
                if (_toTransitionBuffer == null) _toTransitionBuffer = renderToBuffer();
                _dummyWindow.GameSubWindow = new Rectangle(0, 0, (int)_window.AppWindowWidth, (int)_window.AppWindowHeight);
                _glUtils.RefreshViewport(_noAspectRatioSettings, _dummyWindow, _state.Viewport, false);
                if (!transition.RenderTransition(_fromTransitionBuffer, _toTransitionBuffer))
                {
                    return false;
                }
                return true;
            });
            _fromTransitionBuffer = null;
            _toTransitionBuffer = null;

            await waitForRender(() =>
            {
                if (transition == null || to == null || _state.Cutscene.IsSkipping) return false;
                if (!transition.RenderAfterEnteringRoom(_displayList.GetDisplayList(_state.Viewport)))
                {
                    return false;
                }
                return true;
            });

            _transitions.SetOneTimeNextTransition(null);
        }

        private async Task waitForRender(Func<bool> action)
        {
            var completion = new TaskCompletionSource<object>();
            _render = () =>
            {
                if (action()) return true;
                _render = null;
                completion.TrySetResult(null);
                return false;
            };
            await completion.Task;
        }

        private IFrameBuffer renderToBuffer()
        {
            TypedParameter sizeParam = new TypedParameter(typeof(Size), new Size(
                (int)_window.AppWindowWidth, (int)_window.AppWindowHeight));
            IFrameBuffer frameBuffer = _resolver.Container.Resolve<IFrameBuffer>(sizeParam);
            frameBuffer.Begin();
            _rendererLoop.Tick();
            frameBuffer.End();
            return frameBuffer;
        }

        private class DummyWindow : IWindowInfo
        {
            public float AppWindowHeight => throw new NotImplementedException();
            public float AppWindowWidth => throw new NotImplementedException();
            public Rectangle GameSubWindow { get; set; }
#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }
    }
}
