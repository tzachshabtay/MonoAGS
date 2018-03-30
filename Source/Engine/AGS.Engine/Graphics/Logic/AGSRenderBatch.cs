using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSRenderBatch : IRenderBatch
    {
        public AGSRenderBatch(Size resolution, IShader shader, List<IRenderInstruction> instructions)
        {
            Resolution = resolution;
            Shader = shader;
            Instructions = instructions;
        }

        public Size Resolution { get; private set; }

        public IShader Shader { get; private set; }

        public IReadOnlyList<IRenderInstruction> Instructions { get; private set; }
    }
}
