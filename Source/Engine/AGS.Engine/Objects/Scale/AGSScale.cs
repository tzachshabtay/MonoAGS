﻿using AGS.API;
using System;
using System.ComponentModel;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSScale : IScale
    {
        private float _scaleX, _scaleY;
        private SizeF _baseSize;

        private static readonly PropertyChangedEventArgs _scaleXArgs = new PropertyChangedEventArgs(nameof(ScaleX));
        private static readonly PropertyChangedEventArgs _scaleYArgs = new PropertyChangedEventArgs(nameof(ScaleY));
        private static readonly PropertyChangedEventArgs _scaleArgs = new PropertyChangedEventArgs(nameof(Scale));
        private static readonly PropertyChangedEventArgs _baseSizeArgs = new PropertyChangedEventArgs(nameof(BaseSize));

        public AGSScale()
        { 
            _scaleX = 1;
            _scaleY = 1;
        }

        public static Action BindSizeToImage(IHasImage image, IScale scale)
        {
            PropertyChangedEventHandler onPropertyChanged = ((_, args) =>
            {
                if (args.PropertyName != nameof(IHasImage.Image)) return;
                if (MathUtils.FloatEquals(scale.BaseSize.Width, 0f) && image.Image != null) 
                    scale.BaseSize = new SizeF(image.Image.Width, image.Image.Height);
            });
            image.PropertyChanged += onPropertyChanged;
            return () => image.PropertyChanged -= onPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float Height { get; private set; }

        public float Width { get; private set; }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Scale))]
        public float ScaleX 
        {
            get => _scaleX;
            set => scaleBy(value, ScaleY);
        }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Scale))]
        public float ScaleY
        {
            get => _scaleY;
            set => scaleBy(ScaleX, value);
        }

        [AlsoNotifyFor(nameof(ScaleX), nameof(ScaleY))]
        public PointF Scale
        {
            get => new PointF(_scaleX, _scaleY);
            set => scaleBy(value.X, value.Y);
        }

        public SizeF BaseSize
        {
            get => _baseSize;
            set
            {
                float width = value.Width * ScaleX;
                float height = value.Height * ScaleY;
                if (!MathUtils.FloatEquals(width, Width))
                    Width = width;
                if (!MathUtils.FloatEquals(height, Height))
                    Height = height;
                _baseSize = new SizeF(value.Width, value.Height);
            }
        }

        public void ResetScale()
        {
            if (MathUtils.FloatEquals(Width, BaseSize.Width) && MathUtils.FloatEquals(Height, BaseSize.Height) &&
                MathUtils.FloatEquals(ScaleX, 1f) && MathUtils.FloatEquals(ScaleY, 1f)) return;
            Width = BaseSize.Width;
            Height = BaseSize.Height;
            _scaleX = 1f;
            _scaleY = 1f;
            OnPropertyChanged(_scaleXArgs);
            OnPropertyChanged(_scaleYArgs);
            OnPropertyChanged(_scaleArgs);
        }

        public void ResetScale(float initialWidth, float initialHeight)
        {
            if (MathUtils.FloatEquals(BaseSize.Width, initialWidth) && MathUtils.FloatEquals(BaseSize.Height, initialHeight) && 
                MathUtils.FloatEquals(ScaleX, 1f) && MathUtils.FloatEquals(ScaleY, 1f)) return;
            _baseSize = new SizeF(initialWidth, initialHeight);
            OnPropertyChanged(_baseSizeArgs);
            ResetScale();
        }

        public void ScaleTo(float width, float height)
        {
            if (MathUtils.FloatEquals(Width, width) && MathUtils.FloatEquals(Height, height)) return;
            validateScaleInitialized();
            Width = width;
            Height = height;
            _scaleX = Width / BaseSize.Width;
            _scaleY = Height / BaseSize.Height;
            OnPropertyChanged(_scaleXArgs);
            OnPropertyChanged(_scaleYArgs);
            OnPropertyChanged(_scaleArgs);
        }

        public void FlipHorizontally()
        {
            Scale = new PointF(-ScaleX, ScaleY);
        }

        public void FlipVertically()
        {
            Scale = new PointF(ScaleX, -ScaleY);
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        private void validateScaleInitialized()
        {
            if (MathUtils.FloatEquals(BaseSize.Width, 0f))
            {
                throw new InvalidOperationException(
                    "Initial size was not set. Either assign an animation/image to the object, or set BaseSize.");
            }
        }

        private void scaleBy(float scaleX, float scaleY)
        {
            validateScaleInitialized();
            _scaleX = scaleX;
            _scaleY = scaleY;
            Width = BaseSize.Width * ScaleX;
            Height = BaseSize.Height * ScaleY;
        }
    }
}
