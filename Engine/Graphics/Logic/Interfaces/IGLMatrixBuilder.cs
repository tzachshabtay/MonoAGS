using System;
using API;
using OpenTK;

namespace Engine
{
	public interface IGLMatrixBuilder
	{
		Matrix4 Build(IObject obj, IViewport viewport);
	}
}

