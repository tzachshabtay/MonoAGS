using System;
using AGS.API;


namespace AGS.Engine
{
	public class AGSSayLocationProvider : ISayLocationProvider
	{
		private IObject _obj;
        private IGameSettings _settings;

        private static bool _lastSpeakerOnLeft;
        private static IObject _lastSpeaker;

        private static readonly PointF _emptyPoint = new PointF(0f, 0f);

        public AGSSayLocationProvider(IGameSettings settings, IObject obj)
		{
            _settings = settings;
			_obj = obj;
		}

		#region ISayLocation implementation

        public ISayLocation GetLocation(string text, ISayConfig config)
		{
            var portraitLocation = getPortraitLocation(config);
            _lastSpeaker = _obj;
            float x = portraitLocation == null ? (_obj.BoundingBox.MaxX - (_obj.IgnoreViewport ? 0 : _obj.Room.Viewport.X)) 
                                                  : portraitLocation.Value.X;
            if (portraitLocation != null && _lastSpeakerOnLeft) x += config.PortraitConfig.Portrait.Width + getBorderWidth(config.PortraitConfig, true).X;
            float y = portraitLocation == null ? (_obj.BoundingBox.MaxY - (_obj.IgnoreViewport ? 0 : _obj.Room.Viewport.Y)) 
                                                  : _settings.VirtualResolution.Height;
            return new AGSSayLocation(getTextLocation(text, config, x, y), portraitLocation);
		}

        #endregion

        private PointF getTextLocation(string text, ISayConfig config, float x, float y)
        {
            //todo: need to account for alignment
            AGS.API.SizeF size = config.TextConfig.GetTextSize(text, config.LabelSize);
            float width = size.Width + config.TextConfig.PaddingLeft + config.TextConfig.PaddingRight;
            float height = size.Height + config.TextConfig.PaddingTop + config.TextConfig.PaddingBottom;

            y = MathUtils.Clamp(y, 0f, Math.Min(_settings.VirtualResolution.Height,
                                _settings.VirtualResolution.Height - height));
            y -= config.PortraitConfig == null ? 0f : config.PortraitConfig.TextOffset.Y;
            y = MathUtils.Clamp(y, 0f, Math.Min(_settings.VirtualResolution.Height,
                                _settings.VirtualResolution.Height - height));
            y += config.TextOffset.Y;

            float rightPortraitX = _settings.VirtualResolution.Width - 100;
            x += config.PortraitConfig == null ? 0f : (x > rightPortraitX ? -config.PortraitConfig.TextOffset.X
                                                       : config.PortraitConfig.TextOffset.X);
            float maxX = y > _settings.VirtualResolution.Height - 100 && x > rightPortraitX ? 
                                  Math.Min(x, _settings.VirtualResolution.Width) : _settings.VirtualResolution.Width;
            x = MathUtils.Clamp(x, 0f, Math.Max(0f, maxX - width - 10f));
            x += config.TextOffset.X;

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
                    portrait.Anchor = new PointF(0f, 0f);
                    if (_obj != _lastSpeaker)
                    {
                        _lastSpeakerOnLeft = !_lastSpeakerOnLeft;
                    }
                    return getPortraitLocation(portraitConfig);
                case PortraitPositioning.SpeakerPosition:
                    portrait.Anchor = new PointF(0f, 0f);
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

