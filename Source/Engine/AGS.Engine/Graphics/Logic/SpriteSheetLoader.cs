using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class SpriteSheetLoader
	{
		private readonly IResourceLoader _resources;
		private readonly IBitmapLoader _bitmapLoader;
		private readonly Action<GLImage, AGSAnimation, int> _addAnimationFrame;
		private readonly Func<int, IBitmap, string, ILoadImageConfig, ISpriteSheet, GLImage> _loadImage;

		public SpriteSheetLoader (IResourceLoader resources, IBitmapLoader bitmapLoader, 
		                          Action<GLImage, AGSAnimation, int> addAnimationFrame,
		                         Func<int, IBitmap, string, ILoadImageConfig, ISpriteSheet, GLImage> loadImage)
		{
			_resources = resources;
			_bitmapLoader = bitmapLoader;
			_addAnimationFrame = addAnimationFrame;
			_loadImage = loadImage;
		}

		public IAnimation LoadAnimationFromSpriteSheet(ISpriteSheet spriteSheet,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			string filePath = spriteSheet.Path;
			IResource resource = _resources.LoadResource (filePath);
			if (resource == null) {
				throw new InvalidOperationException ("Failed to load sprite sheet from " + filePath);
			}
			IBitmap bitmap = _bitmapLoader.Load (resource.Stream);
			int cellsInRow, cellsInCol, cellsTotal, cellX, cellY, cellsToGrab, cellsGrabbed = 0;
			Point mainStep, secondStep;
			AGSAnimation animation;
			getSpriteSheetData (bitmap, spriteSheet, animationConfig, out cellsInRow, out cellsInCol,
			                    out cellsTotal, out cellX, out cellY, out cellsToGrab, out mainStep, out secondStep, 
			                    out animation);
			for (int currentCell = 0; cellsGrabbed < cellsToGrab; currentCell++) 
			{
				if (currentCell >= spriteSheet.StartFromCell) 
				{
					Rectangle rect; IBitmap clone; string path; int tex;
					getImageInfo (bitmap, cellX, cellY, spriteSheet, filePath, out rect, out clone, out path, out tex);
					GLImage image = _loadImage (tex, clone, path, loadConfig, spriteSheet);
					_addAnimationFrame (image, animation, delay);
					cellsGrabbed++;
				}

				nextCell (mainStep, secondStep, cellsInRow, cellsInCol, ref cellX, ref cellY);
			}
			animation.Setup ();
			return animation;
		}

		public async Task<IAnimation> LoadAnimationFromSpriteSheetAsync(ISpriteSheet spriteSheet,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			string filePath = spriteSheet.Path;
			IResource resource = await Task.Run(() =>_resources.LoadResource (filePath));
			if (resource == null) {
				throw new InvalidOperationException ("Failed to load sprite sheet from " + filePath);
			}
			IBitmap bitmap = await Task.Run(() => _bitmapLoader.Load (resource.Stream));
			int cellsInRow, cellsInCol, cellsTotal, cellX, cellY, cellsToGrab, cellsGrabbed = 0;
			Point mainStep, secondStep;
			AGSAnimation animation;
			getSpriteSheetData (bitmap, spriteSheet, animationConfig, out cellsInRow, out cellsInCol,
								out cellsTotal, out cellX, out cellY, out cellsToGrab, out mainStep, out secondStep,
								out animation);
			for (int currentCell = 0; cellsGrabbed < cellsToGrab; currentCell++) 
			{
				if (currentCell >= spriteSheet.StartFromCell) 
				{
					Rectangle rect; IBitmap clone; string path; int tex;
					getImageInfo (bitmap, cellX, cellY, spriteSheet, filePath, out rect, out clone, out path, out tex);
					GLImage image = _loadImage (tex, clone, path, loadConfig, spriteSheet);
					_addAnimationFrame (image, animation, delay);
					cellsGrabbed++;
				}

				nextCell (mainStep, secondStep, cellsInRow, cellsInCol, ref cellX, ref cellY);
			}
			animation.Setup ();
			return animation;
		}

		private void getSpriteSheetData (IBitmap bitmap, ISpriteSheet spriteSheet, IAnimationConfiguration animationConfig,
		                                out int cellsInRow, out int cellsInCol, out int cellsTotal, out int cellX,
		                                 out int cellY, out int cellsToGrab, out Point mainStep, out Point secondStep, 
		                                 out AGSAnimation animation)
		{
			cellsInRow = bitmap.Width / spriteSheet.CellWidth;
			cellsInCol = bitmap.Height / spriteSheet.CellHeight;
			cellsTotal = cellsInRow * cellsInCol;

			int startRow, startCol;
			getOrder (spriteSheet.Order, cellsInRow, cellsInCol, out startRow, out startCol, out mainStep, out secondStep);

			cellX = startCol;
			cellY = startRow;
			cellsToGrab = spriteSheet.CellsToGrab < 0 ? cellsTotal : Math.Min (spriteSheet.CellsToGrab, cellsTotal);
			animation = new AGSAnimation (animationConfig, new AGSAnimationState (), cellsToGrab);
		}

		private void getImageInfo(IBitmap bitmap, int cellX, int cellY, ISpriteSheet spriteSheet, string filePath, 
		                          out Rectangle rect, out IBitmap clone, out string path, out int texture)
		{
			texture = GLImage.CreateTexture ();
			rect = new Rectangle (cellX * spriteSheet.CellWidth,
										cellY * spriteSheet.CellHeight, spriteSheet.CellWidth, spriteSheet.CellHeight);
			clone = bitmap.Crop (rect);
			path = string.Format ("{0}_{1}_{2}", rect.X, rect.Y, filePath);
		}

		private void nextCell (Point mainStep, Point secondStep, int cellsInRow, int cellsInCol, ref int cellX, ref int cellY)
		{
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

		private void getOrder (SpriteSheetOrder order, int cellsInRow, int cellsInCol, out int startRow, out int startCol,
			out Point mainStep, out Point secondStep)
		{
			switch (order) 
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
					throw new NotSupportedException (string.Format ("Sprite sheet order {0} is not supported", order));
			}
		}
	}
}

