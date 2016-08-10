using AGS.API;

namespace AGS.Engine
{
    public partial class AGSCheckBox
    {
        partial void init(Resolver resolver)
        {
            RenderLayer = AGSLayers.UI;
            IgnoreScalingArea = true;
            IgnoreViewport = true;
            Anchor = new PointF();

            Enabled = true;
        }        
    }
}