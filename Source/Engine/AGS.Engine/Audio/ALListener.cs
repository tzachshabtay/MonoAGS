using AGS.API;

namespace AGS.Engine
{
	public class ALListener : IAudioListener
	{
		private float _volume;
		private ILocation _location;
		private IAudioErrors _errors;
        private IAudioBackend _backend;

        public ALListener(IAudioErrors errors, IAudioBackend backend)
		{
			_errors = errors;
            _backend = backend;
			Volume = 0.5f;
			_location = new AGSLocation ();
		}

		public float Volume
		{
            get => _volume;
            set 
			{
				_volume = value;
                _backend.ListenerSetGain(value);
				_errors.HasErrors();
			}
		}

		public ILocation Location
		{
            get => _location;
            set
			{
				_location = value;
                _backend.ListenerSetPosition(value.X, value.Y, value.Z);
				_errors.HasErrors();
			}
		}
	}
}

