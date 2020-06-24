using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    /// <summary>
    /// How is the tween "completed" when stopped?
    /// </summary>
	public enum TweenCompletion
	{
        /// <summary>
        /// The tween is completed as planned by setting its value to the target value.
        /// </summary>
		Complete,
        /// <summary>
        /// The tween is rewind back to its initial value.
        /// </summary>
		Rewind,
        /// <summary>
        /// The tween will not alter the value, it will stay with the value set by the last tween tick before being stopped.
        /// </summary>
		Stay,
	}

    /// <summary>
    /// The state of the tween.
    /// </summary>
    public enum TweenState
    {
        /// <summary>
        /// The tween is playing. Can be paused by calling <see cref="Tween.Pause"/> or stopped by calling <see cref="Tween.Stop"/>. 
        /// </summary>
        Playing,
        /// <summary>
        /// The tween is paused. Can be resumed by calling <see cref="Tween.Resume"/>. 
        /// </summary>
        Paused,
        /// <summary>
        /// The tween was manually ordered to stop and it's working on that. It cannot be replayed again.
        /// The state will move to "Stopped" in the next tick.
        /// </summary>
        Stopping,
        /// <summary>
        /// The tween was manually stopped. It cannot be replayed again.
        /// </summary>
        Stopped,
        /// <summary>
        /// The tween has completed. It cannot be replayed again.
        /// </summary>
        Completed
    }

    /// <summary>
    /// A tween represents a value that is "animated" between two values (from an initial value to a target value).
    /// To run a tween you can use any one of the built in tweens (for example, 'obj.TweenX', 'obj.TweenScaleY', 
    /// you can see a full list of built in tweens in the <see cref="Tweens"/> static class.
    /// 
    /// Alternatively, you can use the general purpose function <see cref="Run"/>.
    /// 
    /// Once you've started a tween, it will run asynchronously, it will not wait for completion before your next line of code runs.
    /// At any time you want to wait for the tween to complete you can await (asynchronously wait) its task. 
    /// If you don't want to use await, there are other ways to interact with a running tween, see <see cref="Task"/> for more information.
    /// 
    /// If you want your tween to repeat, you can either call <see cref="RepeatForever"/> or <see cref="RepeatTimes"/> 
    /// on the tween.
    /// </summary>
	public class Tween
	{
		private TaskCompletionSource<object> _tcs;
        private static IGameEvents _gameEvents => OverrideGameEvents ?? AGSGame.Game.Events;
        private TweenCompletion _completeOnStop;

		private Tween(bool subscribe)
		{
			_tcs = new TaskCompletionSource<object> (null);
			if (subscribe) _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

        /// <summary>
        /// Gets the duration of the tween in game ticks (if the game is running at 60 FPS, then there should be 60 ticks each second).
        /// Note that if this is a repeating tween, the duration refers to a single loop.
        /// </summary>
        /// <value>The duration in ticks.</value>
		public float DurationInTicks { get; private set; }

        /// <summary>
        /// Gets the duration of the tween in seconds.
        /// Note that if this is a repeating tween, the duration refers to a single loop.
        /// Also note that if this tween is running externally (via <see cref="RunWithExternalVisit"/>), 
        /// the timing of the tween's ticks is controlled by you and not the game, therefore the seconds might not be accurate. 
        /// </summary>
        /// <value>The duration in seconds.</value>
        public float DurationInSeconds => toSeconds(DurationInTicks);

        /// <summary>
        /// Gets the amount of time the tween has been running in game ticks (if the game is running at 60 FPS, then there should be 60 ticks each second).
        /// Note that if this is a repeating tween, the elapsed time refers to a single loop.
        /// Also, for a repeating tween, the elapsed ticks can be negative, meaning that it's currently in the delay between loops.
        /// This can also be set for time manipulations.
        /// </summary>
        /// <value>The elapsed ticks.</value>
        public float ElapsedTicks { get; set; }

        /// <summary>
        /// Gets the amount of time the tween has been running in seconds.
        /// Note that if this is a repeating tween, the elapsed time refers to a single loop.
        /// Also note that if this tween is running externally (via <see cref="RunWithExternalVisit"/>), 
        /// the timing of the tween's ticks is controlled by you and not the game, therefore the seconds might not be accurate. 
        /// 
        /// This can also be set for time manipulations.
        /// </summary>
        /// <value>The elapsed ticks.</value>
        public float ElapsedSeconds 
        {
            get => toSeconds(ElapsedTicks);
            set => ElapsedTicks = toTicks(value);
        }

        /// <summary>
        /// Gets the initial value for the tween.
        /// </summary>
        /// <value>From.</value>
		public float From { get; private set; }

        /// <summary>
        /// Gets the target value for the tween.
        /// </summary>
        /// <value>To.</value>
		public float To { get; private set; }

        /// <summary>
        /// If this is a repeating tween, this will get the configuration and state of the repetitions.
        /// </summary>
        /// <value>The repeat info.</value>
        public TweenRepeatInfo RepeatInfo { get; private set; }

        /// <summary>
        /// Gets the easing function (the curve) for the tween.
        /// <seealso cref="Ease"/> .
        /// </summary>
        /// <value>The easing.</value>
		public Func<float, float> Easing { get; private set; }

        /// <summary>
        /// Gets the setter action for the tween. This is the action
        /// that sets the tweened value on the actual object you want tweened.
        /// </summary>
        /// <value>The setter.</value>
		public Action<float> Setter { get; private set; }

        /// <summary>
        /// This is the on-going tweening task.
        /// You can use the task for the following things:
        /// 1. To asynchronously wait for the tween to complete ('await myTween.Task')
        /// 2. To block and wait for the tween to complete ('myTween.Task.Wait()'). Note that this is less recommended than asynchronously awaiting.
        /// 3. To asynchronously wait for several tweens to complete ('await Task.WhenAll(tween1.Task, tween2.Task, tween3.Task')
        /// 4. To asynchronously wait for the first tween of several to complete ('await Task.WhenAny(tween1.Task, tween2.Tasl, tween3.Tasl').
        /// Note that this also returns back the first completed task which you can use for your logic.
        /// 5. You can query whether the task was completed yet ('tween.Task.IsCompleted').
        /// 6. You can chain a callback to when it's completed ('tween.Task.ContinueWith(...)').
        /// </summary>
        /// <value>The task.</value>
		public Task Task => _tcs.Task;

        /// <summary>
        /// Gets the tween's state.
        /// </summary>
        /// <value>The state.</value>
        public TweenState State { get; private set; }

		public static IGameEvents OverrideGameEvents { private get; set; }

        /// <summary>
        /// Starts a repeating tween, that will repeat forever unless manually stopped (by calling <see cref="Stop"/>).
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="loopingStyle">The looping style of the tween.</param>
        /// <param name="delayBetweenLoopsInSeconds">The number of seconds the tween will wait before starting the next loop.</param>
        public Tween RepeatForever(LoopingStyle loopingStyle = LoopingStyle.Forwards, float delayBetweenLoopsInSeconds = 0f)
        {
            RepeatInfo = new TweenRepeatInfo 
            { 
                Looping = loopingStyle,
                RunningBackwards = loopingStyle == LoopingStyle.Backwards || loopingStyle == LoopingStyle.BackwardsForwards,
                TotalLoops = 0,
                DelayBetweenLoopsInSeconds = delayBetweenLoopsInSeconds
            };
            return this;
        }

        /// <summary>
        /// Starts a repeating tween, that will repeat a number of times, as specified, unless manually stopped (by calling <see cref="Stop"/>).
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="times">Number of times to run the tween for.</param>
        /// <param name="loopingStyle">The looping style.</param>
        /// <param name="delayBetweenLoopsInSeconds">The number of seconds the tween will wait before starting the next loop.</param>
        public Tween RepeatTimes(int times, LoopingStyle loopingStyle = LoopingStyle.Forwards, float delayBetweenLoopsInSeconds = 0f)
        {
            RepeatInfo = new TweenRepeatInfo
            {
                Looping = loopingStyle,
                RunningBackwards = loopingStyle == LoopingStyle.Backwards || loopingStyle == LoopingStyle.BackwardsForwards,
                TotalLoops = times,
                DelayBetweenLoopsInSeconds = delayBetweenLoopsInSeconds
            };
            return this;
        }

        /// <summary>
        /// Starts a new tween. On each game tick, the tween will advance a value
        /// from "from" to "to" on a selected curve, creating an animation.
        /// The value to be advanced is passed via the setter, which is an action that
        /// will run each tick with the current value.
        /// The curve is the easing function that receives a point in time and returns a modified point in time.
        /// The simplest curve is the linear curve which is a function that returns the same time as received, meaning
        /// the value will advance equally on each tick. This is also the default curve if you don't specify an easing.
        /// A lot of easing functions are already built in, you can find them using the static 'Ease' class ('Ease.Linear', 'Ease.ElasticIn', etc).
        /// 
        /// Note that while this function can tween any value, there are a lot of built in tweens on different entity types
        /// to make the tween code a little more simple.
        /// <example>
        /// To tween the X position of an object on a "sine in" curve, we can either use the build in tween:
        /// <code language="lang-csharp">
        /// var tween = obj.TweenX(200f, 2f, Ease.SineIn); //Will animate the x position to x = 200 in 2 seconds on a "sine in" curve.
        /// await tween.Task; //Asynchronously wait for the tween to complete.
        /// </code>
        /// Or use the general purpose function:
        ///<code language="lang-csharp">
        /// var tween = Tween.Run(obj.X, 200f, x => obj.X = x, 2f, Ease.SineIn); //Same animation as before
        /// await tween.Task;
        /// </code>
        /// </example> 
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="setter">The setter function.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">The easing function.</param>
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

        /// <summary>
        /// Starts a new tween where you control the speed of the tween.
        /// This operates the same way as <see cref="Run"/> (you can view more detailed description in that function),
        /// where the only difference is that instead of the tween tick being on every game tick, the tween ticks
        /// whenever you call the 'visit callback' function that gets returned from this function. 
        /// </summary>
        /// <returns>The tween.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="setter">The setter function.</param>
        /// <param name="timeInSeconds">Time in seconds.</param>
        /// <param name="easing">The easing function.</param>
        /// <param name="visitCallback">Visit callback function.</param>
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

        private static float toTicks(float timeInSeconds) => timeInSeconds * (float)AGSGame.UPDATE_RATE;

        private static float toSeconds(float timeInTicks) => timeInTicks / (float)AGSGame.UPDATE_RATE;

        private void onRepeatedlyExecute()
		{
			visit();
		}

		private void visit()
		{
            if (State == TweenState.Stopping)
            {
                State = TweenState.Stopped;
                stop(_completeOnStop);
            }
            if (State != TweenState.Playing) return;
            var repeat = RepeatInfo;
			ElapsedTicks++;
            if (ElapsedTicks < 0) return;
			if (ElapsedTicks >= DurationInTicks)
			{
                if (repeat == null || !repeat.NextLoop())
                {
                    if (State != TweenState.Playing) return;
                    stop(TweenCompletion.Complete);
                    State = TweenState.Completed;
                }
                else if (repeat.DelayBetweenLoopsInSeconds > 0f)
                {
                    ElapsedTicks = -toTicks(repeat.DelayBetweenLoopsInSeconds);
                }
                else
                {
                    ElapsedTicks = 0;
                }
				return;
			}
			float value = Easing(ElapsedTicks / DurationInTicks);
            float from = repeat == null || !repeat.RunningBackwards ? From : To;
            float to = repeat == null || !repeat.RunningBackwards ? To : From;
            float lerpedValue = MathUtils.Lerp(0f, from, 1, to, value);
			Setter(lerpedValue);
		}

        private void stop(TweenCompletion completion)
        {
            _gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
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
        }

        /// <summary>
        /// Stop the running tween. 
        /// <seealso cref="Resume"/>
        /// </summary>
        /// <param name="completion">The tween completion describes where to set the tween value after stopping: 
        /// should it complete the tween, rewind to the beginning or pause in its current position?</param>
		public void Stop(TweenCompletion completion)
		{
            _completeOnStop = completion;
            State = TweenState.Stopping;
		}

        /// <summary>
        /// Pauses a playing tween. If the tween is already paused (or has already been stopped
        /// or completed), this does nothing.
        /// </summary>
        public void Pause()
        {
            if (State != TweenState.Playing) return;
            State = TweenState.Paused;
        }

        /// <summary>
        /// Resumes a previously paused tween.
        /// If the tween was stopped (or completed) it cannot be resumed and this does nothing.
        /// If the tween is currently playing, this also does nothing.
        /// <seealso cref="Stop"/>
        /// </summary>
		public void Resume()
		{
            if (State != TweenState.Paused) return;
            State = TweenState.Playing;
		}

        /// <summary>
        /// Rewind the tween to the beginning.
        /// Note: if this is a repeating tween, it will rewind to the start of the current loop.
        /// </summary>
        public void Rewind()
        {
            ElapsedTicks = 0f;
        }
	}
}

