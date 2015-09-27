using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IWalkBehavior
	{
		int WalkSpeed { get; set; }
		bool IsWalking { get; }

		bool DebugDrawWalkPath { get; set; }

		bool Walk(ILocation location);
		Task<bool> WalkAsync(ILocation location);
		void StopWalking();
		Task StopWalkingAsync();

		void PlaceOnWalkableArea();
	}
}

