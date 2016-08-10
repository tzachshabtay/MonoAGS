using System;

namespace AGS.Engine
{
	public interface IGLViewportMatrixFactory
	{
		IGLViewportMatrix GetViewport(int layer);
	}
}

