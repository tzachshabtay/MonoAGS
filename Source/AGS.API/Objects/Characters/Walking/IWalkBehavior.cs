using System.Threading.Tasks;

namespace AGS.API
{
	public interface IWalkBehavior : IComponent
	{
		float WalkSpeed { get; set; }
        bool AdjustWalkSpeedToScaleArea { get; set; }
		bool IsWalking { get; }
        /// <summary>
        /// Gets the destination the character is currently walking to (if currently walking),
        /// or the last destination the character was walking to (if currently not walking).
        /// </summary>
        /// <value>The walk destination.</value>
        ILocation WalkDestination { get; }

		bool DebugDrawWalkPath { get; set; }

		bool Walk(ILocation location);
		Task<bool> WalkAsync(ILocation location);
		void StopWalking();
		Task StopWalkingAsync();

		void PlaceOnWalkableArea();
	}
}

