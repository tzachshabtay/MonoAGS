using System;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;

namespace AGS.Engine
{
	public class ALListener : IAudioListener
	{
		private float _volume;

		public ALListener()
		{
			Volume = 0.5f;
		}

		public float Volume
		{
			get { return _volume; }
			set 
			{
				_volume = value;
				AL.Listener(ALListenerf.Gain, value);
			}
		}
	}
}

