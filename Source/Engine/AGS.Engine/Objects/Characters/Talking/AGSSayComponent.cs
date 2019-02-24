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
        private IWalkComponent _walkComponent;
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

        [Property(DisplayName = "Speech")]
		public ISayConfig SpeechConfig { get; private set; }
		public IBlockingEvent<BeforeSayEventArgs> OnBeforeSay { get; private set; }

        public override void Init()
        {
            base.Init();
            _characterName = Entity.ID;
            Entity.Bind<IWorldPositionComponent>(c => _emitter.WorldPosition = c, _ => _emitter.WorldPosition = null);
            Entity.Bind<IHasRoomComponent>(c => _emitter.HasRoom = c, _ => _emitter.HasRoom = null);
            Entity.Bind<IFaceDirectionComponent>(c => _faceDirection = c, _ => _faceDirection = null);
            Entity.Bind<IOutfitComponent>(c => _outfit = c, _ => _outfit = null);
            Entity.Bind<IWalkComponent>(c => _walkComponent = c, _ => _walkComponent = null);
        }

        public async Task SayAsync(string text, PointF? textPosition = null, PointF? portraitPosition = null)
		{
            var outfit = _outfit;
            var walkComponent = _walkComponent;
            var previousAnimation = _faceDirection?.CurrentDirectionalAnimation;
            var speakAnimation = outfit == null ? null : outfit.Outfit[AGSOutfit.Speak];
            bool wasWalking = false;
            if (walkComponent?.IsWalking ?? false)
            {
                if (outfit?.Outfit[AGSOutfit.SpeakAndWalk] == null)
                {
                    await walkComponent.StopWalkingAsync();
                    previousAnimation = _outfit.Outfit[AGSOutfit.Idle];
                }
                else
                {
                    wasWalking = true;
                    speakAnimation = outfit.Outfit[AGSOutfit.SpeakAndWalk];
                }
            }
            if (_state.Cutscene.IsSkipping)
			{
                if (outfit != null) await setAnimation(previousAnimation);
				return;
			}

            if (speakAnimation != null) await setAnimation(speakAnimation);
			await Task.Delay(1);
            var speech = await _speechCache.GetSpeechLineAsync(_characterName, text);
            text = speech.Text;

            ISayLocation location = getLocation(text);
            var textLocation = textPosition ?? location.TextLocation;
            portraitPosition = portraitPosition ?? location.PortraitLocation;
            IObject portrait = showPortrait(portraitPosition);
            ILabel label = _factory.UI.GetLabel($"Say: {text} {Guid.NewGuid().ToString()}", text, SpeechConfig.LabelSize.Width, 
                SpeechConfig.LabelSize.Height, textLocation.X, textLocation.Y,
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

            if (outfit != null)
            {
                if (wasWalking && !walkComponent.IsWalking) previousAnimation = outfit.Outfit[AGSOutfit.Idle]; //If we were in the middle of a walk but walk was completed before speech, then instead of revert to the previous animation (walk) we need to go to idle.
                await setAnimation(previousAnimation);
            }
		}

        private IObject showPortrait(PointF? portraitLocation)
        {
            if (portraitLocation == null) return null;
            var portraitConfig = SpeechConfig.PortraitConfig;
            if (portraitConfig == null) return null;
            IObject portrait = portraitConfig.Portrait;
            if (portrait != null)
            {
                portrait.Visible = true;
                portrait.Position = new Position(portraitLocation.Value);
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