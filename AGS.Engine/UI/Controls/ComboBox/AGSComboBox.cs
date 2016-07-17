using System;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSComboBox
    {
        partial void init(Resolver resolver)
        {
            RenderLayer = AGSLayers.UI;
            IgnoreScalingArea = true;
            IgnoreViewport = true;
            Anchor = new PointF();

            Enabled = true;
        }

        public void ApplySkin(IComboBox comboBox)
        {
            throw new NotSupportedException();
        }
    }
}