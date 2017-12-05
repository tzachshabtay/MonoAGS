using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesTexturesPanel : IFeaturesPanel
    {
		private readonly IGame _game;
		private readonly IObject _parent;
        private bool _isClosed;

        private ILabel _label;
        private PlayerAsFeature _playerAsFeature;

        public FeaturesTexturesPanel(IGame game, IObject parent)
        {
			_game = game;
			_parent = parent;
        }

        public void Show()
        {
            _isClosed = false;
			
            var player = _game.State.Player;
            _playerAsFeature = new PlayerAsFeature(player);

            var factory = _game.Factory;
			_label = factory.UI.GetLabel("TexturesPanelLabel", "", 100f, 30f, 0f, _parent.Height - 30f, _parent);
			_label.RenderLayer = _parent.RenderLayer;

			_playerAsFeature.PlaceInFeatureWindow(_parent);
            var animation = player.Outfit[AGSOutfit.Walk].Down;
            player.StartAnimation(animation);
            player.X = 150f;
            player.Y = 50f;
            player.Scale = new PointF(2f, 2f);

            foreach (var frame in animation.Frames)
            {
                frame.Sprite.BaseSize = new SizeF(200f, 200f);
            }
            var textureOffset = player.AddComponent<ITextureOffsetComponent>();
            animate(textureOffset);
        }

		public void Close()
		{
			var player = _game.State.Player;
            if (player != null)
            {
                player.RemoveComponent<ITextureOffsetComponent>();
                setConfig(player, ScaleUpFilters.Nearest, TextureWrap.Clamp, TextureWrap.Clamp);
                var animation = player.Outfit[AGSOutfit.Walk].Down;
                foreach (var frame in animation.Frames)
                {
                    frame.Sprite.BaseSize = new SizeF(frame.Sprite.Image.Width, frame.Sprite.Image.Height);
                }
                player.StartAnimation(player.Outfit[AGSOutfit.Idle].Down);
			}
			_isClosed = true;
			var playerAsFeature = _playerAsFeature;
            if (playerAsFeature != null) playerAsFeature.Restore();

			var label = _label;
			if (label != null)
			{
				_game.State.UI.Remove(label);
				label.Dispose();
			}
		}

        private async void animate(ITextureOffsetComponent textureOffset)
        {
            if (_isClosed) return;

            var player = _game.State.Player;
            setConfig(player, ScaleUpFilters.Nearest, TextureWrap.Repeat, TextureWrap.Repeat);

            await Task.Delay(2000);
            setConfig(player, ScaleUpFilters.Nearest, TextureWrap.Repeat, TextureWrap.MirroredRepeat);

			await Task.Delay(2000);
            setConfig(player, ScaleUpFilters.Nearest, TextureWrap.Clamp, TextureWrap.MirroredRepeat);

			await Task.Delay(2000);
            setConfig(player, ScaleUpFilters.Linear, TextureWrap.Clamp, TextureWrap.MirroredRepeat);

            await Task.Delay(2000);
            setConfig(player, ScaleUpFilters.Linear, TextureWrap.Repeat, TextureWrap.MirroredRepeat);

            _label.Text = "Animating texture offset";
            await textureOffset.TweenX(3f, 2f, Ease.Linear).Task;
            await textureOffset.TweenY(3f, 2f, Ease.Linear).Task;
            await Task.Delay(2000);

            var task1 = textureOffset.TweenX(0f, 2f, Ease.QuadIn).Task;
            var task2 = textureOffset.TweenY(0f, 2f, Ease.QuadIn).Task;
            await Task.WhenAll(task1, task2);

            animate(textureOffset);
        }

        private void setConfig(ICharacter player, ScaleUpFilters scaleUp, TextureWrap wrapX, TextureWrap wrapY)
        {
            if (_isClosed) return;
            var animation = player.Animation;
            if (animation == null) return;
            foreach (var frame in animation.Frames)
            {
                frame.Sprite.Image.Texture.Config = new AGSTextureConfig(ScaleDownFilters.Nearest, scaleUp, wrapX, wrapY);
            }
            _label.Text = string.Format("Scaling: {0}, Tiling: {1},{2}", scaleUp, wrapX, wrapY);
        }
	}
}
