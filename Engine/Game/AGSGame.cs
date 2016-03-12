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

		public void Start(string title, int width, int height)
		{
			VirtualResolution = new Size (width, height);
			GameLoop = _resolver.Container.Resolve<IGameLoop>(new TypedParameter (typeof(Size), VirtualResolution));
			using (var game = new GameWindow (width, height, GraphicsMode.Default, title))
			{
				GL.ClearColor(0, 0.1f, 0.4f, 1);
				game.Load += async (sender, e) =>
				{
					// setup settings, load textures, sounds
					game.VSync = VSyncMode.On;

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
					GL.Ortho(0, width, 0, height, -1, 1);
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
	}
}

