using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using AGS.Engine;

namespace DemoGame
{
	public class DefaultInteractions
	{
		private IGame _game;
		private IGameEvents _gameEvents;

		public DefaultInteractions(IGame game, IGameEvents gameEvents)
		{
			_game = game;
			_gameEvents = gameEvents;
		}

		public void Load()
		{
			_gameEvents.OnSavedGameLoad.Subscribe((sender, e) => onSaveGameLoaded());
            _gameEvents.DefaultInteractions.OnInteract(AGSInteractions.LOOK).SubscribeToAsync(onLook);
            _gameEvents.DefaultInteractions.OnInteract(AGSInteractions.DEFAULT).SubscribeToAsync(onInteract);
            _gameEvents.DefaultInteractions.OnInventoryInteract(AGSInteractions.DEFAULT).SubscribeToAsync(onInventoryInteract);
			subscribeDefaultInventoryCombination();
		}

		private void onSaveGameLoaded()
		{
			subscribeDefaultInventoryCombination();
		}

		private void subscribeDefaultInventoryCombination()
		{
			_game.State.Player.Inventory.OnDefaultCombination.SubscribeToAsync(onInventoryCombination);
		}

		private async Task onLook(object sender, ObjectEventArgs args)
		{
            await Repeat.RotateAsync("Look Default", 
                                     () => sayAsync("&1 It looks nice."), 
                                     () => sayAsync("&2Nothing to see here."),
                                     () => sayAsync("&3 I guess it looks ok."));
		}



		private async Task onInteract(object sender, ObjectEventArgs args)
		{
            await Repeat.RotateAsync("Interact Default",
                                     () => sayAsync("I can't do that."), 
                                     () => sayAsync("Nope."),
                                     () => sayAsync("I don't think so."));

		}

		private Task onInventoryInteract(object sender, InventoryInteractEventArgs args)
		{
            return onInteract(sender, args);
		}

		private async Task onInventoryCombination(object sender, InventoryCombinationEventArgs args)
		{
			if (string.IsNullOrEmpty(args.ActiveItem.Graphics.Hotspot) ||
			    string.IsNullOrEmpty(args.PassiveItem.Graphics.Hotspot))
			{
				await _game.State.Player.SayAsync("I don't think these two items go together.");
				return;
			}

			await _game.State.Player.SayAsync(string.Format("Use {0} on {1}? I don't get it...",
				args.ActiveItem.Graphics.Hotspot, args.PassiveItem.Graphics.Hotspot));
		}

        private Task sayAsync(string text)
        {
            return _game.State.Player.SayAsync(text);
        }
	}
}

