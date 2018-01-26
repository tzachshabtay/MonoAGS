using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSPixelPerfectCollidable : IPixelPerfectCollidable
    {
        private ISpriteRenderComponent _spriteRender;
        private IAnimationComponent _animation;
        private bool _pixelPerfect;

        public AGSPixelPerfectCollidable(ISpriteRenderComponent spriteRender)
        {
            _spriteRender = spriteRender;
            _spriteRender.PropertyChanged += onProviderChanged;
            updateProvider();
        }

        public IArea PixelPerfectHitTestArea
        {
            get
            {
                return _spriteRender?.CurrentSprite?.PixelPerfectHitTestArea;
            }
        }        

        public void PixelPerfect(bool pixelPerfect)
        {
            _pixelPerfect = pixelPerfect;
            if (_animation != null)
            {
                // Special case if the sprite provider is animation, where we need to update all frames
                // TODO: may there be a way to implement an abstract approach here to let do the same
                // kind of update to any hypothetical custom component?
                // Or, event better, do not have the pixel-perfect switch in the sprites at all.
                if (_animation.Animation == null || _animation.Animation.Frames == null)
                    return;
                foreach (var frame in _animation.Animation.Frames)
                {
                    frame.Sprite.PixelPerfect(pixelPerfect);
                }
            }
            else if (_spriteRender.CurrentSprite != null)
            {
                _spriteRender.CurrentSprite.PixelPerfect(pixelPerfect);
            }
        }

        public void Dispose()
        {
            if (_animation != null)
                _animation.OnAnimationStarted.Unsubscribe(refreshPixelPerfect);
            _spriteRender.PropertyChanged -= onProviderChanged;
        }

        private void refreshPixelPerfect()
        {
            PixelPerfect(_pixelPerfect);
        }

        private void onProviderChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISpriteRenderComponent.SpriteProvider))
                updateProvider();
        }

        private void updateProvider()
        {
            if (_spriteRender.SpriteProvider != null && _spriteRender.SpriteProvider is IAnimationComponent)
            {
                _animation = _spriteRender.SpriteProvider as IAnimationComponent;
                _animation.OnAnimationStarted.Subscribe(refreshPixelPerfect);
            }
            else
            {
                if (_animation != null)
                    _animation.OnAnimationStarted.Unsubscribe(refreshPixelPerfect);
                _animation = null;
            }
            refreshPixelPerfect();
        }
    }
}
