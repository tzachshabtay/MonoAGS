using AGS.API;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;

namespace AGS.Engine
{
    public class AGSSoundEmitter : ISoundEmitter
    {
        private readonly ConcurrentDictionary<int, EmittedSound> _playingSounds;
        private readonly IGame _game;

        public AGSSoundEmitter(IGame game)
        {
            _game = game;
            OnSoundStarted = new AGSEvent<ISound>();
            OnSoundCompleted = new AGSEvent<ISound>();
            _playingSounds = new ConcurrentDictionary<int, EmittedSound>();
            game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            AutoPan = true;
            AutoAdjustVolume = true;
        }

        #region ISoundPlayer implementation

        public ISound Play(bool shouldLoop = false, ISoundProperties properties = null)
        {
            ISound sound = AudioClip.Play(shouldLoop, properties);
            OnSoundStarted.Invoke(sound);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            sound.Completed.ContinueWith(_ => OnSoundCompleted?.Invoke(sound));
            return sound;
        }

        public ISound Play(float volume, bool shouldLoop = false)
        {
            ISound sound = AudioClip.Play(volume, shouldLoop);
            OnSoundStarted.Invoke(sound);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            sound.Completed.ContinueWith(_ => OnSoundCompleted?.Invoke(sound));
            return sound;
        }

        public void PlayAndWait(ISoundProperties properties = null)
        {
            ISound sound = AudioClip.Play(false, properties);
            OnSoundStarted.Invoke(sound);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            Task.Run(async () => await sound.Completed).Wait();
            OnSoundCompleted.Invoke(sound);
        }

        public void PlayAndWait(float volume)
        {
            ISound sound = AudioClip.Play(volume, false);
            OnSoundStarted.Invoke(sound);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            Task.Run(async () => await sound.Completed).Wait();
            OnSoundCompleted.Invoke(sound);
        }

        #endregion

        #region ISoundEmitter implementation

        public IAudioClip AudioClip { get; set; }

        public IObject Object
        {
            set { HasRoom = value; WorldPosition = value; EntityID = value == null ? null : value.ID; }
        }
        public string EntityID { get; set; }
        public IHasRoomComponent HasRoom { get; set; }
        public IWorldPositionComponent WorldPosition { get; set; }

		public bool AutoPan { get; set; }
		public bool AutoAdjustVolume { get; set; }

        public bool IsPlaying => _playingSounds.Count > 0;
        public ReadOnlyCollection<ISound> CurrentlyPlayingSounds => _playingSounds.Select(p => p.Value.Sound).ToList().AsReadOnly();

        public IBlockingEvent<ISound> OnSoundStarted { get; }

        public IBlockingEvent<ISound> OnSoundCompleted { get; }

        public void Assign(IDirectionalAnimation animation, params int[] frames)
		{
			Assign(animation.Down, frames);
			Assign(animation.Left, frames);
			Assign(animation.Right, frames);
			Assign(animation.Up, frames);
			Assign(animation.DownLeft, frames);
			Assign(animation.DownRight, frames);
			Assign(animation.UpLeft, frames);
			Assign(animation.UpRight, frames);
		}

		public void Assign(IAnimation animation, params int[] frames)
		{
			if (animation == null) return;
			foreach (int frame in frames)
			{
                if (frame >= animation.Frames.Count) continue;
				animation.Frames[frame].SoundEmitter = this;
			}
		}

		public void Assign(params IAnimationFrame[] frames)
		{
			foreach (var frame in frames)
			{
				frame.SoundEmitter = this;
			}
		}

		#endregion

		private void onRepeatedlyExecute()
		{
			if (_game.State.Paused) return;
            var pos = WorldPosition;
            var hasRoom = HasRoom;

			foreach (var sound in _playingSounds)
			{
				if (sound.Value.Sound.HasCompleted)
				{
					EmittedSound value;
					_playingSounds.TryRemove(sound.Key, out value);
					continue;
				}
				if (pos == null) continue;
				if (AutoPan)
				{
                    float pan = MathUtils.Lerp(0f, -1f, _game.Settings.VirtualResolution.Width, 1f, pos.WorldX);
					sound.Value.Panning = pan;
				}
				if (AutoAdjustVolume)
				{
                    if (hasRoom == null) continue;
                    var room = _game.State.Room;
                    if (room != hasRoom.Room) return;
                    foreach (var area in room.GetMatchingAreas(pos.WorldXY, EntityID))
					{
                        var scalingArea = area.GetComponent<IScalingArea>();
                        if (scalingArea == null || !scalingArea.ScaleVolume) continue;
                        float scale = scalingArea.GetScaling(pos.WorldY);
						sound.Value.Volume = scale;
					}
				}
			}
		}

		private class EmittedSound
		{
			private static int runningId;
            private VolumeModifier _volumeModifier;
            private PanningModifier _panningModifier;

			public EmittedSound(ISound sound)
			{
				Sound = sound;
				ID = runningId;
				runningId++;
                _volumeModifier = new VolumeModifier(1f);
                _panningModifier = new PanningModifier(1f);
                sound.SoundModifiers.Add(_volumeModifier);
                sound.SoundModifiers.Add(_panningModifier);
			}

			public ISound Sound { get; }
			public int ID { get; }
            public float Volume { set => _volumeModifier.VolumeFactor = value; }
            public float Panning { set => _panningModifier.PanningOffset = value; }
		}
	}
}