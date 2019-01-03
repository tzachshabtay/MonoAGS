using System;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
	public class AGSFollowSettings : IFollowSettings
	{
		public AGSFollowSettings (bool followBetweenRooms = true, int wanderOffPercentage = 20, int minWaitBetweenWalks = 50,
		                          int maxWaitBetweenWalks = 500, float minXOffset = 30f, float maxXOffset = 60f, float minYOffset = 5f, float maxYOffset = 10f, 
		                          int stayPutPercentage = 0, float minimumWalkingDistance = 10f, int stayOnTheSameSideForXPercentage = 70, int stayOnTheSameSideForYPercentage = 70, 
		                          float? minXOffsetForWanderOff = null, float? maxXOffsetForWanderOff = null, float? minYOffsetForWanderOff = null, float? maxYOffsetForWanderOff = null)
		{
		    tracePositive(nameof(minXOffset), minXOffset);
		    tracePositive(nameof(maxXOffset), maxXOffset);
		    tracePositive(nameof(minYOffset), minYOffset);
		    tracePositive(nameof(maxYOffset), maxYOffset);
		    tracePositive(nameof(minXOffsetForWanderOff), minXOffsetForWanderOff);
		    tracePositive(nameof(maxXOffsetForWanderOff), maxXOffsetForWanderOff);
		    tracePositive(nameof(minYOffsetForWanderOff), minYOffsetForWanderOff);
		    tracePositive(nameof(maxYOffsetForWanderOff), maxYOffsetForWanderOff);

		    traceMinMax("WaitBetweenWalks", minWaitBetweenWalks, maxWaitBetweenWalks);
		    traceMinMax("XOffset", minXOffset, maxXOffset);
		    traceMinMax("YOffset", minYOffset, maxYOffset);
		    traceMinMax("XOffsetForWanderOff", minXOffsetForWanderOff, maxXOffsetForWanderOff);
		    traceMinMax("YOffsetForWanderOff", minYOffsetForWanderOff, maxYOffsetForWanderOff);
		    
		    traceWalkingDistance("walking area", maxXOffset, maxYOffset, minimumWalkingDistance);
		    traceWalkingDistance("walking area for wandering off", maxXOffsetForWanderOff, maxYOffsetForWanderOff, minimumWalkingDistance);

		    tracePercentage(nameof(StayOnTheSameSideForXPercentage), StayOnTheSameSideForXPercentage);
		    tracePercentage(nameof(StayOnTheSameSideForYPercentage), StayOnTheSameSideForYPercentage);
		    tracePercentage(nameof(WanderOffPercentage), WanderOffPercentage);
		    tracePercentage(nameof(StayPutPercentage), StayPutPercentage);

		    StayOnTheSameSideForXPercentage = stayOnTheSameSideForXPercentage;
		    StayOnTheSameSideForYPercentage = stayOnTheSameSideForYPercentage;
		    FollowBetweenRooms = followBetweenRooms;
			MinWaitBetweenWalks = minWaitBetweenWalks;
			MaxWaitBetweenWalks = maxWaitBetweenWalks;
			MinXOffset = minXOffset;
			MaxXOffset = maxXOffset;
			MinYOffset = minYOffset;
			MaxYOffset = maxYOffset;
		    MinXOffsetForWanderOff = minXOffsetForWanderOff;
		    MaxXOffsetForWanderOff = maxXOffsetForWanderOff;
		    MinYOffsetForWanderOff = minYOffsetForWanderOff;
		    MaxYOffsetForWanderOff = maxYOffsetForWanderOff;
			WanderOffPercentage = wanderOffPercentage;
		    StayPutPercentage = stayPutPercentage;
		    MinimumWalkingDistance = minimumWalkingDistance;
		}

		public bool FollowBetweenRooms { get; }
	    public int WanderOffPercentage { get; }
	    public int StayPutPercentage { get; }
	    public int StayOnTheSameSideForXPercentage { get; }
	    public int StayOnTheSameSideForYPercentage { get; }

	    public int MinWaitBetweenWalks { get; }
		public int MaxWaitBetweenWalks { get; }

		public float MinXOffset { get; }
		public float MaxXOffset { get; }
		public float MinYOffset { get; }
		public float MaxYOffset { get; }

	    public float? MinXOffsetForWanderOff { get; }
	    public float? MaxXOffsetForWanderOff { get; }
	    public float? MinYOffsetForWanderOff { get; }
	    public float? MaxYOffsetForWanderOff { get; }

	    public float MinimumWalkingDistance { get; }

	    private void tracePercentage(string name, int val)
	    {
	        Trace.Assert(val >= 0 && val <= 100, $"{name} is a percentage value, it needs to be between 0 and 100 ({val} is not in range)");
	    }

	    private void tracePositive(string name, float? val)
	    {
	        if (val == null) return;
	        Trace.Assert(val >= 0, $"{name} must be positive ({val} is not in range)");
	    }

	    private void traceMinMax(string name, float? min, float? max)
	    {
	        if (min == null || max == null) return;
	        Trace.Assert(min <= max, $"{name}: range is wrong, min {min} is bigger than max {max}");
	    }

	    private void traceWalkingDistance(string name, float? x, float? y, float minWalkingDistance)
	    {
	        if (x == null || y == null) return;
	        float allowedRange = MathUtils.Distance((0f, 0f), (x.Value, y.Value));
	        Trace.Assert(minWalkingDistance < allowedRange, $"Minimum walking distance {minWalkingDistance} is bigger than the allowed {name}: {allowedRange}- character will never walk");
	    }
	}
}
