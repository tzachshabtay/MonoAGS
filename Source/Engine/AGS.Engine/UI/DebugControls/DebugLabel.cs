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
			_game.Events.OnSavedGameLoad.Subscribe(() => onSaveGameLoaded());
		}

		private void onSaveGameLoaded()
		{
			_label = _game.Find<ILabel>(_label.ID);
		}

		private void onTick()
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

