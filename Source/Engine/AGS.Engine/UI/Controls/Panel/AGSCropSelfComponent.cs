﻿using System;
using AGS.API;

namespace AGS.Engine
{ 
    public class AGSCropSelfComponent : AGSComponent, ICropSelfComponent
    {
        private RectangleF _cropArea;

        public AGSCropSelfComponent()
        {
            OnCropAreaChanged = new AGSEvent<object>();
        }

        public RectangleF CropArea
        {
            get { return _cropArea; }
            set
            {
                _cropArea = value;
                OnCropAreaChanged.Invoke(null);
            }
        }

        public IEvent<object> OnCropAreaChanged { get; private set; }

        public static FourCorners<Vector2> GetCropArea(ICropSelfComponent crop, float spriteWidth, float spriteHeight, out float width, out float height)
        {
            width = spriteWidth;
            height = spriteHeight;
            if (crop == null) return null;
            width = Math.Min(width, crop.CropArea.Width);
            if (crop.CropArea.X + width > spriteWidth) width = spriteWidth - crop.CropArea.X;
            height = Math.Min(height, crop.CropArea.Height);
            if (crop.CropArea.Y + height > spriteHeight) height = spriteHeight - crop.CropArea.Y;
            float left = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, crop.CropArea.X);
            float right = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, crop.CropArea.X + width);
            float top = MathUtils.Lerp(0f, 0f, spriteHeight, 1f, crop.CropArea.Y);
            float bottom = MathUtils.Lerp(0f, 0f, spriteHeight, 1f, crop.CropArea.Y + height);
            var textureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                new Vector2(left, top), new Vector2(right, top));
            return textureBox;
        }
    }
}
