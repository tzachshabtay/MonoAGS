using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSPixelPerfectCollidable : IPixelPerfectCollidable
    {
        private IImageComponent _image;
        private IAnimationComponent _animation;
        private bool _pixelPerfect;

        public AGSPixelPerfectCollidable(IImageComponent spriteRender)
        {
            _image = spriteRender;
            _image.PropertyChanged += onProviderChanged;
            updateProvider();
        }

        public IArea PixelPerfectHitTestArea
        {
            get
            {
                return _image?.CurrentSprite?.PixelPerfectHitTestArea;
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
            else if (_image.CurrentSprite != null)
            {
                _image.CurrentSprite.PixelPerfect(pixelPerfect);
            }
        }

        public void Dispose()
        {
            if (_animation != null)
                _animation.OnAnimationStarted.Unsubscribe(refreshPixelPerfect);
            _image.PropertyChanged -= onProviderChanged;
        }

        private void refreshPixelPerfect()
        {
            PixelPerfect(_pixelPerfect);
        }

        private void onProviderChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IImageComponent.SpriteProvider))
                updateProvider();
        }

        private void updateProvider()
        {
            if (_image.SpriteProvider != null &&
                _image.SpriteProvider is IAnimationComponent animation)
            {
                _animation = animation;
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
