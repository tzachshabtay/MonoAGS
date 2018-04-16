using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSSayComponent : AGSComponent, ISayComponent
	{
		private readonly IGameState _state;
		private readonly IGameFactory _factory;
		private readonly IInput _input;
		private readonly ISayLocationProvider _location;
		private readonly FastFingerChecker _fastFingerChecker;
		private IOutfitComponent _outfit;
		private IFaceDirectionComponent _faceDirection;
        private readonly ISoundEmitter _emitter;
        private readonly ISpeechCache _speechCache;
        private string _characterName;

		public AGSSayComponent(IGameState state, IGameFactory factory, IInput input, ISayLocationProvider location,
			                  FastFingerChecker fastFingerChecker, ISayConfig sayConfig,
                              IBlockingEvent<BeforeSayEventArgs> onBeforeSay, 
                              ISoundEmitter emitter, ISpeechCache speechCache)
		{
			_state = state;
			_factory = factory;
			_input = input;
			_location = location;
			_fastFingerChecker = fastFingerChecker;
            _emitter = emitter;
            _speechCache = speechCache;
			SpeechConfig = sayConfig;
			OnBeforeSay = onBeforeSay;
		}

		public ISayConfig SpeechConfig { get; private set; }
		public IBlockingEvent<BeforeSayEventArgs> OnBeforeSay { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _characterName = entity.ID;
            entity.Bind<IWorldPositionComponent>(c => _emitter.WorldPosition = c, _ => _emitter.WorldPosition = null);
            entity.Bind<IHasRoomComponent>(c => _emitter.HasRoom = c, _ => _emitter.HasRoom = null);
            entity.Bind<IFaceDirectionComponent>(c => _faceDirection = c, _ => _faceDirection = null);
            entity.Bind<IOutfitComponent>(c => _outfit = c, _ => _outfit = null);
        }

		public void Say(string text)
		{
			Task.Run(async () => await SayAsync(text)).Wait();
		}

		public async Task SayAsync(string text)
		{
			if (_state.Cutscene.IsSkipping)
			{
                if (_outfit != null) await setAnimation(_outfit.Outfit[AGSOutfit.Idle]);
				return;
			}
            if (_outfit != null) await setAnimation(_outfit.Outfit[AGSOutfit.Speak]);
			await Task.Delay(1);
            var speech = await _speechCache.GetSpeechLineAsync(_characterName, text);
            text = speech.Text;

            ISayLocation location = getLocation(text);
            IObject portrait = showPortrait(location);
			ILabel label = _factory.UI.GetLabel($"Say: {text}", text, SpeechConfig.LabelSize.Width, 
                SpeechConfig.LabelSize.Height, location.TextLocation.X, location.TextLocation.Y,
                config: SpeechConfig.TextConfig, addToUi: false);
			label.RenderLayer = AGSLayers.Speech;
			label.Border = SpeechConfig.Border;
			label.Tint = SpeechConfig.BackgroundColor;
			TaskCompletionSource<object> externalSkipToken = new TaskCompletionSource<object> (null);
			BeforeSayEventArgs args = new BeforeSayEventArgs (label, () => externalSkipToken.TrySetResult(null));
			OnBeforeSay.Invoke(args);
			label = args.Label;
            _state.UI.Add(label);
			ISound sound = null;
            if (speech.AudioClip != null)
            {
                _emitter.AudioClip = speech.AudioClip;
                sound = _emitter.Play();
            }

			await waitForText(text, externalSkipToken.Task, sound);
			_state.UI.Remove(label);
            label.Dispose();
            if (portrait != null) portrait.Visible = false;

            if (_outfit != null) await setAnimation(_outfit.Outfit[AGSOutfit.Idle]);
		}

        private IObject showPortrait(ISayLocation location)
        {
            if (location.PortraitLocation == null) return null;
            var portraitConfig = SpeechConfig.PortraitConfig;
            if (portraitConfig == null) return null;
            IObject portrait = portraitConfig.Portrait;
            if (portrait != null)
            {
                portrait.Visible = true;
                portrait.Location = new AGSLocation(location.PortraitLocation.Value);
            }
            return portrait;
        }

		private async Task setAnimation(IDirectionalAnimation animation)
		{
            if (animation != null)
			{
				_faceDirection.CurrentDirectionalAnimation = animation;
				await _faceDirection.FaceDirectionAsync(_faceDirection.Direction);
			}
		}

		private async Task waitForText(string text, Task externalWait, ISound sound)
		{
			switch (SpeechConfig.SkipText)
			{
				case SkipText.ByMouse:
					await Task.WhenAny(waitForClick(), externalWait);
					break;
				case SkipText.ByTime:
					await Task.WhenAny(waitTime(text, sound), externalWait);
					break;
				case SkipText.ByTimeAndMouse:
					Task waitingTime = waitTime(text, sound);
					Task completed = await Task.WhenAny(waitingTime, waitForClick(), externalWait);
					if (completed == waitingTime)
					{
						_fastFingerChecker.StartMeasuring();
					}
					else
					{
						_fastFingerChecker.StopMeasuring();
					}
					break;
				case SkipText.External:
					await externalWait;
					break;
				default:
					throw new NotSupportedException ("Skip text configuration is not supported: " + SpeechConfig.SkipText.ToString());
			}
			await Task.Delay(10);
		}

		private async Task waitForClick()
		{
			await _input.MouseDown.WaitUntilAsync(e => e.Button == MouseButton.Left && !_fastFingerChecker.IsFastFinger());
		}

		private async Task waitTime(string text, ISound sound)
		{
			if (sound == null || !sound.IsValid) await Task.Delay(40 + text.Length * SpeechConfig.TextDelay);
			else await sound.Completed;
		}

        private ISayLocation getLocation(string text)
		{
			return _location.GetLocation(text, SpeechConfig);
		}
	}
}

