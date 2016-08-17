using AGS.API;

namespace AGS.Engine
{
    public class AGSPixelPerfectCollidable : IPixelPerfectCollidable
    {
        private IAnimationContainer _animation;
        private bool _pixelPerfect;

        public AGSPixelPerfectCollidable(IAnimationContainer animation)
        {
            _animation = animation;
            _animation.OnAnimationStarted.Subscribe((sender, args) => PixelPerfect(_pixelPerfect));
        }

        public IArea PixelPerfectHitTestArea
        {
            get
            {
                if (_animation.Animation == null || _animation.Animation.Sprite == null) return null;
                return _animation.Animation.Sprite.PixelPerfectHitTestArea;
            }
        }        

        public void PixelPerfect(bool pixelPerfect)
        {
            _pixelPerfect = pixelPerfect;
            if (_animation.Animation == null || _animation.Animation.Frames == null) return;
            foreach (var frame in _animation.Animation.Frames)
            {
                frame.Sprite.PixelPerfect(pixelPerfect);
            }
        }
    }
}
