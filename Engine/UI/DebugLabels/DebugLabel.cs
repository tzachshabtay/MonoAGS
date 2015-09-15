using System;
using AGS.API;

namespace AGS.Engine
{
	public class DebugLabel
	{
		private ILabel _label;
		private IGame _game;
		private Func<IGame, string> _getText;

		public DebugLabel(IGame game, Func<IGame, string> getText, ILabel label)
		{
			_game = game;
			_getText = getText;
			_label = label;
		}

		public void Start()
		{
			_game.Events.OnRepeatedlyExecute.Subscribe(onTick);
		}

		private void onTick(object sender, EventArgs args)
		{
			try
			{
				_label.Text = _getText(_game);
			}
			catch (Exception e)
			{
				_label.Text = e.Message;
			}
		}
	}
}

