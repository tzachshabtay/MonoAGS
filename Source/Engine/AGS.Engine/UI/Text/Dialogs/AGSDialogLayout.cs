using System;
using AGS.API;
using System.Collections.Generic;
using System.Threading.Tasks;


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

		public async Task LayoutAsync(IObject dialogGraphics, IList<IDialogOption> options)
		{
			float y = 0f;
			for (int index = options.Count - 1; index >= 0; index--)
			{
				IDialogOption option = options[index];
                _game.State.UI.Add(option.Label);
				if (!option.Label.Visible) continue;
				option.Label.Y = y;

                int retries = 100;
                while (option.Label.TextHeight <= 5f && retries > 0)
                {
                    await Task.Delay(1); //todo: find a better way (we need to wait at least one render loop for the text height to be correct)
                    retries--;
                }
                y += option.Label.TextHeight;
			}
			if (dialogGraphics.Image == null)
			{
				dialogGraphics.Image = new EmptyImage (_game.Settings.VirtualResolution.Width, y);
			}

			dialogGraphics.Animation.Sprite.ScaleTo(_game.Settings.VirtualResolution.Width, y);
		}

		#endregion
	}
}

