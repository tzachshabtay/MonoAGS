using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSSayLocation : ISayLocation
	{
		private IObject _obj;
		private IGame _game;

		public AGSSayLocation(IGame game, IObject obj)
		{
			_game = game;
			_obj = obj;
		}

		#region ISayLocation implementation

		public IPoint GetLocation(string text, SizeF labelSize, ITextConfig config)
		{
			//todo: need to account for alignment and auto-fit
			SizeF size = text.Measure(config.Font);

			float x = _obj.BoundingBox.MaxX;
			x = MathUtils.Clamp(x, 0f, _game.VirtualResolution.Width - size.Width);

			float y = _obj.BoundingBox.MaxY;
			y = MathUtils.Clamp(y, 0f, _game.VirtualResolution.Height - size.Height);

			return new AGSPoint (x, y);
		}

		#endregion
	}
}

