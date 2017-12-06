using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTextureOffsetComponent : AGSComponent, ITextureOffsetComponent
    {
        public PointF TextureOffset { get; set; }
    }
}
