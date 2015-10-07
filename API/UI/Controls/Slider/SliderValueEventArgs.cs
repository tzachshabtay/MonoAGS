using System;

namespace AGS.API
{
	public class SliderValueEventArgs : AGSEventArgs
	{
		public SliderValueEventArgs(float value)
		{
			Value = value;
		}

		public float Value { get; private set; }
	}
}

