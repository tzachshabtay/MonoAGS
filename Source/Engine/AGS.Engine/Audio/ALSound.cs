using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class ALSound : ISound
	{
		private readonly int _source;
		private float _volume;
		private float _pitch;
		private float _panning;
		private TaskCompletionSource<object> _tcs;
		private IAudioErrors _errors;
        private IAudioBackend _backend;

        public ALSound(int source, float volume, float pitch, bool isLooping, float panning, IAudioErrors errors, IAudioBackend backend)
		{
			_tcs = new TaskCompletionSource<object> (null);
            _backend = backend;
			_source = source;
			_volume = volume;
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
		}

		public int SourceID { get { return _source; } }

		public bool IsPaused { get; private set; }

		public bool IsLooping { get; private set; }

		public bool HasCompleted 
		{ 
			get { return _tcs.Task.IsCompleted; }
		}

		public Task Completed { get { return _tcs.Task; } }

		#endregion

		#region ISoundProperties implementation

		public float Volume
		{
			get { return _volume; }
			set
			{
				_volume = value;
                _backend.SourceSetGain(_source, _volume);
				_errors.HasErrors();
			}
		}

		public float Pitch
		{
			get { return _pitch; }
			set
			{
				_pitch = value;
                _backend.SourceSetPitch(_source, _pitch);
				_errors.HasErrors();
			}
		}

		public float Seek
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

		public float Panning
		{
			get { return _panning; }
			set 
			{
				_panning = value;
				//formula from: https://code.google.com/archive/p/libgdx/issues/1183
				float x = (float)Math.Cos((value - 1) * Math.PI / 2);
				float z = (float)Math.Sin((value + 1) * Math.PI / 2);
                _backend.SourceSetPosition(_source, x, 0f, z);
                _errors.HasErrors();
			}
		}

		#endregion
	}
}

