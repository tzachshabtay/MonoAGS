using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSPortraitConfig : IPortraitConfig
    {
        public AGSPortraitConfig()
        {
            PortraitOffset = new PointF(5f, 5f);
            TextOffset = new PointF(3f, 3f);
        }

        public IObject Portrait { get; set; }

        public PointF PortraitOffset { get; set; }

        public PointF TextOffset { get; set; }

        public PortraitPositioning Positioning { get; set; }
    }
}

