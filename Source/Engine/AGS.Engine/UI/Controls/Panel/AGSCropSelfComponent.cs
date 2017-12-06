using System;
using AGS.API;

namespace AGS.Engine
{ 
    public class AGSCropSelfComponent : AGSComponent, ICropSelfComponent
    {
        private bool _cropEnabled;
        private IImageComponent _image;
        private IEntity _entity;

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

        public RectangleF CropArea { get; set; }

        public IEvent OnCropAreaChanged { get; private set; }

        public IEvent<BeforeCropEventArgs> OnBeforeCrop { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
        }

        public FourCorners<Vector2> GetCropArea(BeforeCropEventArgs eventArgs, float spriteWidth, float spriteHeight, out float width, out float height)
        {
            width = spriteWidth;
            height = spriteHeight;
            OnBeforeCrop.Invoke(eventArgs);
            if (!CropEnabled) return null;
            float cropX = CropArea.X;
            float cropY = CropArea.Y;
            width = Math.Min(width, CropArea.Width);
            if (width <= 0f) return null;
            if (cropX + width > spriteWidth) width = spriteWidth - cropX;
            height = Math.Min(height, CropArea.Height);
            if (height <= 0f) return null;
            if (height - cropY <= 0f) return null;
            float left = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, cropX);
            float right = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, cropX + width);
			float top = MathUtils.Lerp(0f, 1f, spriteHeight, 0f, cropY + height);
            float bottom = MathUtils.Lerp(0f, 1f, spriteHeight, 0f, cropY);

            var textureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                new Vector2(left, top), new Vector2(right, top));
            return textureBox;
        }
    }
}
