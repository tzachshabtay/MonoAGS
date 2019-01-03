using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public interface IAGSRenderPipeline : IRenderPipeline
    {
        IReadOnlyList<(IViewport, List<IRenderBatch>)> InstructionSet { get; }

        void Update();
    }
}
