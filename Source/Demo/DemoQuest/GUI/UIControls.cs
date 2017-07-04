using System;
using AGS.API;

namespace DemoGame
{
	public static class UIControls
	{
		public static void OnMouseClick(this IButton button, Action action, IGame game)
		{
			subscribeButton(button, action);
			game.Events.OnSavedGameLoad.Subscribe(_ => subscribeButton(game.Find<IButton>(button.ID), action));
		}

		public static void OnValueChanged(this ISlider slider, Action<float> action, IGame game)
		{
			subscribeSlider(slider, action);
			game.Events.OnSavedGameLoad.Subscribe(_ => subscribeSlider(game.Find<ISlider>(slider.ID), action));
		}

		private static void subscribeButton(IButton button, Action action)
		{
			button.MouseClicked.Subscribe(_ => action());
		}

		private static void subscribeSlider(ISlider slider, Action<float> action)
		{
			slider.OnValueChanged.Subscribe(args => action(args.Value));
		}
	}
}

