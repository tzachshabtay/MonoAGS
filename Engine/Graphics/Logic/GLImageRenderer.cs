using System;
using API;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK;

namespace Engine
{
	public class GLImageRenderer : IImageRenderer
	{
		private Dictionary<string, GLImage> _textures;
		private IGLMatrixBuilder _matrixBuilder;
		private IGLBoundingBoxBuilder _boundingBoxBuilder;
		private IGLColorBuilder _colorBuilder;
		private IGLTextureRenderer _renderer;

		public GLImageRenderer (Dictionary<string, GLImage> textures, 
			IGLMatrixBuilder matrixBuilder, IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer)
		{
			_textures = textures;
			_matrixBuilder = matrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
		}

		public void Render(IObject obj, IViewport viewport)
		{
			ISprite sprite = obj.Animation.Sprite;

			Matrix4 mvMatrix = _matrixBuilder.Build(obj, viewport);

			IGLBoundingBox boundingBox = _boundingBoxBuilder.Build(sprite.Image.Width,
				                             sprite.Image.Height, mvMatrix);
			Vector3 bottomLeft = boundingBox.BottomLeft;
			Vector3 topLeft = boundingBox.TopLeft;
			Vector3 bottomRight = boundingBox.BottomRight;
			Vector3 topRight = boundingBox.TopRight;

			GLImage glImage = _textures.GetOrAdd (sprite.Image.ID, () => createNewTexture (sprite.Image.ID));

			IGLColor color = _colorBuilder.Build(sprite, obj);

			_renderer.Render(glImage.Texture, boundingBox, color);

			AGSSquare square = new AGSSquare (new AGSPoint (bottomLeft.X, bottomLeft.Y),
				                   new AGSPoint (bottomRight.X, bottomRight.Y), new AGSPoint (topLeft.X, topLeft.Y),
				                   new AGSPoint (topRight.X, topRight.Y));
			obj.BoundingBox = square;

			IBorderStyle border = obj.Border;
			if (border != null)
			{
				color = _colorBuilder.Build(border.Color);
				GLUtils.DrawQuadBorder(bottomLeft, bottomRight, topLeft, topRight, border.LineWidth, 
					color.R, color.G, color.B, color.A);
			}
			if (obj.DebugDrawAnchor)
				GLUtils.DrawCross (obj.X - viewport.X, obj.Y - viewport.Y, 10, 10, 1f, 1f, 1f, 1f);
		}
			
		private GLImage createNewTexture(string path)
		{
			if (string.IsNullOrEmpty(path)) return new GLImage () { Width = 1, Height = 1 }; //transparent image

			GLGraphicsFactory loader = new GLGraphicsFactory (null);
			return loader.LoadImageInner (path);
		}
	}
}

