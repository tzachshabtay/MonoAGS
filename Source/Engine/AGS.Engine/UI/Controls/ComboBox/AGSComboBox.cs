using AGS.API;

namespace AGS.Engine
{
    public partial class AGSComboBox
    {
        partial void afterInitComponents(Resolver resolver)
        {
            RenderLayer = AGSLayers.UI;
            IgnoreScalingArea = true;
            IgnoreViewport = true;
            Pivot = new PointF();

            Enabled = true;
        }        
    }
}