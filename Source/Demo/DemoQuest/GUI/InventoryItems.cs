﻿using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class InventoryItems
	{
		private const string _baseFolder = "Inventory/";

		public static IInventoryItem Bottle { get; private set; }
		public static IInventoryItem VoodooDoll { get; private set; }
		public static IInventoryItem Poster { get; private set; }
		public static IInventoryItem Manual { get; private set; }

		public async Task LoadAsync(IGameFactory factory)
		{
            AGSLoadImageConfig loadConfig = new AGSLoadImageConfig(new Point(0, 0));
			Bottle = await factory.Inventory.GetInventoryItemAsync("Bottle", "Rooms/EmptyStreet/bottle.bmp", null, loadConfig);
			VoodooDoll = await factory.Inventory.GetInventoryItemAsync("Voodoo Doll", _baseFolder + "voodooDoll.bmp", null, loadConfig, true);
			Poster = await factory.Inventory.GetInventoryItemAsync("Poster", _baseFolder + "poster.bmp", playerStartsWithItem: true);
			Manual = await factory.Inventory.GetInventoryItemAsync("Manual", _baseFolder + "manual.bmp", null, loadConfig, true);

            Poster.OnCombination(Manual).SubscribeToAsync(onPutPosterInManual);
		}

        private async Task onPutPosterInManual(InventoryCombinationEventArgs args)
        {
            await Characters.Cris.SayAsync("I don't see a reason to put the poster in the manual.");
        }
    }
}
