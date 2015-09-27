using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface ICharacter : IObject, ISayBehavior
	{
		IInventory Inventory { get; set; }
		IOutfit Outfit { get; set; }

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

