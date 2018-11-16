using System;
using AGS.API;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(Browsable = false)]
	public class ALAudioClip : IAudioClip
	{
		private ISoundData _soundData;
		private Lazy<int> _buffer;
		private IALAudioSystem _system;
		private IAudioErrors _errors;
        private AGSConcurrentHashSet<ISound> _playingSounds;
        private IAudioBackend _backend;

        public ALAudioClip(string id, ISoundData soundData, IALAudioSystem system, IAudioErrors errors, IAudioBackend backend,
                           IBlockingEvent<ISound> onSoundStarted, IBlockingEvent<ISound> onSoundCompleted)
		{
            OnSoundStarted = onSoundStarted;
            OnSoundCompleted = onSoundCompleted;
			_soundData = soundData;
			_errors = errors;
            _backend = backend;
			ID = id;
			_buffer = new Lazy<int> (() => generateBuffer());
            Duration = getDuration(soundData.DataLength, soundData.Channels, soundData.BitsPerSample, soundData.SampleRate);
            _playingSounds = new AGSConcurrentHashSet<ISound>();
            Tags = new AGSConcurrentHashSet<string>();
			_system = system;
			Volume = 1f;
			Pitch = 1f;
		}

        public float Duration { get; private set; }

		#region ISound implementation

		public ISound Play(bool shouldLoop = false, ISoundProperties properties = null)
		{
			properties = properties ?? this;
			return playSound(properties.Volume, properties.Pitch, properties.Panning, properties.Tags, shouldLoop);
		}

		public ISound Play(float volume, bool shouldLoop = false)
		{
			return playSound(volume, Pitch, Panning, Tags, shouldLoop);
		}

		public void PlayAndWait(ISoundProperties properties = null)
		{
			properties = properties ?? this;
            ISound sound = playSound(properties.Volume, properties.Pitch, properties.Panning, properties.Tags);
			Task.Run(async () => await sound.Completed).Wait();
		}

		public void PlayAndWait(float volume)
		{
			ISound sound = playSound(volume, Pitch, Panning, Tags);
			Task.Run(async () => await sound.Completed).Wait();
		}

		public string ID { get; }

		public float Volume { get; set; }
		public float Pitch { get; set; }
		public float Panning { get; set; }

        public bool IsPlaying => _playingSounds.Count > 0;
        public ReadOnlyCollection<ISound> CurrentlyPlayingSounds => _playingSounds.ToList().AsReadOnly();

        public IConcurrentHashSet<string> Tags { get; }

        public IBlockingEvent<ISound> OnSoundStarted { get; }

        public IBlockingEvent<ISound> OnSoundCompleted { get; }

        public override string ToString() => ID;

        #endregion

        private ISound playSound(float volume, float pitch, float panning, IConcurrentHashSet<string> tags, bool looping = false)
		{
            //Debug.WriteLine("Playing Sound: " + ID);
			int source = getSource();
            ALSound sound = new ALSound (source, Duration, volume, pitch, looping, panning, tags, _errors, _backend);
            _playingSounds.Add(sound);
            OnSoundStarted.Invoke(sound);
			sound.Play(_buffer.Value);
            sound.Completed.ContinueWith(_ =>
            {
                _system.ReleaseSource(source);
                _playingSounds.Remove(sound);
                OnSoundCompleted.Invoke(sound);
            });
			return sound;
		}

        private int getSource() => _system.AcquireSource();

        private float getDuration(int dataLength, int channels, int bitsPerSample, int sampleRate)
        {
            if (channels == 0 || bitsPerSample == 0 || sampleRate == 0)
                return 0f;
            return (((float)dataLength / channels) * 8f / bitsPerSample) / sampleRate;
        }

        private int generateBuffer()
		{
			_errors.HasErrors();

            int buffer = _backend.GenBuffer();
			Type dataType = _soundData.Data.GetType();
			if (dataType == typeof(byte[]))
			{
				byte[] bytes = (byte[])_soundData.Data;
                _backend.BufferData(buffer, getSoundFormat(_soundData.Channels, _soundData.BitsPerSample),
					bytes, _soundData.DataLength, _soundData.SampleRate);
			}
			else if (dataType == typeof(short[]))
			{
				short[] shorts = (short[])_soundData.Data;
                _backend.BufferData(buffer, getSoundFormat(_soundData.Channels, _soundData.BitsPerSample),
					shorts, _soundData.DataLength, _soundData.SampleRate);
			}
			else throw new NotSupportedException ("ALSound: Data type not supported: " + dataType.Name);

			_errors.HasErrors();
			return buffer;
		}

        private SoundFormat getSoundFormat(int channels, int bits)
		{
			switch (channels)

			{
				case 1: return bits == 8 ? SoundFormat.Mono8 : SoundFormat.Mono16;
				case 2: return bits == 8 ? SoundFormat.Stereo8 : SoundFormat.Stereo16;
				default: throw new NotSupportedException("The specified sound format is not supported.");
			}
		}
	}
}
