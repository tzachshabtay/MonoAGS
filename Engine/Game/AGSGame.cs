using System;
using AGS.API;
using Autofac;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;

namespace AGS.Engine
{
	public class AGSGame : IGame
	{
		private Resolver _resolver;
		private IRendererLoop _renderLoop;
		private GameWindow _game;
		private int _relativeSpeed;
		private const double UPDATE_RATE = 60.0;

		public AGSGame(IGameFactory factory, IGameState state, IGameEvents gameEvents)
		{
			Factory = factory;
			State = state;
			Events = gameEvents;
			_relativeSpeed = state.Speed;
		}

		public static IGame CreateEmpty()
		{
			Thread.CurrentThread.Name = UIThread;
			printRuntime();
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Resolver resolver = new Resolver (new EngineConfigFile());
			resolver.Build();
			AGSGame game = resolver.Container.Resolve<AGSGame>();
			game._resolver = resolver;
			return game;
		}

		public const string UIThread = "UI";

		#region IGame implementation

		public IGameFactory Factory { get; private set; }

		public IGameState State { get; private set; } 

		public IGameLoop GameLoop { get; private set; } 

		public ISaveLoad SaveLoad { get; private set; } 

		public IInput Input { get; private set; } 

		public IGameEvents Events { get; private set; }

		public Size VirtualResolution { get; private set; }

		public void Start(IGameSettings settings)
		{
			VirtualResolution = settings.VirtualResolution;
			GameLoop = _resolver.Container.Resolve<IGameLoop>(new TypedParameter (typeof(Size), VirtualResolution));
			using (_game = new GameWindow (settings.VirtualResolution.Width, 
                settings.VirtualResolution.Height, GraphicsMode.Default, settings.Title))
			{
				GL.ClearColor(0, 0.1f, 0.4f, 1);
				_game.Size = settings.WindowSize;
				setWindowState(settings);

				_game.Load += async (sender, e) =>
				{
					setVSync(settings);                    

					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					GL.Enable(EnableCap.Texture2D);

					TypedParameter gameParameter = new TypedParameter (typeof(GameWindow), _game);
					TypedParameter sizeParameter = new TypedParameter(typeof(Size), VirtualResolution);
					Input = _resolver.Container.Resolve<IInput>(gameParameter, sizeParameter); 
					TypedParameter inputParamater = new TypedParameter(typeof(IInput), Input);
					_renderLoop = _resolver.Container.Resolve<IRendererLoop>(inputParamater);
					updateResolver();
					SaveLoad = _resolver.Container.Resolve<ISaveLoad>();

					GL.MatrixMode(MatrixMode.Projection);

					GL.LoadIdentity();
					GL.Ortho(0, settings.VirtualResolution.Width, 0, settings.VirtualResolution.Height, -1, 1);
					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();

					await Events.OnLoad.InvokeAsync(sender, new AGSEventArgs());
				};
					
				_game.Resize += (sender, e) =>
				{
					GL.Viewport(0, 0, _game.Width, _game.Height);
				};

				_game.MouseDown += (sender, e) => 
				{
				};
				_game.KeyDown += (sender, e) =>  
				{
					if (e.Key == OpenTK.Input.Key.Escape) Quit();
				};
				_game.UpdateFrame += async (sender, e) =>
				{
					try
					{
						if (State.Paused) return;
						adjustSpeed();
						GameLoop.Update();
						await Events.OnRepeatedlyExecute.InvokeAsync(sender, new AGSEventArgs());
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
						throw ex;
					}
				};

				_game.RenderFrame += (sender, e) =>
				{
					try
					{
						// render graphics
						GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

						_renderLoop.Tick();

						_game.SwapBuffers();

						if (Repeat.OnceOnly("SetFirstRestart"))
						{
							SaveLoad.SetRestartPoint();
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Exception when rendering:");
						Debug.WriteLine(ex.ToString());
						throw;
					}
				};

				// Run the game at 60 updates per second

				_game.Run(UPDATE_RATE);
			}
		}

		public void Quit()
		{
			_game.Exit();
		}

		public TEntity Find<TEntity>(string id) where TEntity : class, IEntity
		{
			return State.Find<TEntity>(id);
		}
			
		#endregion

		private void updateResolver()
		{
			var updater = new ContainerBuilder ();
			updater.RegisterInstance(Input).As<IInput>();
			updater.RegisterInstance(_renderLoop).As<IRendererLoop>();
			updater.RegisterInstance(this).As<IGame>();

			updater.Update(_resolver.Container);
		}

		void adjustSpeed()
		{
			if (_relativeSpeed == State.Speed) return;

			_relativeSpeed = State.Speed;
			_game.TargetUpdateFrequency = UPDATE_RATE * (_relativeSpeed / 100f);
		}
        
        private void setVSync(IGameSettings settings)
        {
            switch (settings.Vsync)
            {
                case VsyncMode.On:
                    _game.VSync = VSyncMode.On;
                    break;
                case VsyncMode.Adaptive:
                    _game.VSync = VSyncMode.Adaptive;
                    break;
                case VsyncMode.Off:
                    _game.VSync = VSyncMode.Off;
                    break;
                default:
                    throw new NotSupportedException(settings.Vsync.ToString());
            }
        }
        
        private void setWindowState(IGameSettings settings)
        {
            switch (settings.WindowState)
            {
                case AGS.API.WindowState.FullScreen:
                    _game.WindowState = OpenTK.WindowState.Fullscreen;
                    break;
				case AGS.API.WindowState.Maximized:
                    _game.WindowState = OpenTK.WindowState.Maximized;
                    break;
				case AGS.API.WindowState.Minimized:
                    _game.WindowState = OpenTK.WindowState.Minimized;
                    break;
				case AGS.API.WindowState.Normal:
                    _game.WindowState = OpenTK.WindowState.Normal;
                    break;
                default:
                    throw new NotSupportedException(settings.WindowState.ToString());
            }
        }

		private static void printRuntime()
		{
			Type type = Type.GetType("Mono.Runtime");
			if (type != null)
			{                                          
				MethodInfo getDisplayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 
				if (getDisplayName != null)
				{
					object displayName = getDisplayName.Invoke(null, null);
					Debug.WriteLine(string.Format("Runtime: Mono- {0}", displayName)); 
				}
			}
		}
	}
}

