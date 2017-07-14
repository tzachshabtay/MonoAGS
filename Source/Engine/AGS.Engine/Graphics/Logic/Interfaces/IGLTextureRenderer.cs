using AGS.API;

namespace AGS.Engine
{
	public interface IGLTextureRenderer
	{
		void Render(int texture, IGLBoundingBox boundingBox, FourCorners<Vector2> textureBox, IGLColor color);
	}
}

