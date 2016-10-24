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

		private readonly MatrixContainer _renderLabelMatrixContainer, _hitTestLabelMatrixContainer;

		private readonly IGLMatrixBuilder _textRenderMatrixBuilder, _labelRenderMatrixBuilder, _hitTestMatrixBuilder;
		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly IGLBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
		private readonly IGLViewportMatrixFactory _viewport;
		private IGLBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes;
		private readonly LabelMatrixRenderTarget _labelMatrixRenderTarget;
		private readonly BitmapPool _bitmapPool;

        public GLLabelRenderer(Dictionary<string, ITexture> textures, 
            IGLMatrixBuilder textRenderMatrixBuilder, IGLMatrixBuilder labelRenderMatrixBuilder, IGLMatrixBuilder hitTestMatrixBuilder,
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrixFactory viewportMatrix,
            IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes, IGraphicsFactory graphicsFactory,
            IGLUtils glUtils, IGraphicsBackend graphics)
		{
            _glUtils = glUtils;
            _graphics = graphics;
			_renderLabelMatrixContainer = new MatrixContainer ();
            _hitTestLabelMatrixContainer = new MatrixContainer();
			_bitmapPool = bitmapPool;
			_labelMatrixRenderTarget = new LabelMatrixRenderTarget ();
			_viewport = viewportMatrix;
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
			_bgRenderer = new GLImageRenderer(textures, _hitTestLabelMatrixContainer, _renderLabelMatrixContainer,
				new BoundingBoxesEmptyBuilder(), colorBuilder, _textureRenderer, _labelBoundingBoxes,
                                              viewportMatrix, graphicsFactory, glUtils);
			_textRenderMatrixBuilder = textRenderMatrixBuilder;
            _labelRenderMatrixBuilder = labelRenderMatrixBuilder;
            _hitTestMatrixBuilder = hitTestMatrixBuilder;
			_colorBuilder = colorBuilder;

			TextVisible = true;
		}

		public bool TextVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public AGS.API.SizeF BaseSize { get; set; }
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

		public void Prepare(IObject obj, IDrawableInfo drawable, IInObjectTree tree, IViewport viewport, PointF areaScaling)
		{
            _glText = _glText ?? new GLText (_graphics, _bitmapPool);

			updateBoundingBoxes(obj, drawable, tree, viewport, areaScaling);
			_bgRenderer.BoundingBoxes = _usedLabelBoundingBoxes;
			_bgRenderer.Prepare(obj, drawable, tree, viewport, areaScaling);
		}

		public void Render(IObject obj, IViewport viewport, PointF areaScaling)
		{
            if (getAutoFit() == AutoFit.LabelShouldFitText)
            {
                _glUtils.AdjustResolution(GLText.TextResolutionWidth, GLText.TextResolutionHeight);
            }
            _bgRenderer.Render(obj, viewport, areaScaling);

			if (TextVisible)
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

        private void updateBoundingBoxes(IObject obj, IDrawableInfo drawable, IInObjectTree tree, IViewport viewport, PointF areaScaling)
		{
            AutoFit autoFit = getAutoFit();
			updateLabelMatrixRenderTarget(obj);

			float height = obj.Height;
			float width = obj.Width;
			if (autoFit == AutoFit.LabelShouldFitText)
			{
				updateText(GLText.EmptySize, null);
				_labelMatrixRenderTarget.Width = _glText.Width;
				_labelMatrixRenderTarget.Height = _glText.Height;
			}
            var resolutionFactor = string.IsNullOrEmpty(Text) ? new PointF(1f,1f) : new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            var noFactor = GLMatrixBuilder.NoScaling;
            bool resolutionMatches = resolutionFactor.Equals(noFactor);
            var matrix = drawable.IgnoreViewport ? Matrix4.Identity : _viewport.GetViewport(drawable.RenderLayer.Z).GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);
            var sprite = obj.Animation == null ? null : obj.Animation.Sprite;
            var parent = tree.TreeNode.Parent;

            IGLMatrices textRenderMatrices = _textRenderMatrixBuilder.Build(_labelMatrixRenderTarget, sprite, parent, matrix, areaScaling, resolutionFactor);
            IGLMatrices labelRenderMatrices = _labelRenderMatrixBuilder.Build(_labelMatrixRenderTarget, sprite, parent, matrix, areaScaling, noFactor);
            IGLMatrices textHitTestMatrices = resolutionMatches ? textRenderMatrices : _hitTestMatrixBuilder.Build(_labelMatrixRenderTarget, sprite, parent,
                matrix, areaScaling, noFactor);
            IGLMatrices labelHitTestMatrices = resolutionMatches ? labelRenderMatrices : textHitTestMatrices;
            _renderLabelMatrixContainer.Matrices = labelRenderMatrices;
            _hitTestLabelMatrixContainer.Matrices = labelHitTestMatrices;
			_labelMatrixRenderTarget.Width = width;
			_labelMatrixRenderTarget.Height = height;

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

		private void updateLabelMatrixRenderTarget(IHasModelMatrix obj)
		{
            _labelMatrixRenderTarget.X = obj.X;
            _labelMatrixRenderTarget.Y = obj.Y;
			_labelMatrixRenderTarget.Anchor = obj.Anchor;
			_labelMatrixRenderTarget.Angle = obj.Angle;
			_labelMatrixRenderTarget.Width = BaseSize.Width;
			_labelMatrixRenderTarget.Height = BaseSize.Height;
			_labelMatrixRenderTarget.ScaleX = obj.ScaleX;
			_labelMatrixRenderTarget.ScaleY = obj.ScaleY;
		}

		private void updateText(AGS.API.SizeF baseSize, int? maxWidth, bool? cropText = null)
		{
			if (TextVisible)
			{
				_glText.SetProperties(baseSize, Text, Config, maxWidth, CaretPosition, RenderCaret, cropText);
				_glText.Refresh();
			}
		}

		private class MatrixContainer : IGLMatrixBuilder
		{
			public IGLMatrices Matrices { get; set; }

			#region IGLMatrixBuilder implementation
			public IGLMatrices Build(IHasModelMatrix obj, IHasModelMatrix sprite, IObject parent, Matrix4 viewport, PointF areaScaling, PointF resolutionTransform)
			{
				return Matrices;
			}
			#endregion
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