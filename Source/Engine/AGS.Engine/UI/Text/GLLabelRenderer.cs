using System;
using AGS.API;

using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class GLLabelRenderer : ILabelRenderer
	{
		private readonly GLImageRenderer _bgRenderer;
        private readonly IGLUtils _glUtils;
        private readonly IGraphicsBackend _graphics;
		private GLText _glTextRender, _glTextHitTest;

		private readonly IBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly AGSBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
		private readonly IGLViewportMatrixFactory _viewport;
		private AGSBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes;
		private readonly BitmapPool _bitmapPool;
        private readonly IFontLoader _fonts;
        private readonly Size _virtualResolution;
        private readonly IMessagePump _messagePump;
        private readonly GLMatrices[] _matricesPool;
        private readonly IRuntimeSettings _settings;
        private readonly BoundingBoxesEmptyBuilder _labelBoundingBoxFakeBuilder;
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private IObject _lastObject;

        private float _lastWidth = 1f, _lastHeight = 1f;

        public GLLabelRenderer(Dictionary<string, ITexture> textures, 
			IBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrixFactory viewportMatrix,
            AGSBoundingBoxes labelBoundingBoxes, AGSBoundingBoxes textBoundingBoxes, IGraphicsFactory graphicsFactory,
            IGLUtils glUtils, IGraphicsBackend graphics, IBitmapLoader bitmapLoader, IFontLoader fonts,
            IRuntimeSettings settings, IMessagePump messagePump, IGameState state, IGameEvents events)
		{
            _state = state;
            _events = events;
            Width = 1f;
            Height = 1f;
            _matricesPool = new GLMatrices[3];
            _messagePump = messagePump;
            OnLabelSizeChanged = new AGSEvent();
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
            _settings = settings;
            _labelBoundingBoxFakeBuilder = new BoundingBoxesEmptyBuilder();
			_bgRenderer = new GLImageRenderer(textures,
				colorBuilder, _textureRenderer, graphicsFactory, glUtils, bitmapLoader);

			_colorBuilder = colorBuilder;

			TextVisible = true;
            TextBackgroundVisible = true;
		}

        public bool TextVisible { get; set; }
        public bool TextBackgroundVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public SizeF BaseSize { get; set; }
		public ILabel Label { get; set; }
        public int CaretPosition { get; set; }
        public bool RenderCaret { get; set; }

        public float Width { get; private set; }

        public float Height { get; private set; }

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

        public IEvent OnLabelSizeChanged { get; private set; }

		#region IImageRenderer implementation

        public SizeF? CustomImageSize { get; private set; }
        public PointF? CustomImageResolutionFactor { get; private set; }
        public ICropSelfComponent CustomTextCrop { get; set; }
        public AGSBoundingBoxes TextBoundingBoxes { get { return _usedTextBoundingBoxes; }}

		public void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
            if (!TextBackgroundVisible && !TextVisible) return;
            if (_lastObject != obj)
            {
                AGSBoundingBoxComponent box = new AGSBoundingBoxComponent(_settings, _viewport, 
                                                                          _labelBoundingBoxFakeBuilder, _state, _events);
                obj.RemoveComponent<IBoundingBoxComponent>();
                obj.AddComponent(box);
                _lastObject = obj;
            }
            _glTextHitTest = _glTextHitTest ?? new GLText (_graphics, _messagePump, _fonts, _bitmapPool, false);
            _glTextRender = _glTextRender ?? new GLText(_graphics, _messagePump, _fonts, _bitmapPool, true);

			updateBoundingBoxes(obj, drawable, viewport);
            _labelBoundingBoxFakeBuilder.BoundingBoxes = _usedLabelBoundingBoxes;
			_bgRenderer.Prepare(obj, drawable, viewport);
            Width = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.RenderBox.Width;
            Height = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.RenderBox.Height;
		}

		public void Render(IObject obj, IViewport viewport)
		{
            if (!TextBackgroundVisible && !TextVisible) return;

            PointF resolutionFactor; Size resolution;
            AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, obj,
               new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY), out resolutionFactor,
               out resolution);
			PointF textScaleFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            if (!resolutionFactor.Equals(textScaleFactor))
            {
                _labelBoundingBoxFakeBuilder.CropScale = new PointF(1f / resolutionFactor.X, 1f / resolutionFactor.Y);
                resolutionFactor = AGSModelMatrixComponent.NoScaling;
            }
            else _labelBoundingBoxFakeBuilder.CropScale = AGSModelMatrixComponent.NoScaling;

            if (TextBackgroundVisible)
            {
                _bgRenderer.Render(obj, viewport);
            }

            if (TextVisible && Text != "")
            {
                _glTextHitTest.Refresh();
                if (!string.IsNullOrEmpty(Text)) _glUtils.AdjustResolution(resolution.Width, resolution.Height);

                IGLColor color = _colorBuilder.Build(Colors.White);
                var cropInfo = _usedTextBoundingBoxes.RenderBox.Crop(BoundingBoxType.Render, CustomTextCrop ?? obj.GetComponent<ICropSelfComponent>(), resolutionFactor, AGSModelMatrixComponent.NoScaling);
                if (cropInfo.Equals(default(AGSCropInfo))) return;
                _usedTextBoundingBoxes.RenderBox = cropInfo.BoundingBox;

                _textureRenderer.Render(_glTextHitTest.Texture, _usedTextBoundingBoxes.RenderBox, cropInfo.TextureBox, color);
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
            PointF textScaleFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, drawable,
                                                                                  textScaleFactor, out hitTestResolutionFactor,
                                                                                  out resolution);
			var scaleUpText = hitTestResolutionFactor;
			var scaleDownText = noFactor;
            if (!textScaleFactor.Equals(hitTestResolutionFactor))
            {
                textScaleFactor = noFactor;
                scaleDownText = hitTestResolutionFactor;
            }
            AutoFit autoFit = getAutoFit();
			float height = obj.Height;
			float width = obj.Width;

            if (autoFit == AutoFit.LabelShouldFitText)
            {
                updateText(_glTextHitTest, resolutionMatches, GLText.EmptySize, scaleUpText, scaleDownText, int.MaxValue);
                if (!resolutionMatches) updateText(_glTextRender, true, GLText.EmptySize, scaleUpText, scaleDownText, int.MaxValue);
                CustomImageSize = new SizeF(_glTextHitTest.Width, _glTextHitTest.Height);
            }
            else if (autoFit == AutoFit.TextShouldWrapAndLabelShouldFitHeight)
            {
                updateText(_glTextHitTest, resolutionMatches, new SizeF(BaseSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)BaseSize.Width);
                if (!resolutionMatches) updateText(_glTextRender, true, new SizeF(BaseSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)BaseSize.Width);
                CustomImageSize = new SizeF(BaseSize.Width, _glTextHitTest.Height);
            }
            else CustomImageSize = BaseSize;

            CustomImageResolutionFactor = hitTestResolutionFactor;
            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : _viewport.GetViewport(drawable.RenderLayer.Z).GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);

            var modelMatrices = obj.GetModelMatrices();

            IGLMatrices textRenderMatrices = acquireMatrix(0).SetMatrices(modelMatrices.InObjResolutionMatrix, viewportMatrix);
            IGLMatrices labelRenderMatrices = obj.RenderLayer.IndependentResolution != null ? textRenderMatrices : acquireMatrix(1).SetMatrices(modelMatrices.InVirtualResolutionMatrix, viewportMatrix);
            IGLMatrices textHitTestMatrices = resolutionMatches ? textRenderMatrices : obj.RenderLayer.IndependentResolution == null ? labelRenderMatrices : acquireMatrix(2).SetMatrices(modelMatrices.InVirtualResolutionMatrix, viewportMatrix);
            IGLMatrices labelHitTestMatrices = obj.RenderLayer.IndependentResolution == null ? labelRenderMatrices : textHitTestMatrices;

            if (textScaleFactor.Equals(hitTestResolutionFactor)) 
                hitTestResolutionFactor = noFactor;

            updateBoundingBoxes(_glTextHitTest, autoFit, textHitTestMatrices, labelHitTestMatrices, scaleUpText, noFactor, hitTestResolutionFactor, resolutionMatches, true);
            if (!resolutionMatches) updateBoundingBoxes(_glTextRender, autoFit, textRenderMatrices, labelRenderMatrices, scaleUpText, scaleDownText, noFactor, true, false);

            if (_lastWidth != Width || _lastHeight != Height)
            {
                OnLabelSizeChanged.Invoke(); 
            }
            _lastWidth = Width;
            _lastHeight = Height;
		}

        //A very simple "object pool" for the possible 3 matrices a label renderer can create, 
        //to avoid allocating the matrices object on each tick.
        //Each possible matrix creation has a cell reserved for it.
        private GLMatrices acquireMatrix(int index)
        {
            GLMatrices matrices = _matricesPool[index];
            if (matrices == null)
            {
                matrices = new GLMatrices();
                _matricesPool[index] = matrices;
            }
            return matrices;
        }

        private void updateBoundingBoxes(GLText glText, AutoFit autoFit, IGLMatrices textMatrices, IGLMatrices labelMatrices, PointF textScaleUp, PointF textScaleDown, PointF labelResolutionFactor, bool buildRenderBox, bool buildHitTestBox)
        {
            switch (autoFit)
            {
                case AutoFit.NoFitting:
                    build(_labelBoundingBoxes, BaseSize.Width / labelResolutionFactor.X, BaseSize.Height / labelResolutionFactor.Y, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, buildRenderBox, GLText.EmptySize, textScaleUp, textScaleDown, int.MaxValue);
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldWrapAndLabelShouldFitHeight:
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    build(_labelBoundingBoxes, BaseSize.Width / labelResolutionFactor.X, glText.Height / labelResolutionFactor.Y, labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldFitLabel:
                    build(_labelBoundingBoxes, BaseSize.Width / labelResolutionFactor.X, BaseSize.Height / labelResolutionFactor.Y, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, buildRenderBox, glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), textScaleUp, textScaleDown, int.MaxValue);

                    float textWidth = glText.Width < BaseSize.Width ? glText.BitmapWidth : MathUtils.Lerp(0f, 0f, glText.Width, BaseSize.Width, glText.BitmapWidth);
                    float textHeight = glText.Height < BaseSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, BaseSize.Height, glText.BitmapHeight);

                    build(_textBoundingBoxes, textWidth, textHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.LabelShouldFitText:
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    build(_labelBoundingBoxes, glText.Width / labelResolutionFactor.X, glText.Height / labelResolutionFactor.Y, labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldCrop:
                    build(_labelBoundingBoxes, BaseSize.Width / labelResolutionFactor.X, BaseSize.Height / labelResolutionFactor.Y, labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, buildRenderBox, glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), textScaleUp, textScaleDown, (int)BaseSize.Width, true);

                    float heightOfText = glText.Height < BaseSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, BaseSize.Height, glText.BitmapHeight);

                    build(_textBoundingBoxes, glText.BitmapWidth, heightOfText, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                default:
                    throw new NotSupportedException(autoFit.ToString());
            }
        }

        private void build(AGSBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox)
        {
            var intermediateBox = _boundingBoxBuilder.BuildIntermediateBox(width, height, matrices.ModelMatrix);
            if (buildHitTestBox)
            {
                boxes.HitTestBox = _boundingBoxBuilder.BuildHitTestBox(intermediateBox);
            }
            if (buildRenderBox)
            {
                PointF scale;
                boxes.RenderBox = _boundingBoxBuilder.BuildRenderBox(intermediateBox, matrices.ViewportMatrix, out scale);
            }
        }

        private void updateText(GLText glText, bool measureOnly, SizeF baseSize, PointF scaleUp, PointF scaleDown, int? maxWidth, bool cropText = false)
		{
			if (TextVisible)
			{
                if (Text == null) return;
                glText.SetProperties(baseSize, Text, Config, maxWidth, scaleUp, scaleDown, CaretPosition, RenderCaret, cropText, measureOnly);
			}
		}

		private class BoundingBoxesEmptyBuilder : IBoundingBoxBuilder
		{
            private AGSBoundingBoxes _boundingBoxes;
            private AGSBoundingBoxes _previousBoxes;

            public BoundingBoxesEmptyBuilder()
            {
                OnNewBoxBuildRequired = new AGSEvent();
            }

            public PointF CropScale { private get; set; }
            public AGSBoundingBoxes BoundingBoxes { private get { return _boundingBoxes; } 
                set
                {
                    if (value.Equals(_previousBoxes)) return;
                    _boundingBoxes = value;
                    _previousBoxes = _previousBoxes ?? new AGSBoundingBoxes();
                    _previousBoxes.CopyFrom(value);
                    OnNewBoxBuildRequired.Invoke();
                }
            }
            public IEvent OnNewBoxBuildRequired { get; private set; }

            #region AGSBoundingBoxBuilder implementation

            public AGSBoundingBox BuildIntermediateBox(float width, float height, Matrix4 modelMatrix)
            {
                return default(AGSBoundingBox);
            }

			public AGSBoundingBox BuildHitTestBox(AGSBoundingBox intermediateBox)
            {
                if (BoundingBoxes != null) return BoundingBoxes.HitTestBox;
                return default(AGSBoundingBox);
            }

			public AGSBoundingBox BuildRenderBox(AGSBoundingBox intermediateBox, Matrix4 viewportMatrix, out PointF scale)
            {
                scale = CropScale;
                if (BoundingBoxes != null) return BoundingBoxes.RenderBox;
				return default(AGSBoundingBox);
            }

			#endregion
		}
	}
}