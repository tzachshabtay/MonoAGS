using AGS.API;
using System.Collections.Generic;
using System;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
        private readonly Dictionary<string, ITexture> _textures;
        private static Lazy<ITexture> _emptyTexture;
		private readonly IBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _renderer;
		private readonly IGLViewportMatrixFactory _layerViewports;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly IGLUtils _glUtils;
        private readonly IBitmapLoader _bitmapLoader;
        private readonly GLMatrices _matrices = new GLMatrices();
        private readonly AGSBoundingBox _emptySquare = default(AGSBoundingBox);
        private readonly Func<string, ITexture> _createTextureFunc;
        private readonly IHasImage[] _colorAdjusters;

        public GLImageRenderer (Dictionary<string, ITexture> textures, 
			IBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, AGSBoundingBoxes bgBoxes,
            IGLViewportMatrixFactory layerViewports, IGraphicsFactory graphicsFactory, IGLUtils glUtils, 
            IBitmapLoader bitmapLoader)
		{
            _graphicsFactory = graphicsFactory;
            _createTextureFunc = createNewTexture; //Creating a delegate in advance to avoid memory allocations on critical path
			_textures = textures;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
			_layerViewports = layerViewports;
			BoundingBoxes = bgBoxes;
            _glUtils = glUtils;
            _bitmapLoader = bitmapLoader;
            _emptyTexture = new Lazy<ITexture>(() => initEmptyTexture());
            _colorAdjusters = new IHasImage[2];
		}

		public AGSBoundingBoxes BoundingBoxes { get; set; }

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
            Size resolution;
            PointF resolutionFactor;
            bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, AGSGame.Game.Settings.VirtualResolution,
                                                         obj, null, out resolutionFactor, out resolution);

            var viewportMatrix = obj.IgnoreViewport ? Matrix4.Identity : layerViewport.GetMatrix(viewport, obj.RenderLayer.ParallaxSpeed);

            var modelMatrices = obj.GetModelMatrices();
            _matrices.ModelMatrix = modelMatrices.InVirtualResolutionMatrix;
            _matrices.ViewportMatrix = viewportMatrix;

            float width = sprite.Image.Width / resolutionFactor.X;
            float height = sprite.Image.Height / resolutionFactor.Y;

            var scale = _boundingBoxBuilder.Build(BoundingBoxes, width, height, _matrices, resolutionMatches, true);
            AGSBoundingBox hitTestBox = BoundingBoxes.HitTestBox;
            
            if (!resolutionMatches)
            {
                _matrices.ModelMatrix = modelMatrices.InObjResolutionMatrix;
                _boundingBoxBuilder.Build(BoundingBoxes, sprite.Image.Width,
                    sprite.Image.Height, _matrices, true, false);
            }
            AGSBoundingBox renderBox = BoundingBoxes.RenderBox;
			var crop = obj.GetComponent<ICropSelfComponent>();
            var cropInfo = renderBox.Crop(BoundingBoxType.Render, crop, resolutionFactor, scale);
            if (cropInfo.Equals(default(AGSCropInfo))) return;
            renderBox = cropInfo.BoundingBox;
            hitTestBox = hitTestBox.Crop(BoundingBoxType.HitTest, crop, AGSModelMatrixComponent.NoScaling, scale).BoundingBox;

			ITexture texture = _textures.GetOrAdd (sprite.Image.ID, _createTextureFunc);

            _colorAdjusters[0] = sprite;
            _colorAdjusters[1] = obj;
			IGLColor color = _colorBuilder.Build(_colorAdjusters);

            IBorderStyle border = obj.Border;
            AGSBoundingBox borderBox = _emptySquare;
			if (border != null)
			{
                if (renderBox.BottomLeft.X > renderBox.BottomRight.X) borderBox = renderBox.FlipHorizontal();
                else borderBox = renderBox;

				border.RenderBorderBack(borderBox);
			}
            _renderer.Render(texture.ID, renderBox, cropInfo.TextureBox, color);

            if (obj.BoundingBoxes == null)
            {
                obj.BoundingBoxes = new AGSBoundingBoxes();
            }
            obj.BoundingBoxes.RenderBox = renderBox;
            obj.BoundingBoxes.HitTestBox = hitTestBox;

			if (border != null)
			{
				border.RenderBorderFront(borderBox);
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

