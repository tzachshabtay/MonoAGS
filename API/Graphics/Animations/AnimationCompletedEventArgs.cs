using System;

namespace API
{
	public class AnimationCompletedEventArgs : EventArgs
	{
		public AnimationCompletedEventArgs (bool completedSuccessfully)
		{
			CompletedSuccessfully = completedSuccessfully;
		}

		public bool CompletedSuccessfully { get; private set; }
	}
}

