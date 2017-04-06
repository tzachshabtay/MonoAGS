using System;
using AGS.API;

using System.Collections.Generic;

namespace AGS.Engine
{
	public class GLLabelRenderer : ILabelRenderer
	{
		private readonly GLImageRenderer _bgRenderer;
        private readonly IGLUtils _glUtils;
        private readonly IGraphicsBackend _graphics;
		private GLText _glText;

		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly IGLBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
		private readonly IGLViewportMatrixFactory _viewport;
		private IGLBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes;
		private readonly BitmapPool _bitmapPool;
        private readonly IFontLoader _fonts;

        public GLLabelRenderer(Dictionary<string, ITexture> textures, 
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrixFactory viewportMatrix,
            IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes, IGraphicsFactory graphicsFactory,
            IGLUtils glUtils, IGraphicsBackend graphics, IBitmapLoader bitmapLoader, IFontLoader fonts)
		{
            _glUtils = glUtils;
            _graphics = graphics;
            _fonts = fonts;
			_bitmapPool = bitmapPool;
			_viewport = viewportMatrix;
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
			_bgRenderer = new GLImageRenderer(textures,
				new BoundingBoxesEmptyBuilder(), colorBuilder, _textureRenderer, _labelBoundingBoxes,
                                              viewportMatrix, graphicsFactory, glUtils, bitmapLoader);

			_colorBuilder = colorBuilder;

			TextVisible = true;
		}

		public bool TextVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public SizeF BaseSize { get; set; }
		public ILabel Label { get; set; }
        public int CaretPosition { get; set; }
        public bool RenderCaret { get; set; }

		public float Width 
		{ 
			get 
			{ 
				return _usedLabelBoundingBoxes == null || _usedLabelBoundingBoxes.RenderBox == null ? 1f : _usedLabelBoundingBoxes.RenderBox.Width; 
			}
		}

		public float Height 
		{ 
			get 
			{ 
				return _usedLabelBoundingBoxes == null || _usedLabelBoundingBoxes.RenderBox == null ? 1f :  _usedLabelBoundingBoxes.RenderBox.Height; 
			}
		}

		public float TextWidth 
		{ 
			get 
			{ 
				return _glText == null ? 1f : _glText.Width; 
			}
		}

		public float TextHeight 
		{ 
			get 
			{ 
				return _glText == null ? 1f : _glText.Height;
			}
		}

		#region IImageRenderer implementation

        public SizeF? CustomImageSize { get; private set; }
        public PointF? CustomImageResolutionFactor { get; private set; }

		public void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
            _glText = _glText ?? new GLText (_graphics, _fonts, _bitmapPool);

			updateBoundingBoxes(obj, drawable, viewport);
			_bgRenderer.BoundingBoxes = _usedLabelBoundingBoxes;
			_bgRenderer.Prepare(obj, drawable, viewport);
		}

		public void Render(IObject obj, IViewport viewport)
		{
            if (getAutoFit() == AutoFit.LabelShouldFitText)
            {
                _glUtils.AdjustResolution(GLText.TextResolutionWidth, GLText.TextResolutionHeight);
            }
            _bgRenderer.Render(obj, viewport);

            if (TextVisible && Text != "")
			{
                if (!string.IsNullOrEmpty(Text)) _glUtils.AdjustResolution(GLText.TextResolutionWidth, GLText.TextResolutionHeight);

                IGLColor color = _colorBuilder.Build(Colors.White);
				_textureRenderer.Render(_glText.Texture, _usedTextBoundingBoxes.RenderBox, color);
			}
		}

        #endregion

        private AutoFit getAutoFit()
        {
            ITextConfig config = Config;
            return TextVisible && config != null ? config.AutoFit : AutoFit.NoFitting;
        }

        private void updateBoundingBoxes(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
            AutoFit autoFit = getAutoFit();

			float height = obj.Height;
			float width = obj.Width;
            if (autoFit == AutoFit.LabelShouldFitText)
            {
                updateText(GLText.EmptySize, null);
                CustomImageSize = new SizeF(_glText.Width, _glText.Height);
            }
            else CustomImageSize = BaseSize;

            var resolutionFactor = string.IsNullOrEmpty(Text) ? AGSModelMatrixComponent.NoScaling : new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            CustomImageResolutionFactor = resolutionFactor;
            var noFactor = AGSModelMatrixComponent.NoScaling;
            bool resolutionMatches = resolutionFactor.Equals(noFactor);
            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : _viewport.GetViewport(drawable.RenderLayer.Z).GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);

            var modelMatrices = obj.GetModelMatrices();

            IGLMatrices textRenderMatrices = new GLMatrices { ModelMatrix = modelMatrices.InObjResolutionMatrix, ViewportMatrix = viewportMatrix }; //_textRenderMatrixBuilder.Build(_labelMatrixRenderTarget, sprite, parent, matrix, areaScaling, resolutionFactor);
            IGLMatrices labelRenderMatrices = new GLMatrices { ModelMatrix = modelMatrices.InVirtualResolutionMatrix, ViewportMatrix = viewportMatrix }; // _labelRenderMatrixBuilder.Build(_labelMatrixRenderTarget, sprite, parent, matrix, areaScaling, noFactor);
            IGLMatrices textHitTestMatrices = resolutionMatches ? textRenderMatrices : labelRenderMatrices;
            IGLMatrices labelHitTestMatrices = labelRenderMatrices;

            updateBoundingBoxes(autoFit, textHitTestMatrices, labelHitTestMatrices, noFactor, resolutionMatches, true);
            if (!resolutionMatches) updateBoundingBoxes(autoFit, textRenderMatrices, labelRenderMatrices, resolutionFactor, true, false);            
		}

        private void updateBoundingBoxes(AutoFit autoFit, IGLMatrices textMatrices, IGLMatrices labelMatrices, PointF resolutionFactor, bool buildRenderBox, bool buildHitTestBox)
        {
            switch (autoFit)
            {
                case AutoFit.NoFitting:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(GLText.EmptySize, null);
                    _boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldWrapAndLabelShouldFitHeight:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(GLText.EmptySize, (int?)BaseSize.Width);
                    _boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, _glText.Width, _glText.Height, labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldFitLabel:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(_glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), null);

                    float textWidth = _glText.Width < BaseSize.Width ? _glText.BitmapWidth : MathUtils.Lerp(0f, 0f, _glText.Width, BaseSize.Width, _glText.BitmapWidth);
                    float textHeight = _glText.Height < BaseSize.Height ? _glText.BitmapHeight : MathUtils.Lerp(0f, 0f, _glText.Height, BaseSize.Height, _glText.BitmapHeight);

                    _boundingBoxBuilder.Build(_textBoundingBoxes, textWidth, textHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.LabelShouldFitText:
                    _boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, _glText.Width * resolutionFactor.X, _glText.Height * resolutionFactor.Y,
                        textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldCrop:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(_glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), (int)BaseSize.Width, true);

                    float heightOfText = _glText.Height < BaseSize.Height ? _glText.BitmapHeight : MathUtils.Lerp(0f, 0f, _glText.Height, BaseSize.Height, _glText.BitmapHeight);

                    _boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, heightOfText, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                default:
                    throw new NotSupportedException(autoFit.ToString());
            }
        }

		private void updateText(SizeF baseSize, int? maxWidth, bool? cropText = null)
		{
			if (TextVisible)
			{
                if (Text == null) return;
				_glText.SetProperties(baseSize, Text, Config, maxWidth, CaretPosition, RenderCaret, cropText);
				_glText.Refresh();
			}
		}

		private class BoundingBoxesEmptyBuilder : IGLBoundingBoxBuilder
		{
			#region IGLBoundingBoxBuilder implementation
			public void Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox)
			{
			}
			#endregion

		}
	}
}