using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSSayBehavior : AGSComponent, ISayBehavior
	{
		private readonly IGameState _state;
		private readonly IGameFactory _factory;
		private readonly IInput _input;
		private readonly ISayLocation _location;
		private readonly FastFingerChecker _fastFingerChecker;
		private readonly IHasOutfit _outfit;
		private readonly IFaceDirectionBehavior _faceDirection;
        private readonly ISoundEmitter _emitter;
        private readonly ISpeechCache _speechCache;
        private string _characterName;

		public AGSSayBehavior(IGameState state, IGameFactory factory, IInput input, ISayLocation location,
			                  FastFingerChecker fastFingerChecker, ISayConfig sayConfig, IHasOutfit outfit, 
                              IFaceDirectionBehavior faceDirection, IBlockingEvent<BeforeSayEventArgs> onBeforeSay, 
                              ISoundEmitter emitter, ISpeechCache speechCache)
		{
			_state = state;
			_factory = factory;
			_input = input;
			_location = location;
			_fastFingerChecker = fastFingerChecker;
			_outfit = outfit;
			_faceDirection = faceDirection;
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
            _emitter.AnimationContainer = entity.GetComponent<IAnimationContainer>();
            _emitter.HasRoom = entity.GetComponent<IHasRoom>();
        }

		public void Say(string text)
		{
			Task.Run(async () => await SayAsync(text)).Wait();
		}

		public async Task SayAsync(string text)
		{
			if (_state.Cutscene.IsSkipping)
			{
				if (_outfit != null) setAnimation(_outfit.Outfit.IdleAnimation);
				return;
			}
			if (_outfit != null) setAnimation(_outfit.Outfit.SpeakAnimation);
			await Task.Delay(1);
            var speech = await _speechCache.GetSpeechLineAsync(_characterName, text);
            text = speech.Text;

            PointF location = getLocation(text);
			ILabel label = _factory.UI.GetLabel(string.Format("Say: {0}", text), text, SpeechConfig.LabelSize.Width, SpeechConfig.LabelSize.Height, 
				location.X, location.Y, SpeechConfig.TextConfig, false);
			label.RenderLayer = AGSLayers.Speech;
			label.Border = SpeechConfig.Border;
			label.Tint = SpeechConfig.BackgroundColor;
			TaskCompletionSource<object> externalSkipToken = new TaskCompletionSource<object> (null);
			BeforeSayEventArgs args = new BeforeSayEventArgs (label, () => externalSkipToken.TrySetResult(null));
			OnBeforeSay.Invoke(this, args);
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

			if (_outfit != null) setAnimation(_outfit.Outfit.IdleAnimation);
		}

		private void setAnimation(IDirectionalAnimation animation)
		{
			if (_outfit.Outfit.SpeakAnimation != null)
			{
				_faceDirection.CurrentDirectionalAnimation = animation;
				_faceDirection.FaceDirection(_faceDirection.Direction);
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
			if (sound == null) await Task.Delay(40 + text.Length * SpeechConfig.TextDelay);
			else await sound.Completed;
		}

		private PointF getLocation(string text)
		{
			return _location.GetLocation(text, SpeechConfig.LabelSize, SpeechConfig.TextConfig);
		}
	}
}

