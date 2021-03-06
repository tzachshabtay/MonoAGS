﻿using AGS.API;
using System.Threading.Tasks;
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
            _gameEvents.OnSavedGameLoad.Subscribe(() => onSaveGameLoaded());
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
            _game.Events.DefaultInteractions.OnInventoryCombination.SubscribeToAsync(onInventoryCombination);
		}

		private async Task onLook(ObjectEventArgs args)
		{
            await Repeat.RotateAsync("Look Default", 
                                     () => sayAsync("&1 It looks nice."), 
                                     () => sayAsync("&2Nothing to see here."),
                                     () => sayAsync("&3 I guess it looks ok."));
		}



		private async Task onInteract(ObjectEventArgs args)
		{
            await Repeat.RotateAsync("Interact Default",
                                     () => sayAsync("I can't do that."), 
                                     () => sayAsync("Nope."),
                                     () => sayAsync("I don't think so."));

		}

		private Task onInventoryInteract(InventoryInteractEventArgs args)
		{
            return onInteract(args);
		}

		private async Task onInventoryCombination(InventoryCombinationEventArgs args)
		{
			if (string.IsNullOrEmpty(args.ActiveItem.Graphics.DisplayName) ||
			    string.IsNullOrEmpty(args.PassiveItem.Graphics.DisplayName))
			{
				await _game.State.Player.SayAsync("I don't think these two items go together.");
				return;
			}

			await _game.State.Player.SayAsync($"Use {args.ActiveItem.Graphics.DisplayName} on {args.PassiveItem.Graphics.DisplayName}? I don't get it...");
		}

        private Task sayAsync(string text)
        {
            return _game.State.Player.SayAsync(text);
        }
	}
}