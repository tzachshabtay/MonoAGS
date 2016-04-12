using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
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
		private IResourceLoader _resources;

		public GLGraphicsFactory (Dictionary<string, GLImage> textures, IContainer resolver)
		{
			this._textures = textures;
			this._resolver = resolver;
			this._resources = resolver.Resolve<IResourceLoader>();
		}

		public ISprite GetSprite()
		{
			ISprite sprite = _resolver.Resolve<ISprite>();
			return sprite;
		}

		public IDirectionalAnimation LoadDirectionalAnimationFromFolders(string baseFolder, string leftFolder = null,
			string rightFolder = null, string downFolder = null, string upFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			if (leftFolder == null && rightFolder == null && downFolder == null && upFolder == null) return null;

			AGSDirectionalAnimation dirAnimation = new AGSDirectionalAnimation ();
			if (leftFolder != null) dirAnimation.Left = LoadAnimationFromFolder(baseFolder + leftFolder, delay, animationConfig, loadConfig);
			if (rightFolder != null) dirAnimation.Right = LoadAnimationFromFolder(baseFolder + rightFolder, delay, animationConfig, loadConfig);
			if (downFolder != null) dirAnimation.Down = LoadAnimationFromFolder(baseFolder + downFolder, delay, animationConfig, loadConfig);
			if (upFolder != null) dirAnimation.Up = LoadAnimationFromFolder(baseFolder + upFolder, delay, animationConfig, loadConfig);

			if (dirAnimation.Left != null && dirAnimation.Right == null)
			{
				dirAnimation.Right = createLeftRightAnimation(dirAnimation.Left);
			}

			if (dirAnimation.Right != null && dirAnimation.Left == null)
			{
				dirAnimation.Left = createLeftRightAnimation(dirAnimation.Right);
			}

			return dirAnimation;
		}

		public IAnimation LoadAnimationFromFiles(int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files)
		{
			return loadAnimationFromResources(_resources.LoadResources(files), delay, animationConfig, loadConfig);
		}

		public IAnimation LoadAnimationFromFolder (string folderPath, int delay = 4, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return loadAnimationFromResources(_resources.LoadResources(folderPath), delay, animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 4, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await Task.Run(() => LoadAnimationFromFolder (folderPath, delay, animationConfig, loadConfig));
		}

		public IAnimation LoadAnimationFromSpriteSheet (ISpriteSheet spriteSheet, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			string filePath = spriteSheet.Path;
			IResource resource = _resources.LoadResource(filePath);
			if (resource == null)
			{
				throw new InvalidOperationException ("Failed to load sprite sheet from " + filePath);
			}
			Bitmap bitmap = new Bitmap (resource.Stream);
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
					IBitmap clone = crop (bitmap, rect);
					string path = string.Format ("{0}_{1}_{2}", rect.X, rect.Y, filePath);
					GLImage image = loadImage (tex, clone, path, loadConfig, spriteSheet);
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

		public async Task<IAnimation> LoadAnimationFromSpriteSheetAsync (ISpriteSheet spriteSheet, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await Task.Run(() => LoadAnimationFromSpriteSheet(spriteSheet, delay, animationConfig, loadConfig));
		}
			
		public GLImage LoadImageInner(string path, ILoadImageConfig config = null)
		{
			IResource resource = _resources.LoadResource(path);
			return loadImage(resource, config);
		}

		public IImage LoadImage(IBitmap bitmap, ILoadImageConfig config = null, string id = null)
		{
			id = id ?? Guid.NewGuid ().ToString ();
			int tex = generateTexture ();
			return loadImage (tex, bitmap, id, config, null);
		}

		public IImage LoadImage(string path, ILoadImageConfig config = null)
		{
			return LoadImageInner (path, config);
		}

		public async Task<IImage> LoadImageAsync (string filePath, ILoadImageConfig config = null)
		{
			return await Task.Run(() => LoadImage (filePath, config));
		}

		private IAnimation createLeftRightAnimation(IAnimation animation)
		{
			foreach (var frame in animation.Frames)
			{
				frame.Sprite.Anchor = new AGSPoint (0.5f, 0f);
			}
			IAnimation clone = animation.Clone();
			clone.FlipHorizontally();
			return clone;
		}
			
		private IAnimation loadAnimationFromResources(List<IResource> resources, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			AGSAnimationState state = new AGSAnimationState ();
			AGSAnimation animation = new AGSAnimation (animationConfig, state, resources.Count);

			foreach (IResource resource in resources) 
			{
				var image = loadImage (resource, loadConfig);
				if (image == null) continue;
				ISprite sprite = GetSprite();
				sprite.Image = image;
				AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = delay };
				animation.Frames.Add (frame);
			}
			animation.Setup ();
			return animation;
		}
			
		private GLImage loadImage(IResource resource, ILoadImageConfig config = null)
		{
			int tex = generateTexture ();
			try
			{
				IBitmap bitmap = new AGSBitmap(new Bitmap (resource.Stream));
				return loadImage (tex, bitmap, resource.ID, config, null);
			}
			catch (ArgumentException e)
			{
				Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e.ToString());
				return null;
			}
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
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);

			return tex;
		}
			
		private IBitmap crop(Bitmap bmp, Rectangle cropRect)
		{
			return new AGSBitmap(bmp.Clone (cropRect, bmp.PixelFormat)); //todo: improve performance by using FastBitmap
		}

		private GLImage loadImage(int texture, IBitmap bitmap, string id, ILoadImageConfig config, ISpriteSheet spriteSheet)
		{
			manipulateImage(bitmap, config);
			bitmap.LoadTexture(null);
			GLImage image = new GLImage (bitmap, id, texture, spriteSheet, config);

			if (_textures != null)
				_textures.GetOrAdd (image.ID, () => image);
			return image;
		}

		private void manipulateImage(IBitmap bitmap, ILoadImageConfig config)
		{
			if (config == null) return;
			if (config.TransparentColorSamplePoint != null)
			{
				IColor transparentColor = bitmap.GetPixel((int)config.TransparentColorSamplePoint.X,
					(int)config.TransparentColorSamplePoint.Y);
				bitmap.MakeTransparent(transparentColor);
			}
		}
	}
}

