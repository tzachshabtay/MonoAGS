using System;
using AGS.API;

namespace AGS.Engine
{
	public class HotspotLabel
	{
		private IGameEvents _events;
		private ILabel _label;
		private IInput _input;
		private IGameState _state;

		public HotspotLabel(IGame game, ILabel label)
		{
			_label = label;
			_events = game.Events;
			_input = game.Input;
			_state = game.State;
		}

		public void Start()
		{
			_events.OnRepeatedlyExecute.Subscribe(onTick);
		}

		private void onTick(object sender, EventArgs args)
		{
			IPoint position = _input.MousePosition;
			IObject obj = _state.Player.Character.Room.GetHotspotAt (position.X, position.Y);
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

