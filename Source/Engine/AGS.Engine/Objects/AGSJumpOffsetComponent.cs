using AGS.API;

namespace AGS.Engine
{
    public class AGSJumpOffsetComponent : AGSComponent, IJumpOffsetComponent
    {
        public PointF JumpOffset { get; set; }
     }
}
