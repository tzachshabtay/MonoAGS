using System;
using AGS.API;

namespace DemoGame
{
	public static class UIControls
	{
		public static void OnMouseClick(this IButton button, Action action, IGame game)
		{
			subscribeButton(button, action);
			game.Events.OnSavedGameLoad.Subscribe((sender, args) => subscribeButton(game.Find<IButton>(button.ID), action));
		}

		public static void OnValueChanged(this ISlider slider, Action<float> action, IGame game)
		{
			subscribeSlider(slider, action);
			game.Events.OnSavedGameLoad.Subscribe((sender, args) => subscribeSlider(game.Find<ISlider>(slider.ID), action));
		}

		private static void subscribeButton(IButton button, Action action)
		{
			button.Events.MouseClicked.Subscribe((sender, args) => action());
		}

		private static void subscribeSlider(ISlider slider, Action<float> action)
		{
			slider.OnValueChanged.Subscribe((sender, args) => action(args.Value));
		}
	}
}

