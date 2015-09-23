using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using System.Diagnostics;

namespace AGS.Engine
{
	public class GLGraphicsFactory : IGraphicsFactory
	{
		private Dictionary<string, GLImage> _textures;
		private IContainer _resolver;

		public GLGraphicsFactory (Dictionary<string, GLImage> textures, IContainer resolver)
		{
			this._textures = textures;
			this._resolver = resolver;
		}

		public ISprite GetSprite()
		{
			ISprite sprite = _resolver.Resolve<ISprite>();
			return sprite;
		}

		public IAnimation LoadAnimationFromFolder (string folderPath, int delay = 1, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			AGSAnimationState state = new AGSAnimationState ();
			string[] files = Directory.GetFiles (folderPath);
			AGSAnimation animation = new AGSAnimation (animationConfig, state, files.Length);
			foreach (string file in files) 
			{
				
				if (file.EndsWith(".DS_Store")) continue; //MAC OS file
				var image = LoadImage (file, loadConfig);
				if (image == null) continue;
				ISprite sprite = GetSprite();
				sprite.Image = image;
				AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = delay };
				animation.Frames.Add (frame);
			}
			animation.Setup ();
			return animation;
		}

		public async Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 1, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await Task.Run(() => LoadAnimationFromFolder (folderPath, delay, animationConfig, loadConfig));
		}

		public IAnimation LoadAnimationFromSpriteSheet (string filePath, ISpriteSheet spriteSheet, 
			int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			Bitmap bitmap = new Bitmap (filePath);
			int cellsInRow = bitmap.Width / spriteSheet.CellWidth;
			int cellsInCol = bitmap.Height / spriteSheet.CellHeight;
			int cellsTotal = cellsInRow * cellsInCol;

			int startRow, startCol;
			Point mainStep, secondStep;

			switch (spriteSheet.Order) 
			{
				case SpriteSheetOrder.BottomLeftGoRight:
					startRow = cellsInCol - 1;
					startCol = 0;
					mainStep = new Point (1, 0);
					secondStep = new Point (0, -1);
					break;
				case SpriteSheetOrder.BottomLeftGoUp:
					startRow = cellsInCol - 1;
					startCol = 0;
					mainStep = new Point (0, -1);
					secondStep = new Point (1, 0);
					break;
				case SpriteSheetOrder.BottomRightGoLeft:
					startRow = cellsInCol - 1;
					startCol = cellsInRow - 1;
					mainStep = new Point (-1, 0);
					secondStep = new Point (0, -1);
					break;
				case SpriteSheetOrder.BottomRightGoUp:
					startRow = cellsInCol - 1;
					startCol = cellsInRow - 1;
					mainStep = new Point (0, -1);
					secondStep = new Point (-1, 0);
					break;
				case SpriteSheetOrder.TopLeftGoDown:
					startRow = 0;
					startCol = 0;
					mainStep = new Point (0, 1);
					secondStep = new Point (1, 0);
					break;
				case SpriteSheetOrder.TopLeftGoRight:
					startRow = 0;
					startCol = 0;
					mainStep = new Point (1, 0);
					secondStep = new Point (0, 1);
					break;
				case SpriteSheetOrder.TopRightGoDown:
					startRow = 0;
					startCol = cellsInRow - 1;
					mainStep = new Point (0, 1);
					secondStep = new Point (-1, 0);
					break;
				case SpriteSheetOrder.TopRightGoLeft:
					startRow = 0;
					startCol = cellsInRow - 1;
					mainStep = new Point (-1, 0);
					secondStep = new Point (0, 1);
					break;
				default:
					throw new NotSupportedException (string.Format("Sprite sheet order {0} is not supported", spriteSheet.Order));
			}

			int cellX = startCol;
			int cellY = startRow;
			int cellsGrabbed = 0;
			int cellsToGrab = spriteSheet.CellsToGrab < 0 ? cellsTotal : Math.Min (spriteSheet.CellsToGrab, cellsTotal);
			AGSAnimation animation = new AGSAnimation (animationConfig, new AGSAnimationState (), cellsToGrab);
			for (int currentCell = 0; cellsGrabbed < cellsToGrab; currentCell++) 
			{
				if (currentCell >= spriteSheet.StartFromCell) 
				{
					int tex = generateTexture ();
					Rectangle rect = new Rectangle (cellX * spriteSheet.CellWidth,
						                cellY * spriteSheet.CellHeight, spriteSheet.CellWidth, spriteSheet.CellHeight);
					Bitmap clone = crop (bitmap, rect);
					string path = string.Format ("{0}_{1}_{2}", rect.X, rect.Y, filePath);
					GLImage image = loadImage (tex, clone, path, loadConfig);
					//GLImage image = loadImage(tex, bitmap, rect, path);
					ISprite sprite = GetSprite();
					sprite.Image = image;
					sprite.Location = AGSLocation.Empty();

		
					AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = delay };
					animation.Frames.Add (frame);
					cellsGrabbed++;
				}

				if (cellX + mainStep.X >= cellsInRow ||
				    cellY + mainStep.Y >= cellsInCol) 
				{
					if (mainStep.X != 0) cellX = 0;
					else cellY = 0;
					cellX += secondStep.X;
					cellY += secondStep.Y;
				} 
				else 
				{
					cellX += mainStep.X;
					cellY += mainStep.Y;
				}
			}
			animation.Setup ();
			return animation;
		}

		public async Task<IAnimation> LoadAnimationFromSpriteSheetAsync (string filePath, ISpriteSheet spriteSheet, 
			int delay = 1, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await Task.Run(() => LoadAnimationFromSpriteSheet(filePath, spriteSheet, delay, animationConfig, loadConfig));
		}
			
		public GLImage LoadImageInner(string path, ILoadImageConfig config = null)
		{
			int tex = generateTexture ();
			try
			{
				Bitmap bitmap = new Bitmap (path);
				return loadImage (tex, bitmap, path, config);
			}
			catch (ArgumentException e)
			{
				Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", path, e.ToString());
				return null;
			}
		}

		public IImage LoadImage(Bitmap bitmap, ILoadImageConfig config = null, string id = null)
		{
			id = id ?? Guid.NewGuid ().ToString ();
			int tex = generateTexture ();
			return loadImage (tex, bitmap, id, config);
		}

		public IImage LoadImage(string path, ILoadImageConfig config = null)
		{
			return LoadImageInner (path, config);
		}

		public async Task<IImage> LoadImageAsync (string filePath, ILoadImageConfig config = null)
		{
			return await Task.Run(() => LoadImage (filePath, config));
		}

		private int generateTexture()
		{
			if (Thread.CurrentThread.Name != AGSGame.UIThread)
			{
				throw new InvalidOperationException ("Must generate textures on the UI thread");
			}
			int tex;
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.GenTextures(1, out tex);
			GL.BindTexture(TextureTarget.Texture2D, tex);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

			return tex;
		}
			
		private Bitmap crop(Bitmap bmp, Rectangle cropRect)
		{
			return bmp.Clone (cropRect, bmp.PixelFormat); //todo: improve performance by using FastBitmap
		}

		private GLImage loadImage(int texture, Bitmap bitmap, string path, ILoadImageConfig config)
		{
			manipulateImage(bitmap, config);
			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bitmap.UnlockBits(data);

			GLImage image = new GLImage (bitmap, path, texture);

			if (_textures != null)
				_textures.GetOrAdd (image.ID, () => image);
			return image;
		}

		private void manipulateImage(Bitmap bitmap, ILoadImageConfig config)
		{
			if (config == null) return;
			if (config.TransparentColorSamplePoint != null)
			{
				Color transparentColor = bitmap.GetPixel(config.TransparentColorSamplePoint.Value.X,
					                         config.TransparentColorSamplePoint.Value.Y);
				bitmap.MakeTransparent(transparentColor);
			}
		}
	}
}

