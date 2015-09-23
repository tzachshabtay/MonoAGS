using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public interface IGLMatrixBuilder
	{
		IGLMatrices Build(ISprite obj, ISprite sprite, IObject parent, Matrix4 viewport);
	}
}

