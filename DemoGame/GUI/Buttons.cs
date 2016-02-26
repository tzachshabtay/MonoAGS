using System;
using AGS.API;

namespace DemoGame
{
	public static class Buttons
	{
		public static void OnMouseClick(this IButton button, Action action, IGame game)
		{
			subscribeButton(button, action);
			game.Events.OnSavedGameLoad.Subscribe((sender, args) => subscribeButton(game.Find<IButton>(button.ID), action));
		}

		private static void subscribeButton(IButton button, Action action)
		{
			button.Events.MouseClicked.Subscribe((sender, args) => action());
		}
	}
}

