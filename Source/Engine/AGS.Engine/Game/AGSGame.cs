using System;
using AGS.API;
using Autofac;
using System.Diagnostics;
using System.Reflection;

namespace AGS.Engine
{
    public class AGSGame : IGame
    {
        private Resolver _resolver;
        private int _relativeSpeed;
        private readonly IMessagePump _renderMessagePump, _updateMessagePump;
        private readonly IGraphicsBackend _graphics;
        private readonly IGLUtils _glUtils;
        private readonly RepeatedlyExecuteEventArgs _repeatArgs = new RepeatedlyExecuteEventArgs();
        public const double UPDATE_RATE = 60.0;
        private int _renderFrameRetries;
        private static AGSUpdateThread _updateThread;
        private bool _shouldSetRestart = true;
        private int _gameIndex;
        private IAGSRenderPipeline _pipeline;
        private IRoomTransitionWorkflow _roomTransitionWorkflow;

        private static int _gameCount;
        private static bool _shouldSwapBuffers = true;

        public AGSGame(IGameState state, IGameEvents gameEvents, IRenderMessagePump renderMessagePump, IUpdateMessagePump updateMessagePump,
                       IGraphicsBackend graphics, IGLUtils glUtils)
        {
            _renderMessagePump = renderMessagePump;
            _renderMessagePump.SetSyncContext();
            _updateMessagePump = updateMessagePump;
            State = state;
            Events = gameEvents;
            _relativeSpeed = state.Speed;
            _graphics = graphics;
            _glUtils = glUtils;
            GLUtils = _glUtils;
        }

        public static IGameWindow GameWindow { get; private set; }

        public static IGame Game { get; private set; }

        public static IDevice Device { get; set; }

        public static IShader Shader { get; set; }

        public static IGLUtils GLUtils { get; private set; }

        public static IGame Create(IGameSettings settings)
        {
            var resolver = new Resolver(Device, settings);
            return Create(resolver);
        }

        public static IGame Create(Resolver resolver)
        {
            UIThreadID = Environment.CurrentManagedThreadId;

            printInfo();
            resolver.Build();
            AGSGame game = resolver.Container.Resolve<AGSGame>();
            game._resolver = resolver;
            Game = game;
            return game;
        }

        public static int UIThreadID;
        public static int UpdateThreadID;

        #region IGame implementation

        public IGameFactory Factory { get; private set; }

        public IGameState State { get; private set; }

        public IGameLoop GameLoop { get; private set; }

        public IRendererLoop RenderLoop { get; private set; }

        public IRenderPipeline RenderPipeline => _pipeline;

        public ISaveLoad SaveLoad { get; private set; }

        public IInput Input { get; private set; }

        public IGameEvents Events { get; private set; }

        public IAudioSystem Audio { get; private set; }

        public IRuntimeSettings Settings { get; private set; }

        public IHitTest HitTest { get; private set; }

        public ICoordinates Coordinates { get; private set; }

        public IResolver Resolver => _resolver;

        public void Start()
        {
            _gameCount++;
            _gameIndex = _gameCount;
            var settings = _resolver.Container.Resolve<IGameSettings>();
            GameLoop = _resolver.Container.Resolve<IGameLoop>(new TypedParameter(typeof(Size), settings.VirtualResolution));
            TypedParameter settingsParameter = new TypedParameter(typeof(IGameSettings), settings);

            bool isNewWindow = false;
            if (GameWindow == null)
            {
                isNewWindow = true;
                try { GameWindow = _resolver.Container.Resolve<IGameWindow>(settingsParameter); }
                catch (Exception ese)
                {
                    Debug.WriteLine(ese.ToString());
                    throw;
                }
                _updateThread = new AGSUpdateThread(GameWindow);
            }

            //using (GameWindow)
            {
                try
                {
                    GameWindow.Load += (sender, e) =>
                    {
                        _graphics.Init();
                        onGameWindowLoaded(settingsParameter, settings);
                    };
                    if (!isNewWindow) onGameWindowLoaded(settingsParameter, settings);

                    GameWindow.Resize += (sender, e) =>
                    {
                        Events.OnScreenResize.Invoke();
                    };

                    _updateThread.OnThreadStarted += (sender, e) =>
                    {
                        _updateMessagePump.SetSyncContext();
                    };

                    _updateThread.UpdateFrame += onUpdateFrame;

                    GameWindow.RenderFrame += onRenderFrame;

                    // Run the game at 60 updates per second
                    _updateThread.Run(UPDATE_RATE, true);
                    if (isNewWindow)
                    {
                        GameWindow.Run(UPDATE_RATE);
                    }
                }
                catch (Exception exx)
                {
                    Debug.WriteLine(exx.ToString());
                    throw;
                }
            }
        }

        public void Quit()
        {
            GameWindow.Exit();
        }

        public TEntity Find<TEntity>(string id) where TEntity : class, IEntity
        {
            return State.Find<TEntity>(id);
        }

        #endregion

        private void onUpdateFrame(object sender, FrameEventArgs e)
        {
            try
            {
                _updateMessagePump.PumpMessages();
                _repeatArgs.DeltaTime = e.Time;
                Events.OnRepeatedlyExecuteAlways.Invoke(_repeatArgs);
                if (State.Paused) return;
                adjustSpeed();
                GameLoop.Update(false);
                Events.OnRepeatedlyExecute.Invoke(_repeatArgs);

                _pipeline?.Update();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                _renderMessagePump.Post(_ => throw ex, null); //throwing on render thread to ensure proper application crash.
            }
        }

        private void onRenderFrame(object sender, FrameEventArgs e)
        {
            if (RenderLoop == null || _renderFrameRetries > 3) return;
            try
            {
                bool rendered = false;
                try
                {
                    _graphics.BeginTick();
                    _renderMessagePump.PumpMessages();
                    // render graphics
                    if (_gameCount == 1 || _gameIndex == 2) //if we have 2 games (editor + game) we want the editor layout drawn above the game so only clear screen from the actual game
                    {
                        var bgColor = State.Room?.BackgroundColor ?? Colors.Black;
                        var color = bgColor.ToGLColor();
                        _graphics.ClearColor(color.R, color.G, color.B, color.A);
                        _graphics.ClearScreen();
                    }
                    Events.OnBeforeRender.Invoke();

                    rendered = render();
                }
                finally 
                {
                    _graphics.EndTick();
                }
                if (rendered)
                {
                    if (_gameIndex == 1) //if we have 2 games (editor + game) editor is game index 1 and should be drawn last, so only the editor should swap buffers
                    {
                        if (_shouldSwapBuffers)
                        {
                            GameWindow.SwapBuffers();
                        }
                        _shouldSwapBuffers = true;
                    }
                }
                else if (_gameIndex != 1)
                {
                    _shouldSwapBuffers = false;
                }
                if (_shouldSetRestart)
                {
                    _shouldSetRestart = false;
                    SaveLoad.SetRestartPoint();
                }
            }
            catch (Exception ex)
            {
                _renderFrameRetries++;
                Debug.WriteLine("Exception when rendering:");
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        private bool render()
        {
            if (State.DuringRoomTransition)
                return _roomTransitionWorkflow.Render();
            RenderLoop.Tick();
            return true;
        }

        private void onGameWindowLoaded(TypedParameter settingsParameter, IGameSettings settings)
        {
            TypedParameter gameWindowParameter = new TypedParameter(typeof(IGameWindow), GameWindow);
            Settings = _resolver.Container.Resolve<IRuntimeSettings>(settingsParameter, gameWindowParameter);

            _glUtils.GenBuffers();

            Factory = _resolver.Container.Resolve<IGameFactory>();
            _resolver.Resolve<ITextureFactory>(); //Need to resolve this early in the rendering thread, as it creates the static empty texture and therefore need to be in the rendering thread

            TypedParameter gameParameter = new TypedParameter(typeof(IGame), this);
            Settings.Defaults.MessageBox = _resolver.Container.Resolve<IMessageBoxSettings>(gameParameter);

            var input = _resolver.Container.Resolve<IInput>();
            Input = input;
            TypedParameter inputParamater = new TypedParameter(typeof(IInput), Input);
            _pipeline = _resolver.Container.Resolve<IAGSRenderPipeline>(gameParameter);
            TypedParameter pipelineParameter = new TypedParameter(typeof(IAGSRenderPipeline), _pipeline);
            RenderLoop = _resolver.Container.Resolve<IRendererLoop>(inputParamater, gameParameter,
                                                                    gameWindowParameter, pipelineParameter);
            updateResolver();
            HitTest = _resolver.Container.Resolve<IHitTest>();
            Audio = _resolver.Container.Resolve<IAudioSystem>();
            SaveLoad = _resolver.Container.Resolve<ISaveLoad>();
            Coordinates = _resolver.Container.Resolve<ICoordinates>();
            _roomTransitionWorkflow = _resolver.Container.Resolve<IRoomTransitionWorkflow>();

            _glUtils.AdjustResolution(settings.VirtualResolution.Width, settings.VirtualResolution.Height);

            _updateMessagePump.Post(_ => Events.OnLoad.Invoke(), null);
        }

        private void updateResolver()
        {
            var updater = new ContainerBuilder();
            updater.RegisterInstance(Input).As<IInput>();
            updater.RegisterInstance(RenderLoop).As<IRendererLoop>();
            updater.RegisterInstance(_pipeline).As<IRenderPipeline>().As<IAGSRenderPipeline>();
            updater.RegisterInstance(this).As<IGame>();
            updater.RegisterInstance(Settings).As<IGameSettings>();
            updater.RegisterInstance(Settings).As<IRuntimeSettings>();

            updater.Update(_resolver.Container);
        }

        void adjustSpeed()
        {
            if (_relativeSpeed == State.Speed) return;

            _relativeSpeed = State.Speed;
            GameWindow.TargetUpdateFrequency = UPDATE_RATE * (_relativeSpeed / 100f);
            _updateThread.TargetUpdateFrequency = UPDATE_RATE * (_relativeSpeed / 100f);
        }

        private static void printInfo()
        {
            printVersion();
            printRuntime();
        }

        private static void printVersion()
        {
            Debug.WriteLine($"MonoAGS version: {ThisAssembly.AssemblyVersion}");
            Debug.WriteLine($"Detailed version: {ThisAssembly.AssemblyInformationalVersion}");
            Debug.WriteLine($"File version: {ThisAssembly.AssemblyFileVersion}");
        }

        private static void printRuntime()
        {
            Type type = Type.GetType("Mono.Runtime");

            MethodInfo getDisplayName = type?.GetRuntimeMethod("GetDisplayName", new Type[] { });

            object displayName = getDisplayName?.Invoke(null, null);
            Debug.WriteLine($"Runtime: Mono- {displayName ?? "Unknown"}");
        }
    }
}
