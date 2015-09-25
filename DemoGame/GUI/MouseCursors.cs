using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

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
				TransparentColorSamplePoint = new Point (0, 12) 
			};
		}

		public IAnimationContainer Point { get; private set; }
		public IAnimationContainer Walk { get; private set; }
		public IAnimationContainer Look { get; private set; }
		public IAnimationContainer Talk { get; private set; }
		public IAnimationContainer Interact { get; private set; }
		public IAnimationContainer Wait { get; private set; }

		public const string TALK_MODE = "Talk";
		public const string POINT_MODE = "Point";

		public void Load(IGame game)
		{
			var factory = game.Factory.Graphics;

			Point = loadCursor("point.bmp", factory);
			Walk = loadCursor("walk.bmp", factory);
			Look = loadCursor("eye.bmp", factory);
			Talk = loadCursor("talk.bmp", factory);
			Interact = loadCursor("hand.bmp", factory);
			Wait = loadCursor("wait.bmp", factory);

			game.Input.Cursor = new AGSAnimationContainer(game.Factory.Graphics.GetSprite(), game.Factory.Graphics);

			RotatingCursorScheme scheme = new RotatingCursorScheme (game, Look, Walk, Interact, Wait);
			scheme.AddCursor(TALK_MODE, Talk, true);
			scheme.AddCursor(POINT_MODE, Point, false);
			scheme.Start();
		}

		private IAnimationContainer loadCursor(string filename, IGraphicsFactory factory)
		{
			IAnimation animation = factory.LoadAnimationFromFiles(loadConfig: _loadConfig, files: new[]{ _baseFolder + filename });
			AGSAnimationContainer cursor = new AGSAnimationContainer (factory.GetSprite(), factory);
			cursor.StartAnimation(animation);
			return cursor;
		}
	}
}

