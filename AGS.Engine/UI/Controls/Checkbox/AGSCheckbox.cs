using System;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSCheckbox
    {
        partial void init(Resolver resolver)
        {
            RenderLayer = AGSLayers.UI;
            IgnoreScalingArea = true;
            IgnoreViewport = true;
            Anchor = new PointF();

            Enabled = true;
        }

        public void ApplySkin(ICheckbox checkbox)
        {
            throw new NotSupportedException();
        }
    }
}