using System;
using System.Threading.Tasks;

namespace API
{
	public interface ICharacter : IObject
	{
		IInventory Inventory { get; set; }
		IDirectionalAnimation WalkAnimation { get; set; }
		IDirectionalAnimation IdleAnimation { get; set; }
		ITextConfig SpeechTextConfig { get; set; }
		int WalkSpeed { get; set; }

		bool DebugDrawWalkPath { get; set; }

		void Say(string text);
		Task SayAsync(string text);

		bool Walk(ILocation location);
		Task<bool> WalkAsync(ILocation location);

		void PlaceOnWalkableArea();
	}
}

