using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class ALSound : ISound
	{
		private readonly int _source;
		private float _volume, _realVolume;
		private float _pitch, _realPitch;
		private float _panning, _realPanning;
		private TaskCompletionSource<object> _tcs;
		private IAudioErrors _errors;
        private IAudioBackend _backend;
        private readonly Action modifierChangeCallback;
        private readonly Action<AGSListChangedEventArgs<ISoundModifier>> modifiersChangedCallback;

        public ALSound(int source, float duration, float volume, float pitch, bool isLooping, float panning,
            IConcurrentHashSet<string> tags, IAudioErrors errors, IAudioBackend backend)
		{
            //Using delegates to avoid heap allocations
            modifierChangeCallback = onModifierChanged;
            modifiersChangedCallback = onModifiersChanged;

            SoundModifiers = new AGSBindingList<ISoundModifier>(3);
            SoundModifiers.OnListChanged.Subscribe(modifiersChangedCallback);
            Tags = tags;
			_tcs = new TaskCompletionSource<object> (null);
            _backend = backend;
			_source = source;
			_volume = volume;
            Duration = duration;
			_pitch = pitch;
			_panning = panning;
			IsLooping = isLooping;
			_errors = errors;
		}

		public async void Play(int buffer)
		{
            _backend.SourceSetBuffer(_source, buffer);
			Volume = _volume;
			Pitch = _pitch;
			Panning = _panning;
            _backend.SourceSetLooping(_source, IsLooping);
            _backend.SourcePlay(_source);
			_errors.HasErrors();

			if (IsLooping) return;
			// Query the source to find out when it stops playing.
			do
			{
				await Task.Delay(100);
			}
            while (_backend.SourceIsPlaying(_source));
			_tcs.TrySetResult(null);
		}

        #region ISound implementation

        public bool IsValid => _backend.IsValid;

        public void Pause()
		{
			if (HasCompleted) return;
			IsPaused = true;
            _backend.SourcePause(_source);
			_errors.HasErrors();
		}

		public void Resume()
		{
			if (HasCompleted || !IsPaused) return;
			_backend.SourcePlay(_source);
			_errors.HasErrors();
			IsPaused = false;
		}

		public void Rewind()
		{
			if (HasCompleted) return;
			_backend.SourceRewind(_source);
			_errors.HasErrors();
		}

		public void Stop()
		{
			if (HasCompleted) return;
			_backend.SourceStop(_source);
			_errors.HasErrors();
			_tcs.TrySetResult(null);
		}

        public int SourceID => _source;

        public bool IsPaused { get; private set; }

		public bool IsLooping { get; }

        public bool HasCompleted => _tcs.Task.IsCompleted;

        public Task Completed => _tcs.Task;

        public IAGSBindingList<ISoundModifier> SoundModifiers { get; }

        public IConcurrentHashSet<string> Tags { get; }

        #endregion

        public float RealVolume
		{
            get => _realVolume;
            private set
			{
                _realVolume = value;
                _backend.SourceSetGain(_source, _realVolume);
				_errors.HasErrors();
			}
		}

		public float RealPitch
		{
            get => _realPitch;
            private set
			{
                _realPitch = value;
                _backend.SourceSetPitch(_source, _realPitch);
				_errors.HasErrors();
			}
		}

        public float Duration { get; private set; }

		public float Position
		{
			get 
			{
                float seek = _backend.SourceGetSeek(_source);
				_errors.HasErrors();
				return seek;
			}
			set 
			{
				if (HasCompleted) return;
                _backend.SourceSetSeek(_source, value);
				_errors.HasErrors();
			}
		}

		public float RealPanning
		{
            get => _realPanning;
            private set 
			{
                _realPanning = value;
				//formula from: https://code.google.com/archive/p/libgdx/issues/1183
				float x = (float)Math.Cos((value - 1) * Math.PI / 2);
				float z = (float)Math.Sin((value + 1) * Math.PI / 2);
                _backend.SourceSetPosition(_source, x, 0f, z);
                _errors.HasErrors();
			}
		}

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                setVolume();
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                setPitch();
            }
        }

        public float Panning
        {
            get => _panning;
            set
            {
                _panning = value;
                setPanning();
            }
        }

        private void onModifiersChanged(AGSListChangedEventArgs<ISoundModifier> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var modifier in args.Items) modifier.Item.OnChange.Subscribe(modifierChangeCallback);
            }
            else
            {
                foreach (var modifier in args.Items) modifier.Item.OnChange.Unsubscribe(modifierChangeCallback);
            }
            onModifierChanged();
        }

        private void onModifierChanged()
        {
            setVolume();
            setPitch();
            setPanning();
        }

        private void setVolume()
        {
            float volume = _volume;
            foreach (var modifier in SoundModifiers)
            {
                volume = modifier.GetVolume(volume);
            }
            RealVolume = volume;
        }

        private void setPitch()
        {
            float pitch = _pitch;
            foreach (var modifier in SoundModifiers)
            {
                pitch = modifier.GetPitch(pitch);
            }
            RealPitch = pitch;
        }

        private void setPanning()
        {
            float panning = _panning;
            foreach (var modifier in SoundModifiers)
            {
                panning = modifier.GetPanning(panning);
            }
            RealPanning = panning;
        }
	}
}