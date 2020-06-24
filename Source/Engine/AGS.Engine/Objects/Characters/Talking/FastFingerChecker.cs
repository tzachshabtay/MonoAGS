using System.Diagnostics;

namespace AGS.Engine
{
	public class FastFingerChecker
	{
		private readonly Stopwatch _fastFingerChecker;

		public FastFingerChecker()
		{
			FastFingerSafeBuffer = 500;
			_fastFingerChecker = new Stopwatch ();
		}

		public int FastFingerSafeBuffer { get; set; }

		public void StartMeasuring()
		{
			_fastFingerChecker.Restart();
		}

		public void StopMeasuring()
		{
			_fastFingerChecker.Stop();
		}

		public bool IsFastFinger()
		{
			return _fastFingerChecker.IsRunning && _fastFingerChecker.ElapsedMilliseconds < FastFingerSafeBuffer;
		}
	}
}

