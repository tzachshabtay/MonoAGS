using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesMoveAreaPanel : IFeaturesPanel
    {
        private readonly IGame _game;
        private readonly IObject _parent;
        private readonly RotatingCursorScheme _scheme;

        private ILabel _label;
        private ICheckBox _checkbox;
        private IObject _elevator;
        private bool _shouldAnimate, _shouldBindAreaToCharacter;
        private int _rotationStage;
        private PlayerAsFeature _playerAsFeature;
        private IArea _area;

        private Tween _currentTween;
        private bool _stopped;

        public FeaturesMoveAreaPanel(IGame game, IObject parent, RotatingCursorScheme scheme)
        {
            _game = game;
            _parent = parent;
            _scheme = scheme;
        }

        public void Show()
        {
            _shouldAnimate = true;
            var player = _game.State.Player;
            _playerAsFeature = new PlayerAsFeature(player);

            var factory = _game.Factory;
            _label = factory.UI.GetLabel("MoveAreaLabel", "Try Walking!", 100f, 30f, 10f, _parent.Height - 30f, _parent);

            _checkbox = factory.UI.GetCheckBox("MoveAreaCheckbox", (ButtonAnimation)null, null, null, null, 10f, _parent.Height - 60f, _parent, "Bind Area & Character", width: 20f, height: 20f);
            _checkbox.Checked = true;
            _shouldBindAreaToCharacter = true;
            _checkbox.OnCheckChanged.Subscribe(onBindChanged);

            _elevator = factory.Object.GetObject("Elevator Parent");
            _elevator.TreeNode.SetParent(_parent.TreeNode);
            _elevator.RenderLayer = _parent.RenderLayer;
            _elevator.Image = new EmptyImage(1f, 1f);
            _elevator.Opacity = 0;
            _elevator.Position = (200f, 100f);

			var areaParent = factory.Object.GetObject("Elevator Area Parent");
            areaParent.TreeNode.SetParent(_elevator.TreeNode);
			areaParent.RenderLayer = _parent.RenderLayer;

            _playerAsFeature.PlaceInFeatureWindow(_elevator);

            bool[,] maskArr = new bool[200, 200];
            var mask = factory.Masks.Load(maskArr, "Elevator Mask", true, Colors.GreenYellow.WithAlpha(150));
            _area = factory.Room.GetArea("Elevator Area", mask, null, true);
            _area.Mask.DebugDraw.TreeNode.SetParent(areaParent.TreeNode);
            _area.Mask.DebugDraw.RenderLayer = _parent.RenderLayer;
            var areaTranslate = _area.AddComponent<ITranslateComponent>();
            var areaRotate = _area.AddComponent<IRotateComponent>();
            player.Room.Areas.Add(_area);
            player.PlaceOnWalkableArea();
            player.Z = _area.Mask.DebugDraw.Z - 1;
            _scheme.CurrentMode = RotatingCursorScheme.WALK_MODE;

            animate(_elevator, areaTranslate, areaRotate);
        }

        public async Task Close()
        {
            _shouldAnimate = false;
            stop();
            var label = _label;
            if (label != null)
            {
                _game.State.UI.Remove(label);
                label.Dispose();
            }

            var checkbox = _checkbox;
            if (checkbox != null)
            {
                label = checkbox.TextLabel;
                if (label != null)
                {
                    _game.State.UI.Remove(label);
                    label.Dispose();
                }
                _game.State.UI.Remove(checkbox);
                checkbox.Dispose();
            }

            var playerAsFeature = _playerAsFeature;
            if (playerAsFeature != null) await playerAsFeature.Restore();

            _elevator?.DestroyWithChildren();
            _area?.Dispose();

            _scheme.CurrentMode = MouseCursors.POINT_MODE;
        }

        private void onBindChanged(CheckBoxEventArgs args)
        {
            _shouldBindAreaToCharacter = args.Checked;
            stop();
        }

        private void stop()
        {
            _stopped = true;
            _game.State.Player.StopWalkingAsync();
            _currentTween?.Stop(TweenCompletion.Stay);
        }

        private async void animate(IObject parent, ITranslateComponent areaTranslate, IRotateComponent areaRotate)
        {
            if (!_shouldAnimate) return;

            if (_stopped)
            {
                parent.Position = (200f, 100f);
                parent.Angle = 0f;
                areaTranslate.X = 0f;
                areaTranslate.Y = 0f;
                areaRotate.Angle = 0f;
                _rotationStage = 0;
                _game.State.Player.Position = (50f, 30f);
                _stopped = false;
            }

            if (_shouldBindAreaToCharacter)
            {
                _label.Text = "Try Walking! (Moving character and area together)";
                if (await tween(() => parent.TweenY(200f, 2f)))
                if (await tween(() => parent.TweenX(300f, 2f)))
                if (await tween(() => parent.TweenY(100f, 2f)))
                    await tween(() => parent.TweenX(200f, 2f));
            }
            else
            {
                _label.Text = "Try Walking! (Moving area only)";
                if (await tween(() => areaTranslate.TweenY(100f, 2f)))
                if (await tween(() => areaTranslate.TweenX(100f, 2f)))
                if (await tween(() => areaTranslate.TweenY(0f, 2f)))
                    await tween(() => areaTranslate.TweenX(0f, 2f));
            }

            if (!_stopped)
            {
                var toRotate = _shouldBindAreaToCharacter ? parent : areaRotate;
                _label.Text = _shouldBindAreaToCharacter ? "Try Walking! (Rotating character and area together)" : "Try Walking! (Rotating area only)";
                switch (_rotationStage)
                {
                    case 0:
                        await tween(() => toRotate.TweenAngle(45f, 2f));
                        break;
                    case 1:
                        await tween(() => toRotate.TweenAngle(0f, 2f));
                        break;
                }
                _rotationStage = (_rotationStage + 1) % 2;
            }

            animate(parent, areaTranslate, areaRotate);
        }

        private async Task<bool> tween(Func<Tween> getTween)
        {
            if (_stopped) return false;
            _currentTween = getTween();
            await _currentTween.Task;
            return !_stopped;
        }
    }
}
