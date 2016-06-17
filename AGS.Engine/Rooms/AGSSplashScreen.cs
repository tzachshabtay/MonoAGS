using System;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class AGSSplashScreen
	{
		private IGame _game;
		private IRoom _splashScreen;
		private ILabel _label;

		public string LoadingText = "Loading";
		public int DotsDelay = 30;
		public ITextConfig TextConfig = getDefaultTextConfig ();

		public IRoom Load(IGame game)
		{
			_game = game;
			_label = game.Factory.UI.GetLabel ("Splash Label", LoadingText, 1f,
			                                   1f, game.VirtualResolution.Width/2f, 
			                                   game.VirtualResolution.Height / 2f,
											   TextConfig, false);
			_label.Anchor = new PointF(0.5f, 0.5f);
			_splashScreen = game.Factory.Room.GetRoom ("Splash Screen");
			_splashScreen.Objects.Add (_label);
			_splashScreen.ShowPlayer = false;

			tweenLabel ();
			return _splashScreen;
		}

		private static ITextConfig getDefaultTextConfig()
		{
			return new AGSTextConfig (brush: Hooks.BrushLoader.LoadSolidBrush (Colors.WhiteSmoke),
			                          alignment: Alignment.MiddleCenter, autoFit: AutoFit.LabelShouldFitText);
		}

		private async void tweenLabel()
		{
			if (_game.State.Player.Character != null &&
				_game.State.Player.Character.Room != _splashScreen && 
			   _game.State.Player.Character.Room != null) return;

			var tweenScaleX = _label.TweenScaleX (1.5f, 3f, Ease.BounceOut);
			var tweenScaleY = _label.TweenScaleY (1.5f, 3f, Ease.BounceOut);

			await tweenScaleX.Task;
			await tweenScaleY.Task;

			tweenScaleX = _label.TweenScaleX (1f, 3f, Ease.BounceOut);
			tweenScaleY = _label.TweenScaleY (1f, 3f, Ease.BounceOut);

			await tweenScaleX.Task;
			await tweenScaleY.Task;

			tweenLabel ();
		}
	}
}

