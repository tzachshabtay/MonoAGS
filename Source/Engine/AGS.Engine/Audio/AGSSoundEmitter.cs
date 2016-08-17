using AGS.API;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AGS.Engine
{
    public class AGSSoundEmitter : ISoundEmitter
    {
        private readonly ConcurrentDictionary<int, EmittedSound> _playingSounds;
        private readonly IGame _game;

        public AGSSoundEmitter(IGame game)
        {
            _game = game;
            _playingSounds = new ConcurrentDictionary<int, EmittedSound>();
            game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            AutoPan = true;
            AutoAdjustVolume = true;
        }

        #region ISoundPlayer implementation

        public ISound Play(bool shouldLoop = false, ISoundProperties properties = null)
        {
            ISound sound = AudioClip.Play(shouldLoop, properties);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            return sound;
        }

        public ISound Play(float volume, bool shouldLoop = false)
        {
            ISound sound = AudioClip.Play(volume, shouldLoop);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            return sound;
        }

        public void PlayAndWait(ISoundProperties properties = null)
        {
            ISound sound = AudioClip.Play(false, properties);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            Task.Run(async () => await sound.Completed).Wait();
        }

        public void PlayAndWait(float volume)
        {
            ISound sound = AudioClip.Play(volume, false);
            EmittedSound emittedSound = new EmittedSound(sound);
            _playingSounds.TryAdd(emittedSound.ID, emittedSound);
            Task.Run(async () => await sound.Completed).Wait();
        }

        #endregion

        #region ISoundEmitter implementation

        public IAudioClip AudioClip { get; set; }

        public IObject Object 
        { 
            set { Transform = value; HasRoom = value; }
        }
        public ITransform Transform { get; set; }
        public IHasRoom HasRoom { get; set; }

		public bool AutoPan { get; set; }
		public bool AutoAdjustVolume { get; set; }

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

		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
			if (_game.State.Paused) return;
            var obj = Transform;
            var hasRoom = HasRoom;

			foreach (var sound in _playingSounds)
			{
				if (sound.Value.Sound.HasCompleted)
				{
					EmittedSound value;
					_playingSounds.TryRemove(sound.Key, out value);
					continue;
				}
				if (obj == null) continue;
				if (AutoPan)
				{
					float pan = MathUtils.Lerp(0f, -1f, _game.VirtualResolution.Width, 1f, obj.Location.X);
					sound.Value.Sound.Panning = pan;
				}
				if (AutoAdjustVolume)
				{
                    if (hasRoom == null) continue;
					var room = _game.State.Player.Character.Room;
                    if (room != hasRoom.Room) return;
					foreach (var area in room.ScalingAreas)
					{
						if (!area.Enabled || !area.ScaleObjects || !area.IsInArea(obj.Location.XY)) continue;
						float scale = area.GetScaling(obj.Y);
						sound.Value.Sound.Volume = AudioClip.Volume * scale;
					}
				}
			}
		}

		private class EmittedSound
		{
			private static int runningId;

			public EmittedSound(ISound sound)
			{
				Sound = sound;
				ID = runningId;
				runningId++;
			}

			public ISound Sound { get; private set; }
			public int ID { get; private set; }
		}
	}
}

