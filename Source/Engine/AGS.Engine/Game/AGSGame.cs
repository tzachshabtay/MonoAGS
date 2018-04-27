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
        private int _updateFrameRetries = 0, _renderFrameRetries = 0;
        private static AGSUpdateThread _updateThread;
        private bool _shouldSetRestart = true;
        private int _gameIndex;
        private IAGSRenderPipeline _pipeline;

        private static int _gameCount;
        private static bool _shouldSwapBuffers = true;

        public AGSGame(IGameState state, IGameEvents gameEvents, IRenderMessagePump renderMessagePump, IUpdateMessagePump updateMessagePump,
                       IGraphicsBackend graphics, IGLUtils glUtils)
		{
            _renderMessagePump = renderMessagePump;
            _renderMessagePump.SetSyncContext ();
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

        public static Resolver Resolver => ((AGSGame)Game)._resolver;

        public static IGLUtils GLUtils { get; private set; }

		public static IGame CreateEmpty(Resolver resolver = null)
		{
			UIThreadID = Environment.CurrentManagedThreadId;

			printRuntime();
			resolver = resolver ?? new Resolver(Device);
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

		public IAudioSettings AudioSettings { get; private set; }

        public IRuntimeSettings Settings { get; private set; }

        public IHitTest HitTest { get; private set; }

        public Resolver GetResolver() => _resolver;

        public void Start(IGameSettings settings)
		{
            _gameCount++;
            _gameIndex = _gameCount;
			GameLoop = _resolver.Container.Resolve<IGameLoop>(new TypedParameter (typeof(AGS.API.Size), settings.VirtualResolution));
            TypedParameter settingsParameter = new TypedParameter(typeof(IGameSettings), settings);

            bool isNewWindow = false;
            if (GameWindow == null)
            {
                isNewWindow = true;
                try { GameWindow = Resolver.Container.Resolve<IGameWindow>(settingsParameter); }
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
                    _updateThread.Run(UPDATE_RATE);
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

        private async void onUpdateFrame(object sender, FrameEventArgs e)
        {
            if (_updateFrameRetries > 3) return;
            try
            {
                _updateMessagePump.PumpMessages();
                _repeatArgs.DeltaTime = e.Time;
                await Events.OnRepeatedlyExecuteAlways.InvokeAsync(_repeatArgs);
                if (State.Paused) return;
                adjustSpeed();
                GameLoop.Update();

                //Invoking repeatedly execute asynchronously, as if one subscriber is waiting on another subscriber the event will 
                //never get to it (for example: calling ChangeRoom from within RepeatedlyExecute calls StopWalking which 
                //waits for the walk to stop, only the walk also happens on RepeatedlyExecute and we'll hang.
                //Since we're running asynchronously, the next UpdateFrame will call RepeatedlyExecute for the walk cycle to stop itself and we're good.
                ///The downside of this approach is that we need to look out for re-entrancy issues.
                await Events.OnRepeatedlyExecute.InvokeAsync(_repeatArgs);

                _pipeline?.Update();
            }
            catch (Exception ex)
            {
                _updateFrameRetries++;
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }

        private void onRenderFrame(object sender, FrameEventArgs e)
        {
            if (RenderLoop == null || _renderFrameRetries > 3) return;
            try
            {
                _renderMessagePump.PumpMessages();
                // render graphics
                if (_gameCount == 1 || _gameIndex == 2) //if we have 2 games (editor + game) we want the editor layout drawn above the game so only clear screen from the actual game
                {
                    _graphics.ClearScreen();
                }
                Events.OnBeforeRender.Invoke();

                if (RenderLoop.Tick())
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

        private void onGameWindowLoaded(TypedParameter settingsParameter, IGameSettings settings)
        {
            TypedParameter gameWindowParameter = new TypedParameter(typeof(IGameWindow), GameWindow);
            Settings = _resolver.Container.Resolve<IRuntimeSettings>(settingsParameter, gameWindowParameter);

            _graphics.ClearColor(0f, 0f, 0f, 1f);

            _graphics.Init();
            _glUtils.GenBuffers();

            Factory = _resolver.Container.Resolve<IGameFactory>();

            IAGSInput input = _resolver.Container.Resolve<IAGSInput>();
            input.Init(settings.VirtualResolution);
            Input = input;
            TypedParameter inputParamater = new TypedParameter(typeof(IInput), Input);
            TypedParameter gameParameter = new TypedParameter(typeof(IGame), this);
            _pipeline = _resolver.Container.Resolve<IAGSRenderPipeline>(gameParameter);
            TypedParameter pipelineParameter = new TypedParameter(typeof(IAGSRenderPipeline), _pipeline);
            RenderLoop = _resolver.Container.Resolve<IRendererLoop>(inputParamater, gameParameter, 
                                                                    gameWindowParameter, pipelineParameter);
            updateResolver();
            HitTest = _resolver.Container.Resolve<IHitTest>();
            AudioSettings = _resolver.Container.Resolve<IAudioSettings>();
            SaveLoad = _resolver.Container.Resolve<ISaveLoad>();

            _glUtils.AdjustResolution(settings.VirtualResolution.Width, settings.VirtualResolution.Height);

            Events.OnLoad.Invoke();
        }

		private void updateResolver()
		{
			var updater = new ContainerBuilder ();
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

		private static void printRuntime()
		{
			Type type = Type.GetType("Mono.Runtime");
			
			MethodInfo getDisplayName = type?.GetRuntimeMethod("GetDisplayName", new Type[]{}); 
				
            object displayName = getDisplayName?.Invoke(null, null);
			Debug.WriteLine($"Runtime: Mono- {displayName ?? "Unknown"}"); 
		}
	}
}

