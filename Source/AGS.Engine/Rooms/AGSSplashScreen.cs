using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class AGSSplashScreen
	{
		private IGame _game;
		private IRoom _splashScreen;
		private ILabel _label;
		private Action _visitTween;
		private Stopwatch _stopwatch;

		public string LoadingText = "Loading";
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
			_stopwatch = new Stopwatch();
			_stopwatch.Start();
			onRepeatedlyExecute ();

			return _splashScreen;
		}

		private static ITextConfig getDefaultTextConfig()
		{
			return new AGSTextConfig (brush: Hooks.BrushLoader.LoadSolidBrush (Colors.WhiteSmoke),
			                          alignment: Alignment.MiddleCenter, autoFit: AutoFit.LabelShouldFitText);
		}

		private async void tweenLabel()
		{
			if (notInRoom ()) return;

			var tween = Tween.RunWithExternalVisit(_label.ScaleX, 1.5f, scale => _label.ScaleBy(scale,scale), 
			                                       3f, Ease.BounceOut, out _visitTween);

			await tween.Task;

			tween = Tween.RunWithExternalVisit (_label.ScaleX, 1f, scale => _label.ScaleBy (scale, scale),
												   3f, Ease.BounceOut, out _visitTween);

			await tween.Task;

			tweenLabel ();
		}

		private async void onRepeatedlyExecute()
		{
			if (notInRoom ()) return;

			//When we're loading assets, FPS is all over the place, so we need to compensate
			const int speed = 60;
			long elapsed = _stopwatch.ElapsedMilliseconds;
			if (elapsed > speed) 
			{
				int times = (int)elapsed / speed;

				for (int i = 0; i < times; i++) {
					_visitTween ();
				}

				_stopwatch.Restart ();
			}

			await Task.Delay (5);
			onRepeatedlyExecute ();
		}

		private bool notInRoom()
		{
			return (_game.State.Player.Character != null &&
				_game.State.Player.Character.Room != _splashScreen &&
					_game.State.Player.Character.Room != null);
		}
	}
}

