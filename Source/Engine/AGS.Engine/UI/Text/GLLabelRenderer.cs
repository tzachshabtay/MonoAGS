using System;
using AGS.API;

using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

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
		private AGSBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes, _afterCropTextBoundingBoxes;
		private readonly BitmapPool _bitmapPool;
        private readonly IFontLoader _fonts;
        private readonly Size _virtualResolution;
        private readonly IRenderMessagePump _messagePump;
        private readonly GLMatrices[] _matricesPool;
        private readonly IRuntimeSettings _settings;
        private readonly BoundingBoxesEmptyBuilder _labelBoundingBoxFakeBuilder;
        private readonly IGameState _state;
        private readonly IGameEvents _events;

        private ITextConfig _lastTextConfig;
        private IObject _lastObject;
        private PointF _lastTextScaleFactor;
        private ModelMatrices _lastMatrices;
        private Matrix4 _lastViewportMatrix;

        private List<IComponentBinding> _bindings;
        private bool _shouldUpdateBoundingBoxes;
        private AGSCropInfo _defaultCrop = default(AGSCropInfo);

        private float _lastWidth = 1f, _lastHeight = 1f;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public GLLabelRenderer(ITextureCache textures, ITextureFactory textureFactory,
			IBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool,
            AGSBoundingBoxes labelBoundingBoxes, AGSBoundingBoxes textBoundingBoxes,
            IGLUtils glUtils, IGraphicsBackend graphics, IFontLoader fonts,
            IRuntimeSettings settings, IRenderMessagePump messagePump, IGameState state, IGameEvents events)
		{
            _bindings = new List<IComponentBinding>();
            _afterCropTextBoundingBoxes = new AGSBoundingBoxes();
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
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
            _virtualResolution = settings.VirtualResolution;
            _settings = settings;
            _labelBoundingBoxFakeBuilder = new BoundingBoxesEmptyBuilder();
			_bgRenderer = new GLImageRenderer(textures, textureFactory, colorBuilder, _textureRenderer, glUtils);

			_colorBuilder = colorBuilder;

			TextVisible = true;
            TextBackgroundVisible = true;

            subscribeTextConfigChanges();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(TextBackgroundVisible)) return;
                onBoundingBoxShouldChange();
                if (e.PropertyName == nameof(Config)) subscribeTextConfigChanges();
            };
            _shouldUpdateBoundingBoxes = true;
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

        public float TextWidth => _glTextHitTest == null ? 1f : _glTextHitTest.Width;

        public float TextHeight => _glTextHitTest == null ? 1f : _glTextHitTest.Height;

        public IBlockingEvent OnLabelSizeChanged { get; }

		#region IImageRenderer implementation

        public SizeF? CustomImageSize { get; private set; }
        public PointF? CustomImageResolutionFactor { get; private set; }
        public ICropSelfComponent CustomTextCrop { get; set; }
        public AGSBoundingBoxes TextBoundingBoxes => _usedTextBoundingBoxes;

        public void Prepare(IObject obj, IDrawableInfoComponent drawable, IViewport viewport)
		{
            if (!TextBackgroundVisible && !TextVisible) return;

            if (_lastObject != obj)
            {
                IBlockingEvent boxChangeEvent = obj.GetComponent<IBoundingBoxComponent>()?.OnBoundingBoxesChanged ?? new AGSEvent();
                AGSBoundingBoxComponent box = new AGSBoundingBoxComponent(_settings, _labelBoundingBoxFakeBuilder, _state, _events, boxChangeEvent);
                obj.RemoveComponent<IBoundingBoxComponent>();
                obj.AddComponent<IBoundingBoxComponent>(box);
                _lastObject = obj;
                foreach (var binding in _bindings)
                {
                    binding?.Unbind();
                }
                var scaleBinding = obj.Bind<IScaleComponent>(c => c.PropertyChanged += onScalePropertyChanged, c => c.PropertyChanged -= onScalePropertyChanged);
                var matrixBinding = obj.Bind<IModelMatrixComponent>(c => c.OnMatrixChanged.Subscribe(onMatrixChanged), c => c.OnMatrixChanged.Unsubscribe(onMatrixChanged));
                var drawableBinding = obj.Bind<IDrawableInfoComponent>(c => c.PropertyChanged += onDrawablePropertyChanged, c => c.PropertyChanged -= onDrawablePropertyChanged);
                onBoundingBoxShouldChange();
                _bindings.Clear();
                _bindings.Add(scaleBinding);
                _bindings.Add(matrixBinding);
                _bindings.Add(drawableBinding);
            }
            _glTextHitTest = _glTextHitTest ?? new GLText(_graphics, _messagePump, _fonts, _bitmapPool, false);
            _glTextRender = _glTextRender ?? new GLText(_graphics, _messagePump, _fonts, _bitmapPool, true);

            updateBoundingBoxes(obj, drawable, viewport);
            if (_usedLabelBoundingBoxes != null)
            {
                _labelBoundingBoxFakeBuilder.BoundingBoxes = _usedLabelBoundingBoxes;
            }
            _bgRenderer.Prepare(obj, drawable, viewport);
            Width = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.ViewportBox.Width;
            Height = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.ViewportBox.Height;
		}

        public void Render(IObject obj, IViewport viewport)
        {
            if (!TextBackgroundVisible && !TextVisible) return;
            if (!obj.Visible || _usedTextBoundingBoxes == null) return;

            PointF resolutionFactor; Size resolution;
            PointF textScaleFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, obj,
               textScaleFactor, out resolutionFactor, out resolution);
            if (!resolutionFactor.Equals(textScaleFactor))
            {
                resolutionFactor = AGSModelMatrixComponent.NoScaling;
            }

            if (TextBackgroundVisible)
            {
                _bgRenderer.Render(obj, viewport);
            }

            if (TextVisible && (!string.IsNullOrEmpty(Text) || RenderCaret))
            {
                _glTextHitTest.Refresh();
                _glUtils.AdjustResolution(resolution.Width, resolution.Height);

                IGLColor color = _colorBuilder.Build(Colors.White);
                var cropInfo = _usedTextBoundingBoxes.ViewportBox.Crop(BoundingBoxType.Render, CustomTextCrop ?? obj.GetComponent<ICropSelfComponent>(), AGSModelMatrixComponent.NoScaling);
                if (cropInfo.Equals(_defaultCrop)) return;

                _afterCropTextBoundingBoxes.ViewportBox = cropInfo.BoundingBox;
                _afterCropTextBoundingBoxes.TextureBox = cropInfo.TextureBox;

                _textureRenderer.Render(_glTextHitTest.Texture, _afterCropTextBoundingBoxes, color);
            }
		}

        #endregion

        private void subscribeTextConfigChanges()
        {
            var config = _lastTextConfig;
            if (config != null) config.PropertyChanged -= onTextConfigPropertyChanged;
            config = Config;
            _lastTextConfig = config;
            if (config != null) config.PropertyChanged += onTextConfigPropertyChanged;
        }

        private void onTextConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            onBoundingBoxShouldChange();
        }

        private void onBoundingBoxShouldChange()
        {
            _shouldUpdateBoundingBoxes = true;
        }

        private void onDrawablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IDrawableInfoComponent.RenderLayer) && e.PropertyName != nameof(IDrawableInfoComponent.IgnoreViewport))
                return;
            onBoundingBoxShouldChange();
        }

        private void onMatrixChanged() => onBoundingBoxShouldChange();

        private void onScalePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IScaleComponent.Width) && e.PropertyName != nameof(IScaleComponent.Height))
                return;
            onBoundingBoxShouldChange();
        }

        private AutoFit getAutoFit()
        {
            ITextConfig config = Config;
            return TextVisible && config != null ? config.AutoFit : AutoFit.NoFitting;
        }

        private void updateBoundingBoxes(IObject obj, IDrawableInfoComponent drawable, IViewport viewport)
		{
            var noFactor = AGSModelMatrixComponent.NoScaling;
            bool resolutionMatches;
            PointF hitTestResolutionFactor;
            Size resolution;
            PointF textScaleFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            if (!textScaleFactor.Equals(_lastTextScaleFactor))
            {
                _lastTextScaleFactor = textScaleFactor;
                onBoundingBoxShouldChange();
            }
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

            bool shouldUpdateBoundingBoxes = _shouldUpdateBoundingBoxes;
            _shouldUpdateBoundingBoxes = false;
            if (autoFit == AutoFit.LabelShouldFitText)
            {
                updateText(_glTextHitTest, false, GLText.EmptySize, scaleUpText, scaleDownText, int.MaxValue);
                if (!resolutionMatches) updateText(_glTextRender, true, GLText.EmptySize, scaleUpText, scaleDownText, int.MaxValue);
                CustomImageSize = new SizeF(_glTextHitTest.Width, _glTextHitTest.Height);
            }
            else if (autoFit == AutoFit.TextShouldWrapAndLabelShouldFitHeight)
            {
                updateText(_glTextHitTest, false, new SizeF(BaseSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)BaseSize.Width);
                if (!resolutionMatches) updateText(_glTextRender, true, new SizeF(BaseSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)BaseSize.Width);
                CustomImageSize = new SizeF(BaseSize.Width, _glTextHitTest.Height);
            }
            else CustomImageSize = BaseSize;

            CustomImageResolutionFactor = hitTestResolutionFactor;
            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : viewport.GetMatrix(drawable.RenderLayer);
            if (!viewportMatrix.Equals(_lastViewportMatrix))
            {
                _lastViewportMatrix = viewportMatrix;
                onBoundingBoxShouldChange();
            }

            var modelMatrices = obj.GetModelMatrices();
            if (!modelMatrices.Equals(_lastMatrices))
            {
                _lastMatrices = modelMatrices;
                onBoundingBoxShouldChange();
            }
            if (!shouldUpdateBoundingBoxes)
            {
                return;
            }

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
                    updateText(glText, !buildHitTestBox, BaseSize, textScaleUp, textScaleDown, int.MaxValue);
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
                    updateText(glText, !buildHitTestBox, glText.Width > BaseSize.Width ? new SizeF(0f, BaseSize.Height) : BaseSize, textScaleUp, textScaleDown, int.MaxValue);

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
                    updateText(glText, !buildHitTestBox, glText.Width > BaseSize.Width ? GLText.EmptySize : new SizeF(BaseSize.Width, GLText.EmptySize.Height), textScaleUp, textScaleDown, (int)BaseSize.Width, true);

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
            var modelMatrix = matrices.ModelMatrix;
            var intermediateBox = _boundingBoxBuilder.BuildIntermediateBox(width, height, ref modelMatrix);
            if (buildHitTestBox)
            {
                boxes.WorldBox = _boundingBoxBuilder.BuildHitTestBox(ref intermediateBox);
            }
            if (buildRenderBox)
            {
                PointF scale;
                var viewportMatrix = matrices.ViewportMatrix;
                boxes.ViewportBox = _boundingBoxBuilder.BuildRenderBox(ref intermediateBox, ref viewportMatrix, out scale);
            }
        }

        private void updateText(GLText glText, bool measureOnly, SizeF baseSize, PointF scaleUp, PointF scaleDown, int? maxWidth, bool cropText = false)
		{
			if (TextVisible)
			{
                if (Text == null) return;
                if (glText.SetProperties(baseSize, Text, Config, maxWidth, scaleUp, scaleDown, CaretPosition, RenderCaret, cropText, measureOnly))
                {
                    onBoundingBoxShouldChange();
                }
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
            public IBlockingEvent OnNewBoxBuildRequired { get; }

            #region AGSBoundingBoxBuilder implementation

            public AGSBoundingBox BuildIntermediateBox(float width, float height, ref Matrix4 modelMatrix) => default;

            public AGSBoundingBox BuildHitTestBox(ref AGSBoundingBox intermediateBox) => BoundingBoxes?.WorldBox ?? default;

            public AGSBoundingBox BuildRenderBox(ref AGSBoundingBox intermediateBox, ref Matrix4 viewportMatrix, out PointF scale)
            {
                scale = AGSModelMatrixComponent.NoScaling;
                return BoundingBoxes?.ViewportBox ?? default;
            }

			#endregion
		}
	}
}
