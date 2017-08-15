using System;
using AGS.API;

namespace AGS.Engine
{ 
    public class AGSCropSelfComponent : AGSComponent, ICropSelfComponent
    {
        private RectangleF _cropArea;
        private bool _cropEnabled;

        public AGSCropSelfComponent()
        {
            OnCropAreaChanged = new AGSEvent();
            OnBeforeCrop = new AGSEvent<BeforeCropEventArgs>();
            _cropEnabled = true;
        }

        public bool CropEnabled 
        { 
            get { return _cropEnabled; }
            set 
            {
                if (_cropEnabled == value) return;
                _cropEnabled = value;
                OnCropAreaChanged.Invoke();
            }
        }

        public RectangleF CropArea
        {
            get { return _cropArea; }
            set
            {
                if (_cropArea.Equals(value)) return;
                _cropArea = value;
                OnCropAreaChanged.Invoke();
            }
        }

        public IEvent OnCropAreaChanged { get; private set; }

        public IEvent<BeforeCropEventArgs> OnBeforeCrop { get; private set; }

        public FourCorners<Vector2> GetCropArea(BeforeCropEventArgs eventArgs, float spriteWidth, float spriteHeight, out float width, out float height)
        {
            width = spriteWidth;
            height = spriteHeight;
            OnBeforeCrop.Invoke(eventArgs);
            if (!CropEnabled) return null;
            width = Math.Min(width, CropArea.Width);
            if (width <= 0f) return null;
            if (CropArea.X + width > spriteWidth) width = spriteWidth - CropArea.X;
            height = Math.Min(height, CropArea.Height);
            if (height <= 0f) return null;
            if (CropArea.Y + height > spriteHeight) height = spriteHeight - CropArea.Y;
            float left = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, CropArea.X);
            float right = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, CropArea.X + width);
            float top = MathUtils.Lerp(0f, 0f, spriteHeight, 1f, CropArea.Y);
            float bottom = MathUtils.Lerp(0f, 0f, spriteHeight, 1f, CropArea.Y + height);
            var textureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                new Vector2(left, top), new Vector2(right, top));
            return textureBox;
        }
    }
}
