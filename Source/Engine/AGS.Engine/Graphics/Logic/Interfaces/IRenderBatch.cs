using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public interface IRenderBatch
    {
        Size Resolution { get; }
        IShader Shader { get; }
        IReadOnlyList<IRenderInstruction> Instructions { get; }
    }
}
