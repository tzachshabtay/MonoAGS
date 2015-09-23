using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLMatrixBuilder
	{
		IGLMatrices Build(IObject obj, Matrix4 viewport);
	}
}

