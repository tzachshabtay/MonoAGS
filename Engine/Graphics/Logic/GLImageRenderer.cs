using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
		private Dictionary<string, GLImage> _textures;
		private IGLMatrixBuilder _matrixBuilder;
		private IGLBoundingBoxBuilder _boundingBoxBuilder;
		private IGLColorBuilder _colorBuilder;
		private IGLTextureRenderer _renderer;
		private IGLViewportMatrix _viewport;

		public GLImageRenderer (Dictionary<string, GLImage> textures, 
			IGLMatrixBuilder matrixBuilder, IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, IGLBoundingBoxes bgBoxes,
			IGLViewportMatrix viewport)
		{
			_textures = textures;
			_matrixBuilder = matrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
			_viewport = viewport;
			BoundingBoxes = bgBoxes;
		}

		public IGLBoundingBoxes BoundingBoxes { get; set; }

		public void Prepare(IObject obj, IViewport viewport, IPoint areaScaling)
		{
		}

		public void Render(IObject obj, IViewport viewport, IPoint areaScaling)
		{
			if (obj.Animation == null)
			{
				return;
			}
			ISprite sprite = obj.Animation.Sprite;

			IGLMatrices matrices = _matrixBuilder.Build(obj, obj.Animation.Sprite, obj.TreeNode.Parent,
				obj.IgnoreViewport ? Matrix4.Identity : _viewport.GetMatrix(viewport), areaScaling);

			_boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
				sprite.Image.Height, matrices);
			IGLBoundingBox hitTestBox = BoundingBoxes.HitTestBox;
			IGLBoundingBox renderBox = BoundingBoxes.RenderBox;

			GLImage glImage = _textures.GetOrAdd (sprite.Image.ID, () => createNewTexture (sprite.Image.ID));

			IGLColor color = _colorBuilder.Build(sprite, obj);

			_renderer.Render(glImage.Texture, renderBox, color);

			Vector3 bottomLeft = hitTestBox.BottomLeft;
			Vector3 topLeft = hitTestBox.TopLeft;
			Vector3 bottomRight = hitTestBox.BottomRight;
			Vector3 topRight = hitTestBox.TopRight;

			AGSSquare square = new AGSSquare (new AGSPoint (bottomLeft.X, bottomLeft.Y),
				                   new AGSPoint (bottomRight.X, bottomRight.Y), new AGSPoint (topLeft.X, topLeft.Y),
				                   new AGSPoint (topRight.X, topRight.Y));
			obj.BoundingBox = square;

			IBorderStyle border = obj.Border;
			if (border != null)
			{
				color = _colorBuilder.Build(border.Color);
				GLUtils.DrawQuadBorder(renderBox.BottomLeft, renderBox.BottomRight, 
					renderBox.TopLeft, renderBox.TopRight, border.LineWidth, 
					color.R, color.G, color.B, color.A);
			}
			if (obj.DebugDrawAnchor)
				GLUtils.DrawCross (obj.X - viewport.X, obj.Y - viewport.Y, 10, 10, 1f, 1f, 1f, 1f);
		}
			
		private GLImage createNewTexture(string path)
		{
			if (string.IsNullOrEmpty(path)) return new GLImage (); //transparent image

			GLGraphicsFactory loader = new GLGraphicsFactory (null, null);
			return loader.LoadImageInner (path);
		}
	}
}

