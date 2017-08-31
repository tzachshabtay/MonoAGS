using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesViewportsPanel : IFeaturesPanel
    {
		private IGame _game;
		private IObject _parent;
        private bool _isShowing;
        private IViewport _viewport1, _viewport2;

        public FeaturesViewportsPanel(IGame game, IObject parent)
        {
			_game = game;
			_parent = parent;
        }

        public void Close()
        {
            _isShowing = false;
            _game.State.SecondaryViewports.Clear();
        }

        public async void Show()
        {
            _isShowing = true;
            AGSDisplayListSettings settings = new AGSDisplayListSettings 
            { 
                DisplayGUIs = false 
            };
            _viewport1 = new AGSViewport(settings, null);
            _viewport1.RoomProvider = new AGSSingleRoomProvider(await Rooms.BrokenCurbStreet);
            _viewport1.ProjectionBox = new RectangleF(0.15f, 0.07f, 0.25f, 0.25f);
            _viewport1.Parent = _parent;
            _game.State.SecondaryViewports.Add(_viewport1);

			_viewport2 = new AGSViewport(settings, null);
            _viewport2.RoomProvider = new AGSSingleRoomProvider(await Rooms.DarsStreet);
			_viewport2.ProjectionBox = new RectangleF(0.15f, 0.37f, 0.25f, 0.25f);
            _viewport2.Parent = _parent;
			_game.State.SecondaryViewports.Add(_viewport2);

            animate();
        }

        private async void animate()
        {
            await Task.Delay(2000);
            if (!_isShowing) return;

            var task1 = _viewport1.TweenProjectY(0.37f, 1f, Ease.QuadIn).Task;
            var task2 = _viewport2.TweenProjectY(0.07f, 1f, Ease.QuadIn).Task;
            await Task.WhenAll(task1, task2);
            await Task.Delay(1000);
            if (!_isShowing) return;

            task1 = _viewport1.TweenProjectWidth(0.1f, 1f, Ease.BounceIn).Task;
            task2 = _viewport2.TweenProjectWidth(0.1f, 1f, Ease.BounceIn).Task;
			await Task.WhenAll(task1, task2);
			await Task.Delay(1000);
			if (!_isShowing) return;

			task1 = _viewport1.TweenProjectY(0.07f, 1f, Ease.SineInOut).Task;
			task2 = _viewport2.TweenProjectY(0.37f, 1f, Ease.SineInOut).Task;
			var task3 = _viewport1.TweenProjectWidth(0.25f, 1.5f, Ease.SineInOut).Task;
			var task4 = _viewport2.TweenProjectWidth(0.25f, 1.5f, Ease.SineInOut).Task;
			await Task.WhenAll(task1, task2, task3, task4);
			await Task.Delay(1000);
			if (!_isShowing) return;

            task1 = _viewport1.TweenAngle(45f, 1f, Ease.ExpoOut).Task;
            task2 = _viewport2.TweenScaleX(2f, 1f, Ease.CircIn).Task;
            task3 = _viewport2.TweenScaleY(2f, 1f, Ease.CircIn).Task;
            await Task.WhenAll(task1, task2, task3);
            await Task.Delay(1000);
            if (!_isShowing) return;

			task1 = _viewport1.TweenAngle(0f, 1f, Ease.ExpoIn).Task;
			task2 = _viewport2.TweenScaleX(1f, 1f, Ease.CircOut).Task;
			task3 = _viewport2.TweenScaleY(1f, 1f, Ease.CircOut).Task;
			await Task.WhenAll(task1, task2, task3);

            animate();
        }
    }
}
