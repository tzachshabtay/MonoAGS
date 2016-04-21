using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DemoGame
{
	public class DefaultInteractions
	{
		private IGame _game;
		private IGameEvents _gameEvents;

		private List<string> _looks, _interacts, _inventoryInteracts, _customInteracts;
		private int _looksIndex, _interactsIndex, _inventoryIndex, _customIndex;

		public DefaultInteractions(IGame game, IGameEvents gameEvents)
		{
			_game = game;
			_gameEvents = gameEvents;

			_looks = new List<string> { "It looks nice.", "Nothing to see here.", "I guess it looks ok." };
			_interacts = new List<string> { "I can't do that.", "Nope.", "I don't think so." };
			_inventoryInteracts = _interacts;
			_customInteracts = _interacts;
		}

		public void Load()
		{
			_gameEvents.OnSavedGameLoad.Subscribe((sender, e) => onSaveGameLoaded());
			_gameEvents.DefaultInteractions.OnLook.SubscribeToAsync(onLook);
			_gameEvents.DefaultInteractions.OnInteract.SubscribeToAsync(onInteract);
			_gameEvents.DefaultInteractions.OnCustomInteract.SubscribeToAsync(onCustomInteract);
			_gameEvents.DefaultInteractions.OnInventoryInteract.SubscribeToAsync(onInventoryInteract);
			subscribeDefaultInventoryCombination();
		}

		private void onSaveGameLoaded()
		{
			subscribeDefaultInventoryCombination();
		}

		private void subscribeDefaultInventoryCombination()
		{
			_game.State.Player.Character.Inventory.OnDefaultCombination.SubscribeToAsync(onInventoryCombination);
		}

		private async Task onLook(object sender, ObjectEventArgs args)
		{
			_looksIndex = await sayDefault(_looks, _looksIndex);
		}

		private async Task onInteract(object sender, ObjectEventArgs args)
		{
			_interactsIndex = await sayDefault(_interacts, _interactsIndex);
		}

		private async Task onCustomInteract(object sender, CustomInteractionEventArgs args)
		{
			_customIndex = await sayDefault(_customInteracts, _customIndex);
		}

		private async Task onInventoryInteract(object sender, InventoryInteractEventArgs args)
		{
			_inventoryIndex = await sayDefault(_inventoryInteracts, _inventoryIndex);
		}

		private async Task onInventoryCombination(object sender, InventoryCombinationEventArgs args)
		{
			if (string.IsNullOrEmpty(args.ActiveItem.Graphics.Hotspot) ||
			    string.IsNullOrEmpty(args.PassiveItem.Graphics.Hotspot))
			{
				await _game.State.Player.Character.SayAsync("I don't think these two items go together.");
				return;
			}

			await _game.State.Player.Character.SayAsync(string.Format("Use {0} on {1}? I don't get it...",
				args.ActiveItem.Graphics.Hotspot, args.PassiveItem.Graphics.Hotspot));
		}

		private async Task<int> sayDefault(List<string> list, int index)
		{
			string sentence = list[index];
			index = (index + 1) % list.Count;

			await _game.State.Player.Character.SayAsync(sentence);

			return index;
		}
	}
}

