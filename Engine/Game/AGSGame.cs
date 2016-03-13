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

		public AGSGame(IGameFactory factory, IGameState state, IGameEvents gameEvents)
		{
			Factory = factory;
			State = state;
			Events = gameEvents;
		}

		public static IGame CreateEmpty()
		{
			Thread.CurrentThread.Name = UIThread;
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Resolver resolver = new Resolver ();
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
			using (var game = new GameWindow (settings.VirtualResolution.Width, 
                settings.VirtualResolution.Height, GraphicsMode.Default, settings.Title))
			{
				GL.ClearColor(0, 0.1f, 0.4f, 1);
                game.Size = settings.WindowSize;
                setWindowState(game, settings);
				game.Load += async (sender, e) =>
				{
                    setVSync(game, settings);                    

					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					GL.Enable(EnableCap.Texture2D);

					TypedParameter gameParameter = new TypedParameter (typeof(GameWindow), game);
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
					
				game.Resize += (sender, e) =>
				{
					GL.Viewport(0, 0, game.Width, game.Height);
				};

				game.MouseDown += (sender, e) => 
				{
				};
				game.KeyDown += (sender, e) =>  
				{
					if (e.Key == OpenTK.Input.Key.Escape) game.Exit();
				};
				game.UpdateFrame += async (sender, e) =>
				{
					try
					{
						if (State.Paused) return;
						GameLoop.Update();
						await Events.OnRepeatedlyExecute.InvokeAsync(sender, new AGSEventArgs());
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
						throw ex;
					}
				};

				game.RenderFrame += (sender, e) =>
				{
					try
					{
						// render graphics
						GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

						_renderLoop.Tick();

						game.SwapBuffers();

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

				game.Run(60.0);
			}
		}

		public TObject Find<TObject>(string id) where TObject : class, IObject
		{
			return State.Find<TObject>(id);
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
        
        private void setVSync(GameWindow game, IGameSettings settings)
        {
            switch (settings.Vsync)
            {
                case VsyncMode.On:
                    game.VSync = VSyncMode.On;
                    break;
                case VsyncMode.Adaptive:
                    game.VSync = VSyncMode.Adaptive;
                    break;
                case VsyncMode.Off:
                    game.VSync = VSyncMode.Off;
                    break;
                default:
                    throw new NotSupportedException(settings.Vsync.ToString());
            }
        }
        
        private void setWindowState(GameWindow game, IGameSettings settings)
        {
            switch (settings.WindowState)
            {
                case AGS.API.WindowState.FullScreen:
                    game.WindowState = OpenTK.WindowState.Fullscreen;
                    break;
				case AGS.API.WindowState.Maximized:
                    game.WindowState = OpenTK.WindowState.Maximized;
                    break;
				case AGS.API.WindowState.Minimized:
                    game.WindowState = OpenTK.WindowState.Minimized;
                    break;
				case AGS.API.WindowState.Normal:
                    game.WindowState = OpenTK.WindowState.Normal;
                    break;
                default:
                    throw new NotSupportedException(settings.WindowState.ToString());
            }
        }
	}
}

