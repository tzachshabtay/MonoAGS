using AGS.API;

namespace AGS.Engine
{
	public class AGSFollowSettings : IFollowSettings
	{
		public AGSFollowSettings (bool followBetweenRooms = true, int wanderOffPercentage = 20, int minWaitBetweenWalks = 50,
		                          int maxWaitBetweenWalks = 500, float minXOffset = 30f, float maxXOffset = 60f, 
		                          float minYOffset = 5f, float maxYOffset = 10f)
		{
			FollowBetweenRooms = followBetweenRooms;
			MinWaitBetweenWalks = minWaitBetweenWalks;
			MaxWaitBetweenWalks = maxWaitBetweenWalks;
			MinXOffset = minXOffset;
			MaxXOffset = maxXOffset;
			MinYOffset = minYOffset;
			MaxYOffset = maxYOffset;
			WanderOffPercentage = wanderOffPercentage;
		}

		public bool FollowBetweenRooms { get; private set; }
		public int WanderOffPercentage { get; private set; }

		public int MinWaitBetweenWalks { get; private set; }
		public int MaxWaitBetweenWalks { get; private set; }

		public float MinXOffset { get; private set; }
		public float MaxXOffset { get; private set; }

		public float MinYOffset { get; private set; }
		public float MaxYOffset { get; private set; }
	}
}

