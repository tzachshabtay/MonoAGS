using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLMatrixBuilder
	{
		Matrix4 Build(IObject obj, IViewport viewport);
	}
}

