using AGS.API;

namespace AGS.Engine
{
    public class AGSSayLocation : ISayLocation
    {
        public AGSSayLocation(PointF textLocation, PointF? portraitLocation)
        {
            TextLocation = textLocation;
            PortraitLocation = portraitLocation;
        }

        public PointF? PortraitLocation { get; private set; }

        public PointF TextLocation { get; private set; }
    }
}

