using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK;
using Autofac;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
		private Dictionary<string, GLImage> _textures;
		private IGLMatrixBuilder _hitTestMatrixBuilder, _renderMatrixBuilder;
		private IGLBoundingBoxBuilder _boundingBoxBuilder;
		private IGLColorBuilder _colorBuilder;
		private IGLTextureRenderer _renderer;
		private IGLViewportMatrixFactory _layerViewports;

		public GLImageRenderer (Dictionary<string, GLImage> textures, 
			IGLMatrixBuilder hitTestMatrixBuilder, IGLMatrixBuilder renderMatrixBuilder, IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, IGLBoundingBoxes bgBoxes,
			IGLViewportMatrixFactory layerViewports)
		{
			_textures = textures;
			_renderMatrixBuilder = renderMatrixBuilder;
            _hitTestMatrixBuilder = hitTestMatrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
			_layerViewports = layerViewports;
			BoundingBoxes = bgBoxes;
		}

		public IGLBoundingBoxes BoundingBoxes { get; set; }

		public void Prepare(IObject obj, IDrawableInfo drawable, IInObjectTree tree, IViewport viewport, PointF areaScaling)
		{
		}

		public void Render(IObject obj, IViewport viewport, PointF areaScaling)
		{
			if (obj.Animation == null)
			{
				return;
			}
			ISprite sprite = obj.Animation.Sprite;
			if (sprite == null || sprite.Image == null)
			{
				return;
			}

			var layerViewport = _layerViewports.GetViewport(obj.RenderLayer.Z);
            var gameResolution = AGSGame.Game.Settings.VirtualResolution;
            var resolution = obj.RenderLayer.IndependentResolution ?? gameResolution;
            bool resolutionMatches = resolution.Equals(gameResolution);

            var viewportMatrix = obj.IgnoreViewport ? Matrix4.Identity : layerViewport.GetMatrix(viewport, obj.RenderLayer.ParallaxSpeed);
            PointF resolutionFactor = new PointF(resolution.Width / gameResolution.Width, resolution.Height / gameResolution.Height);
            IGLMatrices matricesRender = _renderMatrixBuilder.Build(obj, obj.Animation.Sprite, obj.TreeNode.Parent,
                viewportMatrix, areaScaling, resolutionFactor);

            IGLMatrices matricesHitTest = resolutionMatches ? matricesRender : _hitTestMatrixBuilder.Build(obj, obj.Animation.Sprite, obj.TreeNode.Parent,
                viewportMatrix, areaScaling, GLMatrixBuilder.NoScaling);

            _boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
				sprite.Image.Height, matricesHitTest, resolutionMatches, true);
			IGLBoundingBox hitTestBox = BoundingBoxes.HitTestBox;
            
            if (!resolutionMatches)
            {
                _boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
                    sprite.Image.Height, matricesRender, true, false);
            }
            IGLBoundingBox renderBox = BoundingBoxes.RenderBox;

			GLImage glImage = _textures.GetOrAdd (sprite.Image.ID, () => createNewTexture (sprite.Image.ID));

			IGLColor color = _colorBuilder.Build(sprite, obj);

            IBorderStyle border = obj.Border;
			ISquare renderSquare = null;
			if (border != null)
			{
				renderSquare = renderBox.ToSquare();
				border.RenderBorderBack(renderSquare);
			}
			_renderer.Render(glImage.Texture, renderBox, color);

            Vector3 bottomLeft = hitTestBox.BottomLeft;
            Vector3 topLeft = hitTestBox.TopLeft;
            Vector3 bottomRight = hitTestBox.BottomRight;
            Vector3 topRight = hitTestBox.TopRight;

            AGSSquare square = new AGSSquare (new PointF (bottomLeft.X, bottomLeft.Y),
				new PointF (bottomRight.X, bottomRight.Y), new PointF (topLeft.X, topLeft.Y),
				new PointF (topRight.X, topRight.Y));
			obj.BoundingBox = square;

			if (border != null)
			{
				border.RenderBorderFront(renderSquare);
			}
			if (obj.DebugDrawAnchor)
			{
				IObject parent = obj;
				float x = 0f;
				float y = 0f;
				while (parent != null)
				{
					x += parent.X;
					y += parent.Y;
					parent = parent.TreeNode.Parent;
				}
				GLUtils.DrawCross(x - viewport.X, y - viewport.Y, 10, 10, 1f, 1f, 1f, 1f);
			}
		}
			
		private GLImage createNewTexture(string path)
		{
			if (string.IsNullOrEmpty(path)) return new GLImage (); //transparent image

			GLGraphicsFactory loader = new GLGraphicsFactory (null, null);
			return loader.LoadImageInner (path);
		}
	}
}

