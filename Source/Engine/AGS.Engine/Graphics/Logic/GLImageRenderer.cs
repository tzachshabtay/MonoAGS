using AGS.API;
using System.Collections.Generic;
using System;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
        private readonly Dictionary<string, ITexture> _textures;
        private static Lazy<ITexture> _emptyTexture;
		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _renderer;
		private readonly IGLViewportMatrixFactory _layerViewports;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly IGLUtils _glUtils;
        private readonly IBitmapLoader _bitmapLoader;
        private readonly GLMatrices _matrices = new GLMatrices();

        public GLImageRenderer (Dictionary<string, ITexture> textures, 
			IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, IGLBoundingBoxes bgBoxes,
            IGLViewportMatrixFactory layerViewports, IGraphicsFactory graphicsFactory, IGLUtils glUtils, 
            IBitmapLoader bitmapLoader)
		{
            _graphicsFactory = graphicsFactory;
			_textures = textures;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
			_layerViewports = layerViewports;
			BoundingBoxes = bgBoxes;
            _glUtils = glUtils;
            _bitmapLoader = bitmapLoader;
            _emptyTexture = new Lazy<ITexture>(() => initEmptyTexture());
		}

		public IGLBoundingBoxes BoundingBoxes { get; set; }

        public static ITexture EmptyTexture { get { return _emptyTexture.Value; } }

        public SizeF? CustomImageSize { get { return null; } }
        public PointF? CustomImageResolutionFactor { get { return null; } }

		public void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
		}

		public void Render(IObject obj, IViewport viewport)
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

            var modelMatrices = obj.GetModelMatrices();
            _matrices.ModelMatrix = modelMatrices.InVirtualResolutionMatrix;
            _matrices.ViewportMatrix = viewportMatrix;

            _boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
				sprite.Image.Height, _matrices, resolutionMatches, true);
			IGLBoundingBox hitTestBox = BoundingBoxes.HitTestBox;
            
            if (!resolutionMatches)
            {
                _matrices.ModelMatrix = modelMatrices.InObjResolutionMatrix;
                _boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
                    sprite.Image.Height, _matrices, true, false);
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
                IObject parent = obj.TreeNode.Parent;
                float x = obj.X;
                float y = obj.Y;
				while (parent != null)
				{
                    x += (parent.X - parent.Width * parent.Anchor.X);
                    y += (parent.Y - parent.Height * parent.Anchor.Y);
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
            var bitmap = _bitmapLoader.Load(1, 1);
            bitmap.SetPixel(Colors.White, 0, 0);
            return _graphicsFactory.LoadImage(bitmap, new AGSLoadImageConfig(config: new AGSTextureConfig(scaleUp: ScaleUpFilters.Nearest))).Texture;
        }
	}
}

