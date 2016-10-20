using System;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class HotspotLabel
	{
		private readonly IGameEvents _events;
		private ILabel _label;
		private readonly IInput _input;
		private IGameState _state;
		private readonly IGame _game;

		public HotspotLabel(IGame game, ILabel label)
		{
			_label = label;
			_events = game.Events;
			_input = game.Input;
			_state = game.State;
			_game = game;
		}

		public void Start()
		{
			_events.OnRepeatedlyExecute.Subscribe(onTick);
			_events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
		}

		private void onSavedGameLoaded()
		{
			ILabel oldLabel = _label;
			_state = _game.State;
			_label = _game.Find<ILabel>(oldLabel.ID);
		}

		private void onTick(object sender, EventArgs args)
		{
			if (_label == null) return;
			PointF position = _input.MousePosition;
			IObject obj = _state.Room.GetObjectAt (position.X, position.Y);
			if (obj == null || obj.Hotspot == null) 
			{
				_label.Visible = false;
				return;
			}
			_label.Text = obj.Hotspot;
			_label.Visible = true;
		}
	}
}

