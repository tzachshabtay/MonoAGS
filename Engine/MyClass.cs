using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;
using API;
using System.Collections.Generic;
using Autofac;

namespace Engine
{
	public class MyClass
	{
		//private List<ISprite> sprites;
		Dictionary<string, GLImage> images;
		IGameState gameState;

		int viewportX = 800;
		int viewportY = 600;

		public MyClass ()
		{
		}

		private void LoadImages(string dir)
		{
			string[] files = Directory.GetFiles (dir);
			images = new Dictionary<string, GLImage> (files.Length);
			GLGraphicsFactory loader = new GLGraphicsFactory (images);
			//sprites = new List<ISprite> (files.Length);
			//AGSObject obj = new AGSObject ();
			//AGSCharacter character = new AGSCharacter (obj);
			//AGSPlayer player = new AGSPlayer { Character = character };
			AGSRoom room = new AGSRoom ("Demo", gameState.Player, new AGSViewport { 
				Follower = new AGSViewportFollower (1f, 1f) { Enabled = false }
			});
			//room.Viewport.X = 200f;
			//gameState.Player = player;
			int i = 0;
			foreach (string file in files) 
			{
				if (!file.Contains("Village")) {
					i++;
					continue;
				}
				if (file.Contains("Village")) 
				{
					try
					{
						Bitmap debugBitmap;
						bool[][] mask = GraphicsUtils.LoadMask (file, out debugBitmap, debugMaskPath: "debug");
						room.WalkableAreas.Add(new AGSArea { Mask = mask });
						//GLBitmapRenderer debugMaskRenderer = new GLBitmapRenderer(loader, debugBitmap);
						//room.AddCustomRenderer(debugMaskRenderer);
					}
					catch (Exception e) 
					{
						Console.WriteLine (e.ToString ());
						throw;
					}
					AGSSpriteSheet downSheet = new AGSSpriteSheet (128, 128, 0, 13);
					AGSSpriteSheet downLeftSheet = new AGSSpriteSheet (128, 128, 13, 13);
					AGSSpriteSheet downRightSheet = new AGSSpriteSheet (128, 128, 26, 13);
					AGSSpriteSheet leftSheet = new AGSSpriteSheet (128, 128, 39, 13);
					AGSSpriteSheet upSheet = new AGSSpriteSheet (128, 128, 52, 13);
					AGSSpriteSheet upLeftSheet = new AGSSpriteSheet (128, 128, 65, 13);
					AGSSpriteSheet upRightSheet = new AGSSpriteSheet (128, 128, 78, 13);
					AGSSpriteSheet rightSheet = new AGSSpriteSheet (128, 128, 91, 13);

					AGSDirectionalAnimation walkAnimation = new AGSDirectionalAnimation
					{
						Left = loader.LoadAnimationFromSpriteSheet(file, leftSheet),
						Right = loader.LoadAnimationFromSpriteSheet(file, rightSheet),
						Down = loader.LoadAnimationFromSpriteSheet(file, downSheet),
						Up = loader.LoadAnimationFromSpriteSheet(file, upSheet),
						DownLeft = loader.LoadAnimationFromSpriteSheet(file, downLeftSheet),
						DownRight = loader.LoadAnimationFromSpriteSheet(file, downRightSheet),
						UpLeft = loader.LoadAnimationFromSpriteSheet(file, upLeftSheet),
						UpRight = loader.LoadAnimationFromSpriteSheet(file, upRightSheet),
					};

					AGSDirectionalAnimation idleAnimation = new AGSDirectionalAnimation 
					{
						Left = loader.LoadAnimationFromSpriteSheet (file, new AGSSpriteSheet (128, 128, 39, 1)),
						Right = loader.LoadAnimationFromSpriteSheet (file, new AGSSpriteSheet (128, 128, 91, 1)),
						Down = loader.LoadAnimationFromSpriteSheet(file, new AGSSpriteSheet(128, 128, 0, 1)),
						Up = loader.LoadAnimationFromSpriteSheet(file, new AGSSpriteSheet(128, 128, 52, 1)),
					};

					//IAnimation animation = loader.LoadAnimationFromSpriteSheet (file, sheet, 1);
					/*foreach (var frame in animation.Frames) 
					{
						images.Add (frame.Sprite.Image.ID, (GLImage)frame.Sprite.Image);
					}*/
					AGSCharacter dude = new AGSCharacter();
					dude.WalkAnimation = walkAnimation;
					dude.IdleAnimation = idleAnimation;
					dude.StartAnimation (idleAnimation.Left);
					dude.Z = 50;
					dude.Hotspot = "Dude";
					//obj2.StartAnimation(animation);
					//obj2.Z = 50;
					gameState.Player.Character = dude;
					gameState.Player.Character.Room = room;
					gameState.Player.Character.Anchor = new AGSPoint (0.5f, 0.2f);
					//player.Character.Anchor = new AGSPoint (0f, 0f);
					gameState.Player.Character.PlaceOnWalkableArea ();
					room.Viewport.Follower.Target = gameState.Player.Character;
					room.Objects.Add(dude); 

					//i++;
					//continue;
				}
				GLImage image = loader.LoadImageInner (file);
				//images.Add (image.ID, image);
				//ISprite sprite = new AGSSprite { Image = image, Location = new AGSLocation() };
				//if (i == 2)
				//	sprite.Angle = 45f;
				//sprites.Add (sprite);

				if (i == 0) 
				{
					room.Background = new AGSObject(new AGSSprite()) { Image = image };
				}
				else
					room.Objects.Add (new AGSObject(new AGSSprite()) { Image = image, Anchor = new AGSPoint() });
				//gameState.Player.Character.CurrentImage = sprite;
				i++;
			}
			//GLImage mm = loader.LoadImageInner(Directory.GetCurrentDirectory() + "/Assets/2.png");
			//GLImage mm = loader.LoadImageInner("Assets/Rooms/EmptyStreet/bg.png");
			//room.Objects.Add (new AGSObject(new AGSSprite()) { Image = mm, Anchor = new AGSPoint() });

			Resolver resolver = new Resolver ();
			resolver.Build();
			AGSLabel label = resolver.Container.Resolve<AGSLabel>();
			label.Anchor = new AGSPoint ();
			label.ScaleTo(100f, 40f);
			label.Tint = Color.Blue;
			label.X = 400f;
			label.Y = 20f;
			label.Text = "Label";
			AGSBorderStyle border = new AGSBorderStyle 
				{ LineWidth = 10f, Color = Color.Green };
			label.Border = border;
			gameState.UI.Add(label);
		}

		public void Run()
		{
			string dir = Directory.GetCurrentDirectory ();
			dir = Path.Combine (dir, "Assets");
			//dir = Path.Combine(dir, "blue graph.png");
			Console.WriteLine (dir);
			Color midnightBlue = Color.MidnightBlue;
			//int textureId = 0;
			GLImageRenderer renderer = null;
			gameState = new AGSGameState (new AGSPlayer());
			GLRendererLoop loop = null; 
			AGSGameLoop gameLoop = new AGSGameLoop (gameState);
			GLText text = null;
			Thread.CurrentThread.Name = AGSGame.UIThread;

			using (var game = new GameWindow(viewportX, viewportY, GraphicsMode.Default, "AGS"))
			{
				text = new GLText (/*game,*/ "Hello World");
				TwoButtonsInputScheme inputScheme = new TwoButtonsInputScheme (gameState,
					new GLInputEvents (game), text);
				inputScheme.Start ();

				GL.ClearColor(0, 0.1f, 0.4f, 1);
				game.Load += (sender, e) =>
				{
					// setup settings, load textures, sounds
					game.VSync = VSyncMode.On;

					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					GL.Enable (EnableCap.Texture2D);

					//GL.Enable(EnableCap.DepthTest);

					//textureId = LoadTexture (dir);
					LoadImages(dir);
					renderer = new GLImageRenderer(images, new GLMatrixBuilder(), 
						new GLBoundingBox(), new GLColor(), new GLTextureRenderer());
					loop = new GLRendererLoop (gameState, renderer);

					GL.MatrixMode (MatrixMode.Projection);
				
					GL.LoadIdentity ();
					//GL.Ortho (-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
					GL.Ortho(0,800,0,600,-1,1);
					GL.MatrixMode (MatrixMode.Modelview);
					GL.LoadIdentity ();

					//text = new GLText (game, "Hello World");
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
				game.UpdateFrame += (sender, e) =>
				{
					// add game logic, input handling


					gameLoop.Update();
					count++;
					//gameState.Player.Character.Room.Viewport.X -= 0.1f;
					//gameState.Player.Character.Angle += 0.01f;
					//gameState.Player.Character.ScaleBy(1f + gameState.Player.Character.Angle * 0.01f, 
					//	1f + gameState.Player.Character.Angle * 0.01f);
				};

				game.RenderFrame += (sender, e) =>
				{
					// render graphics
					/*GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

					GL.Begin(PrimitiveType.Triangles);

					GL.Color3(midnightBlue);
					GL.Vertex2(-1.0f, 1.0f);
					GL.Color3(Color.SpringGreen);
					GL.Vertex2(0.0f, -1.0f);
					GL.Color3(Color.Ivory);
					GL.Vertex2(1.0f, 1.0f);

					GL.End();

					game.SwapBuffers();*/

					GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

					loop.Tick();
					text.Render();
					//sprites[1].X = 100 + count % 100;
					//float factor = ((float)(count % 100)) / 100f;
					//sprites[1].Opacity = factor;
					//sprites[1].ScaleBy(factor, factor);
					//foreach (var image in sprites)
					//{
					//	renderer.Render(image);
					//}
					//DrawImage(textureId);
					//DrawImage(textureId, 100 + count % 100, 100);

					game.SwapBuffers();
				};

				// Run the game at 60 updates per second
				game.Run(60.0);
			}
		}
		int count;

		public int LoadTexture(string file)
		{
			int tex;
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.GenTextures(1, out tex);
			GL.BindTexture(TextureTarget.Texture2D, tex);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

			Bitmap bitmap = new Bitmap(file);

			BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bitmap.UnlockBits(data);

			return tex;
		}
			
		public void DrawImage(int texture, float x = 0, float y = 0)
		{
			GL.BindTexture (TextureTarget.Texture2D, texture);

			GL.Begin (PrimitiveType.Quads);

			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (x + 0f, y + 0f);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (x + 300f, y + 0f);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (x + 300f, y + 300f);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (x + 0f, y + 300f);

			GL.End ();
		}
	}
}

