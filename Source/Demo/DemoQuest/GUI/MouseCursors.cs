using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class MouseCursors
	{
		private const string _baseFolder = "Gui/Cursors/";
		private readonly AGSLoadImageConfig _loadConfig;

		public MouseCursors()
		{
            _loadConfig = new AGSLoadImageConfig(new AGS.API.Point(0, 12));
		}

		public IObject Point { get; private set; }
		public IObject Walk { get; private set; }
		public IObject Look { get; private set; }
		public IObject Talk { get; private set; }
		public IObject Interact { get; private set; }
		public IObject Wait { get; private set; }

		public RotatingCursorScheme Scheme { get; private set; }

		public const string TALK_MODE = "Talk";
		public const string POINT_MODE = "Point";

		public async Task LoadAsync(IGame game)
		{
			var factory = game.Factory;

			Point = await loadCursor("point.bmp", factory);
			Walk = await loadCursor("walk.bmp", factory);
			Look = await loadCursor("eye.bmp", factory);
			Talk = await loadCursor("talk.bmp", factory);
			Interact = await loadCursor("hand.bmp", factory);
			Wait = await loadCursor("wait.bmp", factory);

			Scheme = new RotatingCursorScheme (game, Look, Walk, Interact, Wait);
			Scheme.AddCursor(TALK_MODE, Talk, true);
			Scheme.AddCursor(POINT_MODE, Point, false);
			Scheme.Start();
		}

		private async Task<IObject> loadCursor(string filename, IGameFactory factory)
		{
			IAnimation animation = await factory.Graphics.LoadAnimationFromFilesAsync(loadConfig: _loadConfig, files: new[]{ _baseFolder + filename });
            var cursor = factory.Object.GetObject($"Cursor_{filename}");
            cursor.Pivot = new PointF(0f, 1f);
            cursor.IgnoreScalingArea = true;
            cursor.IgnoreViewport = true;
            cursor.StartAnimation(animation);
			return cursor;
		}
	}
}

