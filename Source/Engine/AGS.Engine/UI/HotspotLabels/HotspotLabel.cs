using System;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class HotspotLabel
	{
		private readonly IGameEvents _events;
		private ILabel _label;
		private readonly IGame _game;

		public HotspotLabel(IGame game, ILabel label)
		{
			_label = label;
			_events = game.Events;
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
			_label = _game.Find<ILabel>(oldLabel.ID);
		}

		private void onTick()
		{
            if (_label == null) return;
            IObject obj = _game.HitTest.ObjectAtMousePosition;
            IHotspotComponent hotspot = obj?.GetComponent<IHotspotComponent>();
            string hotspotName = hotspot != null ? obj.DisplayName : null;
            if (obj == null || (hotspotName == null && !DebugMode))
			{
				_label.Visible = false;
				return;
			}
            _label.Text = hotspotName ?? obj.ID ?? "???";
			_label.Visible = true;
		}
	}
}

