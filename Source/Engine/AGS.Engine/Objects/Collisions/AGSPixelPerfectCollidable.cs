using AGS.API;

namespace AGS.Engine
{
    public class AGSPixelPerfectCollidable : IPixelPerfectCollidable
    {
        private IHasImage _image;
        private IMaskLoader _maskLoader;

        public AGSPixelPerfectCollidable(IHasImage image, IMaskLoader maskLoader)
        {
            _image = image;
            _maskLoader = maskLoader;
        }

        public IArea PixelPerfectHitTestArea { get; private set; }
        public void PixelPerfect(bool pixelPerfect)
        {
            IArea area = PixelPerfectHitTestArea;
            if (!pixelPerfect)
            {
                if (area == null) return;
                area.Enabled = false;
                return;
            }
            if (area != null) return;

            PixelPerfectHitTestArea = new AGSArea { Mask = _maskLoader.Load(_image.Image.OriginalBitmap) };
        }
    }
}
