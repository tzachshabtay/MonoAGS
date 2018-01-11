using System;
using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class FPSCounter
	{
		private ILabel _label;
		private IGame _game;

		private int _numRenderSamples, _numUpdateSamples;

		private Stopwatch _stopwatch;

		public FPSCounter(IGame game, ILabel label)
		{
			_label = label;
			_game = game;
		}

		public void Start()
		{
            _stopwatch = new Stopwatch();
            _stopwatch.Restart();
            _game.Events.OnBeforeRender.Subscribe(onRenderTick);
            _game.Events.OnRepeatedlyExecute.Subscribe(onUpdateTick);
			_game.Events.OnSavedGameLoad.Subscribe(() => onSaveGameLoaded());
		}

		private void onSaveGameLoaded()
		{
			_label = _game.Find<ILabel>(_label.ID);
		}

        private void onRenderTick()
		{
            _numRenderSamples++;
		}

        private void onUpdateTick()
        {
            _numUpdateSamples++;
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                _label.Text = $"FPS: {_numRenderSamples}/{_numUpdateSamples}";
                _numRenderSamples = 0;
                _numUpdateSamples = 0;
                _stopwatch.Restart();
            }
        }
	}
}

