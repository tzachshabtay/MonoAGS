using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public enum TweenCompletion
	{
		Complete,
		Rewind,
		Pause,
	}

	public class Tween
	{
		private TaskCompletionSource<object> _tcs;
		private static IGameEvents _gameEvents { get { return OverrideGameEvents ?? AGSGame.Game.Events; } }

		private Tween(bool subscribe)
		{
			_tcs = new TaskCompletionSource<object> (null);
			if (subscribe) _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		public float DurationInTicks { get; private set; }
		public float ElapsedTicks { get; private set; }
		public float From { get; private set; }
		public float To { get; private set; }
		public Func<float, float> Easing { get; private set; }
		public Action<float> Setter { get; private set; }
		public Task Task { get { return _tcs.Task; } }

		public static IGameEvents OverrideGameEvents { private get; set; }

		public static Tween Run(float from, float to, Action<float> setter, 
			float timeInSeconds, Func<float, float> easing = null)
		{
			easing = easing ?? Ease.Linear;
			Tween tween = new Tween(true)
			{
				From = from, To = to, Setter = setter, Easing = easing, DurationInTicks = toTicks(timeInSeconds)
			};
			return tween;
		}

		public static Tween RunWithExternalVisit(float from, float to, Action<float> setter, 
			float timeInSeconds, Func<float, float> easing, out Action visitCallback)
		{
			easing = easing ?? Ease.Linear;
			Tween tween = new Tween(false)
			{
				From = from, To = to, Setter = setter, Easing = easing, DurationInTicks = toTicks(timeInSeconds)
			};
			visitCallback = tween.visit;
			return tween;
		}

		private static float toTicks(float timeInSeconds)
		{
			return timeInSeconds * (float)AGSGame.UPDATE_RATE;
		}

		private void onRepeatedlyExecute()
		{
			visit();
		}

		private void visit()
		{
			ElapsedTicks++;
			if (ElapsedTicks >= DurationInTicks)
			{
				Stop(TweenCompletion.Complete);
				return;
			}
			float value = Easing(ElapsedTicks / DurationInTicks);
			float lerpedValue = MathUtils.Lerp(0f, From, 1, To, value);
			Setter(lerpedValue);
		}

		public void Stop(TweenCompletion completion)
		{
			if (completion == TweenCompletion.Complete)
			{
				Setter(To);
				ElapsedTicks = DurationInTicks;
			}
			else if (completion == TweenCompletion.Rewind)
			{
				Setter(From);
				ElapsedTicks = 0;
			}

			_tcs.TrySetResult(null);
			_gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
		}

		public void Resume()
		{
			_tcs = new TaskCompletionSource<object> (null);
			_gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}
	}
}

