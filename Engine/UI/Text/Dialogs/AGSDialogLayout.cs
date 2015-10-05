using System;
using AGS.API;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSDialogLayout : IDialogLayout
	{
		private IGame _game;

		public AGSDialogLayout(IGame game)
		{
			_game = game;
		}

		#region IDialogLayout implementation

		public Task LayoutAsync(IObject dialogGraphics, IList<IDialogOption> options)
		{
			float y = 0f;
			for (int index = options.Count - 1; index >= 0; index--)
			{
				IDialogOption option = options[index];
				if (!option.Label.Visible) continue;
				option.Label.Y = y;

				y += option.Label.TextHeight;
			}
			if (dialogGraphics.Image == null)
			{
				dialogGraphics.Image = new EmptyImage (_game.VirtualResolution.Width, y);
			}

			dialogGraphics.Animation.Sprite.ScaleTo(_game.VirtualResolution.Width, y);
			return Task.FromResult(true);
		}

		#endregion
	}
}

