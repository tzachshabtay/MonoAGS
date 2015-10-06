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
			//todo: need to account for alignment
			SizeF size = getSize(text, labelSize, config);

			float x = _obj.BoundingBox.MaxX;
			x = MathUtils.Clamp(x, 0f, Math.Max(0f, _game.VirtualResolution.Width - size.Width - 10f));

			float y = _obj.BoundingBox.MaxY;
			y = MathUtils.Clamp(y, 0f, Math.Min(_game.VirtualResolution.Height,
				_game.VirtualResolution.Height - size.Height));

			return new AGSPoint (x, y);
		}

		#endregion

		private SizeF getSize(string text, SizeF labelSize, ITextConfig config)
		{
			switch (config.AutoFit)
			{
				case AutoFit.TextShouldFitLabel:
					return labelSize;
				case AutoFit.TextShouldWrap:
					return text.Measure(config.Font, (int)labelSize.Width);
				default:
					return text.Measure(config.Font);
			}
		}
	}
}

