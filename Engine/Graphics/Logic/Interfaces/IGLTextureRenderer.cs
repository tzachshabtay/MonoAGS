using System;

namespace AGS.Engine
{
	public interface IGLTextureRenderer
	{
		void Render(int texture, IGLBoundingBox boundingBox, IGLColor color);
	}
}

