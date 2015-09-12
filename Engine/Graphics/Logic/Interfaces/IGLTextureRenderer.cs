using System;

namespace Engine
{
	public interface IGLTextureRenderer
	{
		void Render(int texture, IGLBoundingBox boundingBox, IGLColor color);
	}
}

