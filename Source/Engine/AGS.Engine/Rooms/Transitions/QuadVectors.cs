using AGS.API;

namespace AGS.Engine
{
	public class QuadVectors
	{
        private readonly IGLUtils _glUtils;
        private readonly AGSBoundingBox _box;

        public QuadVectors(IGame game, IGLUtils glUtils)
		{
            _glUtils = glUtils;
			TopLeft = new Vector3 ();
			BottomLeft = new Vector3 (0f, game.Settings.VirtualResolution.Height, 0f);
			TopRight = new Vector3 (game.Settings.VirtualResolution.Width, 0f, 0f);
			BottomRight = new Vector3 (game.Settings.VirtualResolution.Width, game.Settings.VirtualResolution.Height, 0f);
            _box = new AGSBoundingBox(BottomLeft, BottomRight, TopLeft, TopRight);
            if (shouldFlipY()) _box = _box.FlipVertical();
		}

        public QuadVectors(float x, float y, float width, float height, IGLUtils glUtils)
		{
            _glUtils = glUtils;
			TopLeft = new Vector3 (x, y, 0f);
			BottomLeft = new Vector3 (x, y + height, 0f);
			TopRight = new Vector3 (x + width, y, 0f);
			BottomRight = new Vector3 (x + width, y + height, 0f);
            _box = new AGSBoundingBox(BottomLeft, BottomRight, TopLeft, TopRight);
            if (shouldFlipY()) _box = _box.FlipVertical();
		}

		public Vector3 BottomLeft { get; private set; }
		public Vector3 BottomRight { get; private set; }
		public Vector3 TopLeft { get; private set; }
		public Vector3 TopRight { get; private set; }

        public void Render(ITexture texture, float r = 1f, float g = 1f, float b = 1f, float a = 1f)
		{
            _glUtils.DrawQuad(texture == null ? 0 : texture.ID, _box, r, g, b, a);
		}

        //https://github.com/mellinoe/veldrid/issues/35
        private bool shouldFlipY() => _glUtils.Backend != GraphicsBackend.OpenGL && _glUtils.Backend != GraphicsBackend.OpenGLES;
	}
}

