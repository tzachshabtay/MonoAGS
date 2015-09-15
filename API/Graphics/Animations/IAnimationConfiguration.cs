using System;

namespace AGS.API
{
	public enum LoopingStyle
	{
		/// <summary>
		/// Will animate the loop forwards, and then start again from the top
		/// </summary>
		Forwards,
		/// <summary>
		/// Will animate the loop backwards, and then start again from the bottom
		/// </summary>
		Backwards,
		/// <summary>
		/// Will animate the loop forwards, and then it will go back, and start again
		/// </summary>
		ForwardsBackwards,
		/// <summary>
		/// Will animate the loop backwards, and then will it will go forwards, and start again
		/// </summary>
		BackwardsForwards,
	}

	public interface IAnimationConfiguration
	{
		LoopingStyle Looping { get; set; }

		/// <summary>
		/// 0 for endless.
		/// </summary>
		/// <value>The loops.</value>
		int Loops { get; set; }
	}
}

