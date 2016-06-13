using System;
namespace AGS.API
{
	public interface IFollowSettings
	{
		int MinWaitBetweenWalks { get; }
		int MaxWaitBetweenWalks { get; }

		float MinXOffset { get; }
		float MaxXOffset { get; }
		float MinYOffset { get; }
		float MaxYOffset { get; }

		int WanderOffPercentage { get; }
		bool FollowBetweenRooms { get; }
	}
}

