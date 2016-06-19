using System;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSCheckbox
    {
        private ILabelRenderer _labelRenderer;

        partial void init(Resolver resolver, ILabelRenderer labelRenderer, AGS.API.SizeF baseSize)
        {
            RenderLayer = AGSLayers.UI;
            IgnoreScalingArea = true;
            IgnoreViewport = true;
            Anchor = new PointF();

            _labelRenderer = labelRenderer;
            _labelRenderer.BaseSize = baseSize;
            CustomRenderer = _labelRenderer;

            Enabled = true;
        }

        public ITextConfig TextConfig
        {
            get { return _labelRenderer.Config; }
            set { _labelRenderer.Config = value; }
        }

        public string Text
        {
            get { return _labelRenderer.Text; }
            set { _labelRenderer.Text = value; }
        }

        public float TextHeight { get { return _labelRenderer.TextHeight; } }

        public float TextWidth { get { return _labelRenderer.TextWidth; } }

        public void ApplySkin(ICheckbox checkbox)
        {
            throw new NotSupportedException();
        }
    }
}