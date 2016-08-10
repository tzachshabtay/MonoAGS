using System;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public class ALListener : IAudioListener
	{
		private float _volume;
		private ILocation _location;
		private IAudioErrors _errors;

		public ALListener(IAudioErrors errors)
		{
			_errors = errors;
			Volume = 0.5f;
			_location = new AGSLocation ();
		}

		public float Volume
		{
			get { return _volume; }
			set 
			{
				_volume = value;
				AL.Listener(ALListenerf.Gain, value);
				_errors.HasErrors();
			}
		}

		public ILocation Location
		{
			get { return _location; }
			set
			{
				_location = value;
				AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
				_errors.HasErrors();
			}
		}
	}
}

