﻿using AGS.API;

namespace AGS.Engine
{
	public class GLTextureRenderer : IGLTextureRenderer
	{
        private readonly IGLUtils _glUtils;

        public GLTextureRenderer(IGLUtils glUtils)
		{
            _glUtils = glUtils;
		}

		#region IGLTextureRenderer implementation

		public void Render(int texture, AGSBoundingBoxes boundingBoxes, GLColor color)
		{
            if (boundingBoxes.TextureBox == null)
            {
                _glUtils.DrawQuad(texture, boundingBoxes.ViewportBox, color.R, color.G, color.B, color.A);
            }
            else
            {
                _glUtils.DrawQuad(texture, boundingBoxes.ViewportBox, color, boundingBoxes.TextureBox);
            }
		}

		#endregion
	}
}