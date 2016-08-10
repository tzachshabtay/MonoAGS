using System;
namespace AGS.API
{
    public interface ISayLocation
    {
        PointF TextLocation { get; }
        PointF? PortraitLocation { get; }
    }
}

