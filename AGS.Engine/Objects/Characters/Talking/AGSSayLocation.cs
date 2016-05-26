using System;
using AGS.API;


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

		public PointF GetLocation(string text, AGS.API.SizeF labelSize, ITextConfig config)
		{
			//todo: need to account for alignment
			AGS.API.SizeF size = config.GetTextSize(text, labelSize);

			float x = _obj.BoundingBox.MaxX;
			x = MathUtils.Clamp(x, 0f, Math.Max(0f, _game.VirtualResolution.Width - size.Width - 10f));

			float y = _obj.BoundingBox.MaxY;
			y = MathUtils.Clamp(y, 0f, Math.Min(_game.VirtualResolution.Height,
				_game.VirtualResolution.Height - size.Height));

			return new PointF (x, y);
		}

		#endregion
	}
}

