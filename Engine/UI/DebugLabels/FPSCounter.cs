using System;
using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class FPSCounter
	{
		private ILabel _label;
		private IGameEvents _events;

		private int _numSamples;

		private Stopwatch _stopwatch;

		public FPSCounter(IGameEvents events, ILabel label)
		{
			_label = label;
			_events = events;
		}

		public void Start()
		{
			_stopwatch = new Stopwatch ();
			_stopwatch.Restart();
			_events.OnRepeatedlyExecute.Subscribe(onTick);
		}

		private void onTick(object sender, EventArgs e)
		{
			_numSamples++;
			if (_stopwatch.ElapsedMilliseconds > 1000)
			{
				_label.Text = "FPS: " + _numSamples;
				_numSamples = 0;
				_stopwatch.Restart();
			}
		}
	}
}

