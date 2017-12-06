using System.Collections.Generic;
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
        private bool _shouldAnimate;
        private int _rotationStage;
        private PlayerAsFeature _playerAsFeature;

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
            _label = factory.UI.GetLabel("MoveAreaLabel", "Try Walking!", 100f, 30f, 0f, _parent.Height - 30f, _parent);
            _label.RenderLayer = _parent.RenderLayer;

            var parent = factory.Object.GetObject("Elevator Parent");
            parent.TreeNode.SetParent(_parent.TreeNode);
            parent.RenderLayer = _parent.RenderLayer;
            parent.Image = new EmptyImage(1f, 1f);
            parent.Opacity = 0;
            parent.X = 200f;
            parent.Y = 100f;

			var areaParent = factory.Object.GetObject("Elevator Area Parent");
			areaParent.TreeNode.SetParent(parent.TreeNode);
			areaParent.RenderLayer = _parent.RenderLayer;

            _playerAsFeature.PlaceInFeatureWindow(parent);

            bool[,] maskArr = new bool[200, 200];
            var mask = factory.Masks.Load(maskArr, "Elevator Mask", true, Colors.GreenYellow.WithAlpha(150));
            var area = factory.Room.GetArea("Elevator Area", mask, true);
            area.Mask.DebugDraw.TreeNode.SetParent(areaParent.TreeNode);
            area.Mask.DebugDraw.RenderLayer = _parent.RenderLayer;
            var areaTranslate = area.AddComponent<ITranslateComponent>();
            var areaRotate = area.AddComponent<IRotateComponent>();
            player.Room.Areas.Add(area);
            player.PlaceOnWalkableArea();
            player.Z = area.Mask.DebugDraw.Z - 1;
            _scheme.CurrentMode = RotatingCursorScheme.WALK_MODE;

            animate(parent, areaTranslate, areaRotate);
        }

        public async Task Close()
        {
            _shouldAnimate = false;
            var label = _label;
            if (label != null)
            {
                _game.State.UI.Remove(label);
                label.Dispose();
            }

            var playerAsFeature = _playerAsFeature;
            if (playerAsFeature != null) await playerAsFeature.Restore();

            _scheme.CurrentMode = MouseCursors.POINT_MODE;
        }

        private async void animate(IObject parent, ITranslateComponent areaTranslate, IRotateComponent areaRotate)
        {
            if (!_shouldAnimate) return;

            _label.Text = "Try Walking! (Moving character and area together)";
			await parent.TweenY(200f, 2f).Task;
			await parent.TweenX(300f, 2f).Task;
			await parent.TweenY(100f, 2f).Task;
			await parent.TweenX(200f, 2f).Task;

            _label.Text = "Try Walking! (Moving area only)";
            await areaTranslate.TweenY(100f, 2f).Task;
			await areaTranslate.TweenX(100f, 2f).Task;
			await areaTranslate.TweenY(0f, 2f).Task;
			await areaTranslate.TweenX(0f, 2f).Task;

            switch (_rotationStage)
            {
                case 0:
                    _label.Text = "Try Walking! (Rotating character and area together)";
                    await parent.TweenAngle(45f, 2f).Task;
                    break;
                case 1:
                    _label.Text = "Try Walking! (Rotating character and area together)";
                    await parent.TweenAngle(0f, 2f).Task;
                    break;
                case 2:
                    _label.Text = "Try Walking! (Rotating area only)";
                    await areaRotate.TweenAngle(45f, 2f).Task;
                    break;
                case 3:
                    _label.Text = "Try Walking! (Rotating area only)";
                    await areaRotate.TweenAngle(0f, 2f).Task;
                    break;
            }
            _rotationStage = (_rotationStage + 1) % 4;

            animate(parent, areaTranslate, areaRotate);
        }
    }
}
