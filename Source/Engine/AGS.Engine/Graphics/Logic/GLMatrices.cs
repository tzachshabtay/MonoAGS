using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
    public class GLMatrices : IGLMatrices
    {
        public Matrix4 ModelMatrix { get; private set; }

        public Matrix4 ViewportMatrix { get; private set; }

        public GLMatrices SetMatrices(Matrix4 modelMatrix, Matrix4 viewportMatrix)
        {
            ModelMatrix = modelMatrix;
            ViewportMatrix = viewportMatrix;
            return this;
        }
    }
}
