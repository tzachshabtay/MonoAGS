using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Diagnostics;

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

		public AGSSayBehavior(IGameState state, IGameFactory factory, IInput input, ISayLocation location,
			FastFingerChecker fastFingerChecker, ISayConfig sayConfig, IHasOutfit outfit, IFaceDirectionBehavior faceDirection)
		{
			_state = state;
			_factory = factory;
			_input = input;
			_location = location;
			_fastFingerChecker = fastFingerChecker;
			_outfit = outfit;
			_faceDirection = faceDirection;
			SpeechConfig = sayConfig;
		}

		public ISayConfig SpeechConfig { get; private set; }

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
			PointF location = getLocation(text);
			ILabel label = _factory.UI.GetLabel(string.Format("Say: {0}", text), text, SpeechConfig.LabelSize.Width, SpeechConfig.LabelSize.Height, 
				location.X, location.Y, SpeechConfig.TextConfig);
			label.RenderLayer = AGSLayers.Speech;
			label.Border = SpeechConfig.Border;
			label.Tint = SpeechConfig.BackgroundColor;
			await waitForText(text);
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

		private async Task waitForText(string text)
		{
			switch (SpeechConfig.SkipText)
			{
				case SkipText.ByMouse:
					await waitForClick();
					break;
				case SkipText.ByTime:
					await waitTime(text);
					break;
				case SkipText.ByTimeAndMouse:
					Task waitingTime = waitTime(text);
					Task completed = await Task.WhenAny(waitingTime, waitForClick());
					if (completed == waitingTime)
					{
						_fastFingerChecker.StartMeasuring();
					}
					else
					{
						_fastFingerChecker.StopMeasuring();
					}
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

		private async Task waitTime(string text)
		{
			await Task.Delay(40 + text.Length * SpeechConfig.TextDelay);
		}

		private PointF getLocation(string text)
		{
			return _location.GetLocation(text, SpeechConfig.LabelSize, SpeechConfig.TextConfig);
		}
	}
}

