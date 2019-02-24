using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AGS.Engine
{
    //Code extracted from: https://github.com/opentk/opentk/blob/develop/src/OpenTK/GameWindow.cs
    public class AGSUpdateThread
    {
        private readonly Stopwatch watch = new Stopwatch();

        private double update_period;
        private double target_update_period;
        private double update_time; // length of last UpdateFrame event
        private double update_timestamp; // timestamp of last UpdateFrame event
        private double update_epsilon; // quantization error for UpdateFrame events

        private bool is_running_slowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        private FrameEventArgs update_args = new FrameEventArgs();

        private const double MaxFrequency = 500.0; // Frequency cap for Update/RenderFrame events

        private Task _thread;

        private IGameWindow _window;
        private bool _isExiting => _window.IsExiting;

        private SpinWait _spinner = new SpinWait();

        public AGSUpdateThread(IGameWindow window)
        {
            _window = window;
        }

        /// <summary>
        /// Occurs when it is time to update a frame.
        /// </summary>
        public event EventHandler<FrameEventArgs> UpdateFrame = delegate { };

        public event EventHandler OnThreadStarted = delegate { };

        /// <summary>
        /// Gets or sets a double representing the target update frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 500.0Hz.</para>
        /// </remarks>
        public double TargetUpdateFrequency
        {
            get
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (TargetUpdatePeriod == 0.0)
                {
                    return 0.0;
                }
                return 1.0 / TargetUpdatePeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    TargetUpdatePeriod = 0.0;
                }
                else if (value <= MaxFrequency)
                {
                    TargetUpdatePeriod = 1.0 / value;
                }
                else
                {
                    Debug.WriteLine("Target render frequency clamped to {0}Hz.", MaxFrequency);
                }
            }
        }

        /// <summary>
        /// Gets or sets a double representing the target update period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public double TargetUpdatePeriod
        {
            get
            {
                return target_update_period;
            }
            set
            {
                if (value <= 1 / MaxFrequency)
                {
                    target_update_period = 0.0;
                }
                else if (value <= 1.0)
                {
                    target_update_period = value;
                }
                else
                {
                    Debug.WriteLine("Target update period clamped to 1.0 seconds.");
                }
            }
        }

        /// <summary>
        /// Gets a double representing the frequency of UpdateFrame events, in hertz.
        /// </summary>
        public double UpdateFrequency
        {
            get
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (update_period == 0.0)
                {
                    return 1.0;
                }
                return 1.0 / update_period;
            }
        }

        /// <summary>
        /// Gets a double representing the period of UpdateFrame events, in seconds.
        /// </summary>
        public double UpdatePeriod
        {
            get
            {
                return update_period;
            }
        }

        /// <summary>
        /// Gets a double representing the time spent in the UpdateFrame function, in seconds.
        /// </summary>
        public double UpdateTime
        {
            get
            {
                return update_time;
            }
        }

        public void Run(double updates_per_second, bool spawnThread)
        {
            if (updates_per_second < 0.0 || updates_per_second > 200.0)
            {
                throw new ArgumentOutOfRangeException("updates_per_second", updates_per_second,
                    "Parameter should be inside the range [0.0, 200.0]");
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (updates_per_second != 0)
            {
                TargetUpdateFrequency = updates_per_second;
            }
            if (_thread != null) return;
            if (spawnThread)
            {
                _thread = Task.Factory.StartNew(threadLoop, TaskCreationOptions.LongRunning);
            }
            else {
                threadLoop();
            }
        }

        private void threadLoop()
        {
            AGSGame.UpdateThreadID = Environment.CurrentManagedThreadId;
            OnThreadStarted(this, null);
            watch.Start();
            while (!_isExiting)
            {
                DispatchUpdateFrame();
            }
        }

        private double ClampElapsed(double elapsed)
        {
            return Math.Max(Math.Min(elapsed, 1.0), 0.0);
        }

        private void DispatchUpdateFrame()
        {
            int is_running_slowly_retries = 4;
            double timestamp = watch.Elapsed.TotalSeconds;

            double elapsed = ClampElapsed(timestamp - update_timestamp);
            while (elapsed > 0 && elapsed + update_epsilon >= TargetUpdatePeriod)
            {
                RaiseUpdateFrame(elapsed, ref timestamp);

                // Calculate difference (positive or negative) between
                // actual elapsed time and target elapsed time. We must
                // compensate for this difference.
                update_epsilon += elapsed - TargetUpdatePeriod;

                // Prepare for next loop
                elapsed = ClampElapsed(timestamp - update_timestamp);

                if (TargetUpdatePeriod <= Double.Epsilon)
                {
                    // According to the TargetUpdatePeriod documentation,
                    // a TargetUpdatePeriod of zero means we will raise
                    // UpdateFrame events as fast as possible (one event
                    // per ProcessEvents() call)
                    break;
                }

                is_running_slowly = update_epsilon >= TargetUpdatePeriod;
                if (is_running_slowly && --is_running_slowly_retries == 0)
                {
                    // If UpdateFrame consistently takes longer than TargetUpdateFrame
                    // stop raising events to avoid hanging inside the UpdateFrame loop.
                    break;
                }
            }
            _spinner.SpinOnce();
        }

        private void RaiseUpdateFrame(double elapsed, ref double timestamp)
        {
            // Raise UpdateFrame event
            update_args.Time = elapsed;
            UpdateFrame(this, update_args);

            // Update UpdatePeriod/UpdateFrequency properties
            update_period = elapsed;

            // Update UpdateTime property
            update_timestamp = timestamp;
            timestamp = watch.Elapsed.TotalSeconds;
            update_time = timestamp - update_timestamp;
        }
    }
}
