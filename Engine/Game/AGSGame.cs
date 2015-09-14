using System;
using API;
using Autofac;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

namespace Engine
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

		public IInputEvents Input { get; private set; } 

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
					_renderLoop = _resolver.Container.Resolve<IRendererLoop>(gameParameter);
					Input = _resolver.Container.Resolve<IInputEvents>(gameParameter); 

					GL.MatrixMode(MatrixMode.Projection);

					GL.LoadIdentity();
					GL.Ortho(0, width, 0, height, -1, 1);
					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();

					await Events.OnLoad.InvokeAsync(sender, e);
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
						// add game logic, input handling
						GameLoop.Update();
						await Events.OnRepeatedlyExecute.InvokeAsync(sender, e);
					}
					catch (Exception ex)
					{
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
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Exception when rendering:");
						Debug.WriteLine(ex.ToString());
						throw ex;
					}
				};

				// Run the game at 60 updates per second

				game.Run(60.0);
			}
		}

		#endregion
	}
}

