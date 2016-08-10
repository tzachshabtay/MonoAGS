using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class MouseCursors
	{
		private const string _baseFolder = "../../Assets/Gui/Cursors/";
		private readonly AGSLoadImageConfig _loadConfig;

		public MouseCursors()
		{
			_loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new AGS.API.Point (0, 12) 
			};
		}

		public IAnimationContainer Point { get; private set; }
		public IAnimationContainer Walk { get; private set; }
		public IAnimationContainer Look { get; private set; }
		public IAnimationContainer Talk { get; private set; }
		public IAnimationContainer Interact { get; private set; }
		public IAnimationContainer Wait { get; private set; }

		public RotatingCursorScheme Scheme { get; private set; }

		public const string TALK_MODE = "Talk";
		public const string POINT_MODE = "Point";

		public async Task LoadAsync(IGame game)
		{
			var factory = game.Factory.Graphics;

			Point = await loadCursor("point.bmp", factory);
			Walk = await loadCursor("walk.bmp", factory);
			Look = await loadCursor("eye.bmp", factory);
			Talk = await loadCursor("talk.bmp", factory);
			Interact = await loadCursor("hand.bmp", factory);
			Wait = await loadCursor("wait.bmp", factory);

			game.Input.Cursor = new AGSAnimationContainer(game.Factory.Graphics.GetSprite(), game.Factory.Graphics);

			Scheme = new RotatingCursorScheme (game, Look, Walk, Interact, Wait);
			Scheme.AddCursor(TALK_MODE, Talk, true);
			Scheme.AddCursor(POINT_MODE, Point, false);
			Scheme.Start();
		}

		private async Task<IAnimationContainer> loadCursor(string filename, IGraphicsFactory factory)
		{
			IAnimation animation = await factory.LoadAnimationFromFilesAsync(loadConfig: _loadConfig, files: new[]{ _baseFolder + filename });
			AGSAnimationContainer cursor = new AGSAnimationContainer (factory.GetSprite(), factory);
			cursor.StartAnimation(animation);
			return cursor;
		}
	}
}

