using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class InventoryItems
	{
		private const string _baseFolder = "../../Assets/Inventory/";

		public static IInventoryItem Bottle { get; private set; }
		public static IInventoryItem VoodooDoll { get; private set; }
		public static IInventoryItem Poster { get; private set; }
		public static IInventoryItem Manual { get; private set; }

		public async Task LoadAsync(IGameFactory factory)
		{
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{
				TransparentColorSamplePoint = new AGS.API.Point(0,0),
			};
			Bottle = await factory.Inventory.GetInventoryItemAsync("Bottle", "../../Assets/Rooms/EmptyStreet/bottle.bmp", null, loadConfig);
			VoodooDoll = await factory.Inventory.GetInventoryItemAsync("Voodoo Doll", _baseFolder + "voodooDoll.bmp", null, loadConfig, true);
			Poster = await factory.Inventory.GetInventoryItemAsync("Poster", _baseFolder + "poster.bmp", playerStartsWithItem: true);
			Manual = await factory.Inventory.GetInventoryItemAsync("Manual", _baseFolder + "manual.bmp", null, loadConfig, true);
		}
	}
}

