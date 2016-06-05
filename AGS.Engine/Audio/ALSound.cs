using System;
using AGS.API;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;

namespace AGS.Engine
{
	public class ALSound : ISound
	{
		private readonly int _source;
		private float _volume;
		private float _pitch;
		private TaskCompletionSource<object> _tcs;

		public ALSound(int source, float volume, float pitch, bool isLooping)
		{
			_tcs = new TaskCompletionSource<object> (null);
			_source = source;
			_volume = volume;
			_pitch = pitch;
			IsLooping = isLooping;
		}

		public async void Play(int buffer)
		{
			AL.Source(_source, ALSourcei.Buffer, buffer);
			Volume = _volume;
			Pitch = _pitch;
			AL.Source(_source, ALSourceb.Looping, IsLooping);
			AL.SourcePlay(_source);

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
		}

		public void Resume()
		{
			if (HasCompleted || !IsPaused) return;
			AL.SourcePlay(_source);
			IsPaused = false;
		}

		public void Rewind()
		{
			if (HasCompleted) return;
			AL.SourceRewind(_source);
		}

		public void Stop()
		{
			if (HasCompleted) return;
			_tcs.TrySetResult(null);
			AL.SourceStop(_source);
		}

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
			}
		}

		public float Pitch
		{
			get { return _pitch; }
			set
			{
				_pitch = value;
				AL.Source(_source, ALSourcef.Pitch, _pitch);
			}
		}

		public float Seek
		{
			get 
			{
				float seek;
				AL.GetSource(_source, ALSourcef.SecOffset, out seek); 
				return seek;
			}
			set 
			{
				if (HasCompleted) return;
				AL.Source(_source, ALSourcef.SecOffset, value);
			}
		}

		#endregion
	}
}

