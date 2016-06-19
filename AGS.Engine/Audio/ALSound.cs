using System;
using AGS.API;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;
using OpenTK;

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

		public ALSound(int source, float volume, float pitch, bool isLooping, float panning, IAudioErrors errors)
		{
			_tcs = new TaskCompletionSource<object> (null);
			_source = source;
			_volume = volume;
			_pitch = pitch;
			_panning = panning;
			IsLooping = isLooping;
			_errors = errors;
		}

		public async void Play(int buffer)
		{
			AL.Source(_source, ALSourcei.Buffer, buffer);
			Volume = _volume;
			Pitch = _pitch;
			Panning = _panning;
			AL.Source(_source, ALSourceb.Looping, IsLooping);
			AL.SourcePlay(_source);
			_errors.HasErrors();

			if (IsLooping) return;
			int state;
			// Query the source to find out when it stops playing.
			do
			{
				await Task.Delay(100);
				AL.GetSource(_source, ALGetSourcei.SourceState, out state);
			}
			while ((ALSourceState)state == ALSourceState.Playing);
			_tcs.TrySetResult(null);
		}

		#region ISound implementation

		public void Pause()
		{
			if (HasCompleted) return;
			IsPaused = true;
			AL.SourcePause(_source);
			_errors.HasErrors();
		}

		public void Resume()
		{
			if (HasCompleted || !IsPaused) return;
			AL.SourcePlay(_source);
			_errors.HasErrors();
			IsPaused = false;
		}

		public void Rewind()
		{
			if (HasCompleted) return;
			AL.SourceRewind(_source);
			_errors.HasErrors();
		}

		public void Stop()
		{
			if (HasCompleted) return;
			AL.SourceStop(_source);
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
				AL.Source(_source, ALSourcef.Gain, _volume);
				_errors.HasErrors();
			}
		}

		public float Pitch
		{
			get { return _pitch; }
			set
			{
				_pitch = value;
				AL.Source(_source, ALSourcef.Pitch, _pitch);
				_errors.HasErrors();
			}
		}

		public float Seek
		{
			get 
			{
				float seek;
				AL.GetSource(_source, ALSourcef.SecOffset, out seek);
				_errors.HasErrors();
				return seek;
			}
			set 
			{
				if (HasCompleted) return;
				AL.Source(_source, ALSourcef.SecOffset, value);
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
				AL.Source(_source, ALSource3f.Position, x, 0f, z);
                _errors.HasErrors();
			}
		}

		#endregion
	}
}

