using System;
using AGS.API;

namespace AGS.Engine
{
	public class GLImageRenderer : IImageRenderer
	{
        private readonly ITextureCache _textures;
        private readonly Func<string, ITexture> _getTextureFunc;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _renderer;
        private readonly IGLUtils _glUtils;
        private readonly GLMatrices _matrices = new GLMatrices();
        private readonly AGSBoundingBox _emptySquare = default;
        private readonly IHasImage[] _colorAdjusters;

        public GLImageRenderer (ITextureCache textures, ITextureFactory textureFactory,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer, IGLUtils glUtils)
		{
			_textures = textures;
            _getTextureFunc = textureFactory.CreateTexture;  //Creating a delegate in advance to avoid memory allocations on critical path
			_colorBuilder = colorBuilder;
			_renderer = renderer;
            _glUtils = glUtils;
            _colorAdjusters = new IHasImage[2];
		}

        public SizeF? CustomImageSize => null;
        public PointF? CustomImageResolutionFactor => null;

        public void Prepare(IObject obj, IDrawableInfoComponent drawable, IViewport viewport)
		{
		}

        public void Render(IObject obj, IViewport viewport)
		{
            ISprite sprite = obj.CurrentSprite;
            if (sprite == null || sprite.Image == null)
            {
                return;
            }
            var boundingBoxes = obj.GetBoundingBoxes(viewport);
            if (boundingBoxes == null || !boundingBoxes.ViewportBox.IsValid)
            {
                return;
            }

            ITexture texture = _textures.GetTexture(sprite.Image.ID, _getTextureFunc);

            _colorAdjusters[0] = sprite;
            _colorAdjusters[1] = obj;
			IGLColor color = _colorBuilder.Build(_colorAdjusters);

            IBorderStyle border = obj.Border;
            AGSBoundingBox borderBox = _emptySquare;
            if (!obj.Visible) return;
			if (border != null)
			{
                if (boundingBoxes.ViewportBox.BottomLeft.X > boundingBoxes.ViewportBox.BottomRight.X) borderBox = boundingBoxes.ViewportBox.FlipHorizontal();
                else borderBox = boundingBoxes.ViewportBox;

				border.RenderBorderBack(borderBox);
			}
            _renderer.Render(texture.ID, boundingBoxes, color);

			border?.RenderBorderFront(borderBox);
			if (obj.DebugDrawPivot)
			{
                IObject parent = obj.TreeNode.Parent;
                float x = obj.X;
                float y = obj.Y;
				while (parent != null)
				{
                    x += (parent.X - parent.Width * parent.Pivot.X);
                    y += (parent.Y - parent.Height * parent.Pivot.Y);
					parent = parent.TreeNode.Parent;
				}
                if (!obj.IgnoreViewport)
                {
                    x -= viewport.X;
                    y -= viewport.Y;
                }
                _glUtils.DrawCross(x, y, 10, 10, 1f, 1f, 1f, 1f);
			}
		}
	}
}

