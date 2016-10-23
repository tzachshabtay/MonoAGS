using AGS.API;
using System.Collections.Generic;
using OpenTK;
using System;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
        private Dictionary<string, ITexture> _textures;
        private static Lazy<ITexture> _emptyTexture;
		private IGLMatrixBuilder _hitTestMatrixBuilder, _renderMatrixBuilder;
		private IGLBoundingBoxBuilder _boundingBoxBuilder;
		private IGLColorBuilder _colorBuilder;
		private IGLTextureRenderer _renderer;
		private IGLViewportMatrixFactory _layerViewports;
        private IGraphicsFactory _graphicsFactory;
        private IGLUtils _glUtils;

        public GLImageRenderer (Dictionary<string, ITexture> textures, 
			IGLMatrixBuilder hitTestMatrixBuilder, IGLMatrixBuilder renderMatrixBuilder, IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, IGLBoundingBoxes bgBoxes,
            IGLViewportMatrixFactory layerViewports, IGraphicsFactory graphicsFactory, IGLUtils glUtils)
		{
            _graphicsFactory = graphicsFactory;
			_textures = textures;
			_renderMatrixBuilder = renderMatrixBuilder;
            _hitTestMatrixBuilder = hitTestMatrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
			_layerViewports = layerViewports;
			BoundingBoxes = bgBoxes;
            _glUtils = glUtils;
            _emptyTexture = new Lazy<ITexture>(() => initEmptyTexture());
		}

		public IGLBoundingBoxes BoundingBoxes { get; set; }

        public static ITexture EmptyTexture { get { return _emptyTexture.Value; } }

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

            ITexture texture = _textures.GetOrAdd (sprite.Image.ID, () => createNewTexture (sprite.Image.ID));

			IGLColor color = _colorBuilder.Build(sprite, obj);

            IBorderStyle border = obj.Border;
			ISquare renderSquare = null;
			if (border != null)
			{
				renderSquare = renderBox.ToSquare();
				border.RenderBorderBack(renderSquare);
			}
            _renderer.Render(texture.ID, renderBox, color);

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
                _glUtils.DrawCross(x - viewport.X, y - viewport.Y, 10, 10, 1f, 1f, 1f, 1f);
			}
		}
			
        private ITexture createNewTexture(string path)
		{
            if (string.IsNullOrEmpty(path)) return _emptyTexture.Value;
            return _graphicsFactory.LoadImage(path).Texture;
		}

        private ITexture initEmptyTexture()
        {
            var bitmap = Hooks.BitmapLoader.Load(1, 1);
            bitmap.SetPixel(Colors.White, 0, 0);
            return _graphicsFactory.LoadImage(bitmap, new AGSLoadImageConfig(config: new AGSTextureConfig(scaleUp: ScaleUpFilters.Nearest))).Texture;
        }
	}
}

