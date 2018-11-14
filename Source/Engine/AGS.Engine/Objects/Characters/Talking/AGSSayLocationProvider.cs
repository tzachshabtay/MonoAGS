using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSSayLocationProvider : ISayLocationProvider
    {
        private readonly IObject _obj;
        private readonly IGameSettings _settings;
        private readonly IGameState _state;

        private static bool _lastSpeakerOnLeft;
        private static IObject _lastSpeaker;

        private static readonly PointF _emptyPoint = new PointF(0f, 0f);

        public AGSSayLocationProvider(IGameSettings settings, IGameState state, IObject obj)
        {
            _settings = settings;
            _obj = obj;
            _state = state;
        }

        #region ISayLocation implementation

        public ISayLocation GetLocation(string text, ISayConfig config)
        {
            var portraitLocation = getPortraitLocation(config);
            _lastSpeaker = _obj;
            var boundingBox = _obj.WorldBoundingBox;
            float x = portraitLocation == null ? (getCharacterX(boundingBox, config.TextConfig.Alignment) - (_obj.IgnoreViewport ? 0 : _state.Viewport.X))
                                                  : portraitLocation.Value.X;
            if (portraitLocation != null && _lastSpeakerOnLeft)
                x += config.PortraitConfig.Portrait.Width + getBorderWidth(config.PortraitConfig, true).X;
            float y = portraitLocation == null ? (boundingBox.MaxY - (_obj.IgnoreViewport ? 0 : _state.Viewport.Y))
                                                  : _settings.VirtualResolution.Height;
            return new AGSSayLocation(getTextLocation(text, config, x, y), portraitLocation);
        }

        #endregion

        private float getCharacterX(AGSBoundingBox boundingBox, Alignment alignment)
        {
            //This might seem unintuitive -> if the text is left aligned, we use the right-most character x as the origin (and vice versa),
            //with the (hopefully correct) thought that it's what the user is after: left aligned for left-to-right languages, and right aligned for right-to-left languages.
            switch (alignment)
            {
                case Alignment.BottomCenter:
                case Alignment.MiddleCenter:
                case Alignment.TopCenter:
                    return (boundingBox.MinX + boundingBox.MaxX) / 2f;
                case Alignment.BottomLeft:
                case Alignment.MiddleLeft:
                case Alignment.TopLeft:
                    return boundingBox.MaxX;
                case Alignment.BottomRight:
                case Alignment.MiddleRight:
                case Alignment.TopRight:
                    return boundingBox.MinX;
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }

        private float getTextXOffset(float labelWidth, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.BottomCenter:
                case Alignment.MiddleCenter:
                case Alignment.TopCenter:
                    return -(labelWidth / 2f);
                case Alignment.BottomLeft:
                case Alignment.MiddleLeft:
                case Alignment.TopLeft:
                    return 0f;
                case Alignment.BottomRight:
                case Alignment.MiddleRight:
                case Alignment.TopRight:
                    return -labelWidth;
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }

        private float clampX(float x, Alignment alignment, float horizontalGap, float maxX, float width)
        {
            const float padding = 15f;
            switch (alignment)
            {
                case Alignment.BottomCenter:
                case Alignment.MiddleCenter:
                case Alignment.TopCenter:
                    return MathUtils.Clamp(x, -horizontalGap + padding, Math.Max(-horizontalGap + padding, maxX - width - horizontalGap - padding));
                case Alignment.BottomLeft:
                case Alignment.MiddleLeft:
                case Alignment.TopLeft:
                    return MathUtils.Clamp(x, padding, Math.Max(padding, maxX - width - padding));
                case Alignment.BottomRight:
                case Alignment.MiddleRight:
                case Alignment.TopRight:
                    return MathUtils.Clamp(x, -(horizontalGap * 2f) + padding, Math.Max(-(horizontalGap * 2f) + padding, maxX - width - (horizontalGap * 2f) - padding));
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }

        private PointF getTextLocation(string text, ISayConfig config, float x, float y)
        {
            SizeF labelSize = (config.LabelSize.Width - config.TextConfig.PaddingLeft - config.TextConfig.PaddingRight,
                               config.LabelSize.Height - config.TextConfig.PaddingTop - config.TextConfig.PaddingBottom);
            AGS.API.SizeF size = config.TextConfig.GetTextSize(text, labelSize);
            float width = size.Width + config.TextConfig.PaddingLeft + config.TextConfig.PaddingRight;
            float height = size.Height + config.TextConfig.PaddingTop + config.TextConfig.PaddingBottom;
            float horizontalGap = (config.LabelSize.Width - width)/2f;

            y = MathUtils.Clamp(y, 0f, Math.Min(_settings.VirtualResolution.Height,
                                _settings.VirtualResolution.Height - height));
            y -= config.PortraitConfig == null ? 0f : config.PortraitConfig.TextOffset.Y;
            y = MathUtils.Clamp(y, 0f, Math.Min(_settings.VirtualResolution.Height,
                                _settings.VirtualResolution.Height - height));
            y += config.TextOffset.Y;

            float rightPortraitX = _settings.VirtualResolution.Width - 100;
            x += config.PortraitConfig == null ? getTextXOffset(config.LabelSize.Width, config.TextConfig.Alignment) : (x > rightPortraitX ? -config.PortraitConfig.TextOffset.X
                                                       : config.PortraitConfig.TextOffset.X);
            float maxX = y > _settings.VirtualResolution.Height - 100 && x > rightPortraitX ? 
                                  Math.Min(x, _settings.VirtualResolution.Width) : _settings.VirtualResolution.Width;
            x += config.TextOffset.X;
            x = clampX(x, config.TextConfig.Alignment, horizontalGap, maxX, width);

            return new PointF(x, y);
        }

        private PointF? getPortraitLocation(ISayConfig config)
        {
            var portraitConfig = config.PortraitConfig;
            if (portraitConfig == null) return null;
            var portrait = portraitConfig.Portrait;
            if (portrait == null) return null;
            switch (portraitConfig.Positioning)
            {
                case PortraitPositioning.Custom: return null;
                case PortraitPositioning.Alternating:
                    portrait.Pivot = new PointF(0f, 0f);
                    if (_obj != _lastSpeaker)
                    {
                        _lastSpeakerOnLeft = !_lastSpeakerOnLeft;
                    }
                    return getPortraitLocation(portraitConfig);
                case PortraitPositioning.SpeakerPosition:
                    portrait.Pivot = new PointF(0f, 0f);
                    if (_obj.X < _settings.VirtualResolution.Width / 2) _lastSpeakerOnLeft = true;
                    else _lastSpeakerOnLeft = false;
                    return getPortraitLocation(portraitConfig);
                default: throw new NotSupportedException(portraitConfig.Positioning.ToString());
            }
        }

        private PointF getPortraitLocation(IPortraitConfig portraitConfig)
        { 
            return _lastSpeakerOnLeft ? getLeftPortrait(portraitConfig) : getRightPortrait(portraitConfig);
        }

        private PointF getLeftPortrait(IPortraitConfig portraitConfig)
        {
            var border = getBorderWidth(portraitConfig, true);
            return new PointF(portraitConfig.PortraitOffset.X + border.X, 
                              _settings.VirtualResolution.Height - portraitConfig.Portrait.Height - 
                              portraitConfig.PortraitOffset.Y - border.Y);
        }

        private PointF getRightPortrait(IPortraitConfig portraitConfig)
        {
            var border = getBorderWidth(portraitConfig, false);
            return new PointF(_settings.VirtualResolution.Width - portraitConfig.Portrait.Width - 
                              portraitConfig.PortraitOffset.X - border.X,
                              _settings.VirtualResolution.Height - portraitConfig.Portrait.Height - 
                              portraitConfig.PortraitOffset.Y - border.Y);
        }

        private PointF getBorderWidth(IPortraitConfig portraitConfig, bool left)
        {
            var border = portraitConfig.Portrait.Border;
            if (border == null) return _emptyPoint;
            float x = left ? border.WidthLeft : border.WidthRight;
            float y = border.WidthTop;
            return new PointF(x, y);
        }
	}
}