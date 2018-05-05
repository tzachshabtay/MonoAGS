using System;
using AGS.API;

namespace AGS.Engine
{
    public interface IAGSWindowInfo : IWindowInfo
    {
        new Rectangle ScreenViewport { get; set; }
    }
}