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
		private GLText _glTextRender, _glTextHitTest;

		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly IGLBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
		private readonly IGLViewportMatrixFactory _viewport;
		private IGLBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes;
		private readonly BitmapPool _bitmapPool;
        private readonly IFontLoader _fonts;
        private readonly Size _virtualResolution;

        public GLLabelRenderer(Dictionary<string, ITexture> textures, 
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrixFactory viewportMatrix,
            IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes, IGraphicsFactory graphicsFactory,
                               IGLUtils glUtils, IGraphicsBackend graphics, IBitmapLoader bitmapLoader, IFontLoader fonts, 
                               IRuntimeSettings settings)
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
            _virtualResolution = settings.VirtualResolution;
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
				return _glTextHitTest == null ? 1f : _glTextHitTest.Width; 
			}
		}

		public float TextHeight 
		{ 
			get 
			{ 
				return _glTextHitTest == null ? 1f : _glTextHitTest.Height;
			}
		}

		#region IImageRenderer implementation

        public SizeF? CustomImageSize { get; private set; }
        public PointF? CustomImageResolutionFactor { get; private set; }

		public void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
            _glTextHitTest = _glTextHitTest ?? new GLText (_graphics, _fonts, _bitmapPool);
            _glTextRender = _glTextRender ?? new GLText(_graphics, _fonts, _bitmapPool);

			updateBoundingBoxes(obj, drawable, viewport);
			_bgRenderer.BoundingBoxes = _usedLabelBoundingBoxes;
			_bgRenderer.Prepare(obj, drawable, viewport);
		}

		public void Render(IObject obj, IViewport viewport)
		{
            PointF resolutionFactor; Size resolution;
            AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, obj,
               new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY), out resolutionFactor,
               out resolution);
            
            if (getAutoFit() == AutoFit.LabelShouldFitText)
            {
                _glUtils.AdjustResolution(resolution.Width, resolution.Height);
            }
            _bgRenderer.Render(obj, viewport);

            if (TextVisible && Text != "")
			{
                if (!string.IsNullOrEmpty(Text)) _glUtils.AdjustResolution(resolution.Width, resolution.Height);

                IGLColor color = _colorBuilder.Build(Colors.White);
				_textureRenderer.Render(_glTextHitTest.Texture, _usedTextBoundingBoxes.RenderBox, color);
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
            var noFactor = AGSModelMatrixComponent.NoScaling;
            bool resolutionMatches;
            PointF hitTestResolutionFactor;
            Size resolution;
            PointF renderResolutionFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            if (string.IsNullOrEmpty(Text))
            {
                hitTestResolutionFactor = noFactor;
                resolutionMatches = true;
            }
            else resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, drawable,
                                                                                  renderResolutionFactor, out hitTestResolutionFactor,
                                                                                  out resolution);
            if (!renderResolutionFactor.Equals(hitTestResolutionFactor))
            {
                renderResolutionFactor = noFactor;
            }
            AutoFit autoFit = getAutoFit();
			float height = obj.Height;
			float width = obj.Width;
            if (autoFit == AutoFit.LabelShouldFitText)
            {
                updateText(_glTextHitTest, GLText.EmptySize, renderResolutionFactor, null);
                updateText(_glTextRender, GLText.EmptySize, renderResolutionFactor, null);
                CustomImageSize = new SizeF(_glTextHitTest.Width, _glTextHitTest.Height);
            }
            else CustomImageSize = BaseSize;

            CustomImageResolutionFactor = hitTestResolutionFactor;
            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : _viewport.GetViewport(drawable.RenderLayer.Z).GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);

            var modelMatrices = obj.GetModelMatrices();

            IGLMatrices textRenderMatrices = new GLMatrices { ModelMatrix = modelMatrices.InObjResolutionMatrix, ViewportMatrix = viewportMatrix };
            IGLMatrices labelRenderMatrices = obj.RenderLayer.IndependentResolution != null ? textRenderMatrices : new GLMatrices { ModelMatrix = modelMatrices.InVirtualResolutionMatrix, ViewportMatrix = viewportMatrix };
            IGLMatrices textHitTestMatrices = resolutionMatches ? textRenderMatrices : obj.RenderLayer.IndependentResolution == null ? labelRenderMatrices : new GLMatrices { ModelMatrix = modelMatrices.InVirtualResolutionMatrix, ViewportMatrix = viewportMatrix };
            IGLMatrices labelHitTestMatrices = obj.RenderLayer.IndependentResolution == null ? labelRenderMatrices : textHitTestMatrices;

            if (autoFit == AutoFit.LabelShouldFitText)
            {
                hitTestResolutionFactor = new PointF(1f / hitTestResolutionFactor.X, 1f / hitTestResolutionFactor.Y);
            }
            updateBoundingBoxes(_glTextHitTest, autoFit, textHitTestMatrices, labelHitTestMatrices, hitTestResolutionFactor, resolutionMatches, true);
            if (!resolutionMatches) updateBoundingBoxes(_glTextRender, autoFit, textRenderMatrices, labelRenderMatrices, renderResolutionFactor, true, false);            
		}


        private void updateBoundingBoxes(GLText glText, AutoFit autoFit, IGLMatrices textMatrices, IGLMatrices labelMatrices, PointF resolutionFactor, bool buildRenderBox, bool buildHitTestBox)
        {
            switch (autoFit)
            {
                case AutoFit.NoFitting:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, GLText.EmptySize, resolutionFactor, null);
                    _boundingBoxBuilder.Build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldWrapAndLabelShouldFitHeight:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, GLText.EmptySize, resolutionFactor, (int?)BaseSize.Width);
                    _boundingBoxBuilder.Build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, glText.Width, glText.Height, labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldFitLabel:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), resolutionFactor, null);

                    float textWidth = glText.Width < BaseSize.Width ? glText.BitmapWidth : MathUtils.Lerp(0f, 0f, glText.Width, BaseSize.Width, glText.BitmapWidth);
                    float textHeight = glText.Height < BaseSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, BaseSize.Height, glText.BitmapHeight);

                    _boundingBoxBuilder.Build(_textBoundingBoxes, textWidth, textHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.LabelShouldFitText:
                    _boundingBoxBuilder.Build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, glText.Width * resolutionFactor.X, glText.Height * resolutionFactor.Y,
                        textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldCrop:
                    _boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), resolutionFactor, (int)BaseSize.Width, true);

                    float heightOfText = glText.Height < BaseSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, BaseSize.Height, glText.BitmapHeight);

                    _boundingBoxBuilder.Build(_textBoundingBoxes, glText.BitmapWidth, heightOfText, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                default:
                    throw new NotSupportedException(autoFit.ToString());
            }
        }

		private void updateText(GLText glText, SizeF baseSize, PointF resolutionFactor, int? maxWidth, bool cropText = false)
		{
			if (TextVisible)
			{
                if (Text == null) return;
                glText.SetProperties(baseSize, Text, Config, maxWidth, resolutionFactor, CaretPosition, RenderCaret, cropText);
				glText.Refresh();
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