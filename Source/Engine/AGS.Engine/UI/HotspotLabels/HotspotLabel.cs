using System;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class HotspotLabel
	{
		private readonly IGameEvents _events;
		private ILabel _label;
		private IGameState _state;
		private readonly IGame _game;

		public HotspotLabel(IGame game, ILabel label)
		{
			_label = label;
			_events = game.Events;
			_state = game.State;
			_game = game;
		}

        /// <summary>
        /// Activating debug mode will show all objects under the mouse, not just the hotspots.
        /// </summary>
        /// <value><c>true</c> if debug mode; otherwise, <c>false</c>.</value>
        public bool DebugMode { get; set; }

		public void Start()
		{
			_events.OnRepeatedlyExecute.Subscribe(onTick);
			_events.OnSavedGameLoad.Subscribe(() => onSavedGameLoaded());
		}

		private void onSavedGameLoaded()
		{
			ILabel oldLabel = _label;
			_state = _game.State;
			_label = _game.Find<ILabel>(oldLabel.ID);
		}

		private void onTick()
		{
            if (_label == null || _state.Room == null) return;
            IObject obj = _state.Room.GetObjectAtMousePosition();
            if (obj == null || (obj.Hotspot == null && !DebugMode)) 
			{
				_label.Visible = false;
				return;
			}
            _label.Text = obj.Hotspot ?? obj.ID ?? "???";
			_label.Visible = true;
		}
	}
}

