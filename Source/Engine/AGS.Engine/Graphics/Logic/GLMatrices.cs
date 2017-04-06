using AGS.API;

namespace AGS.Engine
{
    public class GLMatrices : IGLMatrices
    {
        public Matrix4 ModelMatrix { get; set; }

        public Matrix4 ViewportMatrix { get; set; }
    }
}
