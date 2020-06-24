using AGS.API;

namespace AGS.Engine
{
	public interface IGLTextureRenderer
	{
		void Render(int texture, AGSBoundingBoxes boundingBoxes, GLColor color);
	}
}

