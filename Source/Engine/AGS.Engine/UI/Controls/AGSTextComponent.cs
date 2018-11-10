using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSTextComponent : AGSComponent, ITextComponent, IRenderer, ILockStep
    {
        private IImageComponent _image;
        private IScaleComponent _scale;
        private IModelMatrixComponent _matrix;
        private IVisibleComponent _visible;
        private readonly IGameEvents _events;
        private readonly IRenderPipeline _pipeline;
        private SizeF _baseSize;
        private GLText _glTextRender, _glTextHitTest;
        private readonly AGSBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
        private AGSBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes, _afterCropTextBoundingBoxes;
        private readonly Size _virtualResolution;
        private ITextConfig _lastTextConfig;
        private PointF _lastTextScaleFactor;
        private ModelMatrices _lastMatrices;
        private Matrix4 _lastViewportMatrix;
        private bool _shouldUpdateBoundingBoxes;
        private readonly IBoundingBoxBuilder _boundingBoxBuilder;
        private readonly GLMatrices[] _matricesPool;
        private readonly IGameState _state;
        private readonly IGraphicsBackend _graphics;
        private readonly IRenderMessagePump _messagePump;
        private readonly BitmapPool _bitmapPool;
        private readonly IFontFactory _fonts;
        private readonly BoundingBoxesEmptyBuilder _labelBoundingBoxFakeBuilder;
        private readonly IRuntimeSettings _settings;
        private readonly ObjectPool<Instruction> _instructionPool;
        private float _lastWidth = 1f, _lastHeight = 1f;
        private bool _isGuaranteedToFullyCrop;
        private int _pendingLocks;

        private IDrawableInfoComponent _drawable;
        private ICropSelfComponent _cropSelf;

        public AGSTextComponent(IRenderPipeline pipeline, IBoundingBoxBuilder boundingBoxBuilder,
            IGLTextureRenderer textureRenderer, BitmapPool bitmapPool,
            AGSBoundingBoxes labelBoundingBoxes, AGSBoundingBoxes textBoundingBoxes,
            IGLUtils glUtils, IGraphicsBackend graphics, IFontFactory fonts,
            IRuntimeSettings settings, IRenderMessagePump messagePump, IGameState state, IGameEvents events)
        {
            _pipeline = pipeline;
            _afterCropTextBoundingBoxes = new AGSBoundingBoxes();
            _state = state;
            _events = events;
            Width = 1f;
            Height = 1f;
            _matricesPool = new GLMatrices[3];
            _messagePump = messagePump;
            OnLabelSizeChanged = new AGSEvent();
            _graphics = graphics;
            _fonts = fonts;
            _bitmapPool = bitmapPool;
            _labelBoundingBoxes = labelBoundingBoxes;
            _textBoundingBoxes = textBoundingBoxes;
            _boundingBoxBuilder = boundingBoxBuilder;
            _virtualResolution = settings.VirtualResolution;
            _settings = settings;
            _labelBoundingBoxFakeBuilder = new BoundingBoxesEmptyBuilder();

            _instructionPool = new ObjectPool<Instruction>(pool => new Instruction(pool, glUtils, textureRenderer, _glTextHitTest), 0);

            TextVisible = true;

            subscribeTextConfigChanges();
            PropertyChanged += onPropertyChanged;
            _shouldUpdateBoundingBoxes = true;
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            Entity.Bind<IScaleComponent>(c => { _scale = c; c.PropertyChanged += onScalePropertyChanged; }, c => { _scale = null; c.PropertyChanged -= onScalePropertyChanged; });
            Entity.Bind<IDrawableInfoComponent>(c => { _drawable = c; c.PropertyChanged += onDrawablePropertyChanged; }, c => { _drawable = null; c.PropertyChanged -= onDrawablePropertyChanged; });
            Entity.Bind<ICropSelfComponent>(c => _cropSelf = c, _ => _cropSelf = null);
            Entity.Bind<IModelMatrixComponent>(c => { _matrix = c; c.OnMatrixChanged.Subscribe(onMatrixChanged); }, c => { _matrix = null; c.OnMatrixChanged.Unsubscribe(onMatrixChanged); });
            Entity.Bind<IVisibleComponent>(c => _visible = c, _ => _visible = null);
            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            _pipeline.Subscribe(Entity.ID, this, -10);
        }

		public override void AfterInit()
		{
            base.AfterInit();
            replaceBoundingBox();
		}

		private void replaceBoundingBox()
        {
            IBlockingEvent boxChangeEvent = Entity.GetComponent<IBoundingBoxComponent>()?.OnBoundingBoxesChanged ?? new AGSEvent();
            AGSBoundingBoxComponent box = new AGSBoundingBoxComponent(_settings, _labelBoundingBoxFakeBuilder, _state, _events, boxChangeEvent);
            Entity.RemoveComponent<IBoundingBoxComponent>();
            Entity.AddComponent<IBoundingBoxComponent>(box);
            box.Init(Entity, typeof(IBoundingBoxComponent));
            onBoundingBoxShouldChange();
        }

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            PropertyChanged -= onPropertyChanged;
            unsubscribeTextConfigChanges();
            var entity = Entity;
            if (entity != null)
            {
                _pipeline.Unsubscribe(entity.ID, this);
            }
            CustomTextCrop = null;
            _instructionPool?.Dispose();
        }

        public ITextConfig TextConfig { get; set; }

        public string Text { get; set; }

        public bool TextVisible { get; set; }

        public bool TextBackgroundVisible { get => _image?.IsImageVisible ?? false; set => _image.IsImageVisible = value; }

        [DoNotNotify]
        public SizeF LabelRenderSize
        {
            get => _baseSize;
            set
            {
                bool hasChanged = !_baseSize.Equals(value);
                _baseSize = value;
                if (hasChanged) OnPropertyChanged(nameof(LabelRenderSize));
                var obj = _image;
                if (obj != null && obj.Image == null) obj.Image = new EmptyImage(value.Width, value.Height);                
            }
        }

        public float TextWidth => _glTextHitTest == null ? 1f : _glTextHitTest.Width;

        public float TextHeight => _glTextHitTest == null ? 1f : _glTextHitTest.Height;

        public float Width { get; private set; }

        public float Height { get; private set; }

        public IBlockingEvent OnLabelSizeChanged { get; }

        public SizeF? CustomImageSize { get; private set; }
        public PointF? CustomImageResolutionFactor { get; private set; }
        public ICropSelfComponent CustomTextCrop { get; set; }
        public AGSBoundingBoxes TextBoundingBoxes => _usedTextBoundingBoxes;

        public int CaretPosition { get; set; }
        public bool RenderCaret { get; set; }
        public int CaretXOffset { get; set; } = -1;

        [Property(Browsable = false)]
        public ILockStep TextLockStep { get { return this; } }

        public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            if (!TextVisible || _usedTextBoundingBoxes == null || _isGuaranteedToFullyCrop) return null;

            if (!_afterCropTextBoundingBoxes.ViewportBox.IsValid)
                return null;

            Size resolution;
            PointF textScaleFactor = new PointF(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, _drawable,
               textScaleFactor, out _, out resolution);

            var instruction = _instructionPool.Acquire();
            if (instruction == null) return null;
            instruction.Setup(resolution, _afterCropTextBoundingBoxes);
            return instruction;
        }

        public void PrepareTextBoundingBoxes()
        {
            if (_pendingLocks > 0) return;
            if (!prepare(_state.Viewport)) //todo: support multiple viewports
            {
                return;
            }

            if (_usedTextBoundingBoxes == null) return;
            var cropInfo = _usedTextBoundingBoxes.ViewportBox.Crop(BoundingBoxType.Render, CustomTextCrop ?? _cropSelf, AGSModelMatrixComponent.NoScaling);

            _afterCropTextBoundingBoxes.ViewportBox = cropInfo.BoundingBox;
            _afterCropTextBoundingBoxes.TextureBox = cropInfo.TextureBox;
        }

        public void Lock()
        {
            Interlocked.Increment(ref _pendingLocks);
        }

        public void PrepareForUnlock() {}

        public void Unlock()
        {
            if (Interlocked.Decrement(ref _pendingLocks) > 0) return;
            PrepareTextBoundingBoxes();
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            onBoundingBoxShouldChange();
            if (e.PropertyName == nameof(TextConfig))
                subscribeTextConfigChanges();
        }

        private void onRepeatedlyExecute()
        {
            if (TextConfig == null) return;
            updateSize();
            PrepareTextBoundingBoxes();
        }

        private void updateSize()
        {
            var config = TextConfig;
            if (config == null) return;
            if (config.AutoFit != AutoFit.LabelShouldFitText &&
                config.AutoFit != AutoFit.TextShouldWrapAndLabelShouldFitHeight) return;
            var scale = _scale;
            if (scale == null) return;
            scale.BaseSize = new SizeF(Width, Height);
        }

        private bool prepare(IViewport viewport)
        {
            if (!TextBackgroundVisible && !TextVisible) return false;
            if (!(_visible?.Visible ?? false)) return false;
            var crop = CustomTextCrop ?? _cropSelf;
            _isGuaranteedToFullyCrop = crop?.IsGuaranteedToFullyCrop() ?? false;
            if (_isGuaranteedToFullyCrop)
            {
                return false;
            }

            _glTextHitTest = _glTextHitTest ?? new GLText(_graphics, _messagePump, _fonts, _settings.Defaults.TextFont, _bitmapPool, false);
            _glTextRender = _glTextRender ?? new GLText(_graphics, _messagePump, _fonts, _settings.Defaults.TextFont, _bitmapPool, true);

            if (!updateBoundingBoxes(viewport))
            {
                return false;
            }
            if (_usedLabelBoundingBoxes != null)
            {
                _labelBoundingBoxFakeBuilder.BoundingBoxes = _usedLabelBoundingBoxes;
            }
            Width = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.ViewportBox.Width;
            Height = _usedLabelBoundingBoxes == null ? 1f : _usedLabelBoundingBoxes.ViewportBox.Height;
            return true;
        }

        private AutoFit getAutoFit() => TextConfig?.AutoFit ?? AutoFit.NoFitting;

        private bool updateBoundingBoxes(IViewport viewport)
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
            resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _virtualResolution, _drawable,
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
            float height = _scale.Height;
            float width = _scale.Width;

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
                updateText(_glTextHitTest, false, new SizeF(LabelRenderSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)LabelRenderSize.Width);
                if (!resolutionMatches) updateText(_glTextRender, true, new SizeF(LabelRenderSize.Width, GLText.EmptySize.Height), scaleUpText, scaleDownText, (int)LabelRenderSize.Width);
                CustomImageSize = new SizeF(LabelRenderSize.Width, _glTextHitTest.Height);
            }
            else CustomImageSize = LabelRenderSize;

            CustomImageResolutionFactor = hitTestResolutionFactor;
            var viewportMatrix = _drawable.IgnoreViewport ? Matrix4.Identity : viewport.GetMatrix(_drawable.RenderLayer);
            if (!viewportMatrix.Equals(_lastViewportMatrix))
            {
                _lastViewportMatrix = viewportMatrix;
                onBoundingBoxShouldChange();
            }

            var modelMatrices = _matrix.GetModelMatrices();
            if (!modelMatrices.Equals(ref _lastMatrices))
            {
                _lastMatrices = modelMatrices;
                onBoundingBoxShouldChange();
            }
            if (!shouldUpdateBoundingBoxes)
            {
                return false;
            }

            IGLMatrices textRenderMatrices = acquireMatrix(0).SetMatrices(modelMatrices.InObjResolutionMatrix, viewportMatrix);
            IGLMatrices labelRenderMatrices = _drawable.RenderLayer?.IndependentResolution != null ? textRenderMatrices : acquireMatrix(1).SetMatrices(modelMatrices.InVirtualResolutionMatrix, viewportMatrix);
            IGLMatrices textHitTestMatrices = resolutionMatches ? textRenderMatrices : _drawable.RenderLayer?.IndependentResolution == null ? labelRenderMatrices : acquireMatrix(2).SetMatrices(modelMatrices.InVirtualResolutionMatrix, viewportMatrix);
            IGLMatrices labelHitTestMatrices = _drawable.RenderLayer?.IndependentResolution == null ? labelRenderMatrices : textHitTestMatrices;

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

            return true;
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
                    build(_labelBoundingBoxes, clampWidth(LabelRenderSize.Width / labelResolutionFactor.X), clampHeight(LabelRenderSize.Height / labelResolutionFactor.Y), labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, !buildHitTestBox, LabelRenderSize, textScaleUp, textScaleDown, int.MaxValue);
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldWrapAndLabelShouldFitHeight:
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    build(_labelBoundingBoxes, clampWidth(LabelRenderSize.Width / labelResolutionFactor.X), clampHeight(glText.Height / labelResolutionFactor.Y), labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldFitLabel:
                    build(_labelBoundingBoxes, clampWidth(LabelRenderSize.Width / labelResolutionFactor.X), clampHeight(LabelRenderSize.Height / labelResolutionFactor.Y), labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, !buildHitTestBox, glText.Width > LabelRenderSize.Width ? new SizeF(0f, LabelRenderSize.Height) : LabelRenderSize, textScaleUp, textScaleDown, int.MaxValue);

                    float textWidth = glText.Width < LabelRenderSize.Width ? glText.BitmapWidth : MathUtils.Lerp(0f, 0f, glText.Width, LabelRenderSize.Width, glText.BitmapWidth);
                    float textHeight = glText.Height < LabelRenderSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, LabelRenderSize.Height, glText.BitmapHeight);

                    build(_textBoundingBoxes, textWidth, textHeight, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.LabelShouldFitText:
                    build(_textBoundingBoxes, glText.BitmapWidth, glText.BitmapHeight, textMatrices, buildRenderBox, buildHitTestBox);
                    build(_labelBoundingBoxes, clampWidth(glText.Width / labelResolutionFactor.X), clampHeight(glText.Height / labelResolutionFactor.Y), labelMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                case AutoFit.TextShouldCrop:
                    build(_labelBoundingBoxes, clampWidth(LabelRenderSize.Width / labelResolutionFactor.X), clampHeight(LabelRenderSize.Height / labelResolutionFactor.Y), labelMatrices, buildRenderBox, buildHitTestBox);
                    updateText(glText, !buildHitTestBox, glText.Width > LabelRenderSize.Width ? GLText.EmptySize : new SizeF(LabelRenderSize.Width, GLText.EmptySize.Height), textScaleUp, textScaleDown, (int)LabelRenderSize.Width, true);

                    float heightOfText = glText.Height < LabelRenderSize.Height ? glText.BitmapHeight : MathUtils.Lerp(0f, 0f, glText.Height, LabelRenderSize.Height, glText.BitmapHeight);

                    build(_textBoundingBoxes, glText.BitmapWidth, heightOfText, textMatrices, buildRenderBox, buildHitTestBox);

                    _usedLabelBoundingBoxes = _labelBoundingBoxes;
                    _usedTextBoundingBoxes = _textBoundingBoxes;
                    break;

                default:
                    throw new NotSupportedException(autoFit.ToString());
            }
        }

        private float clampWidth(float width) => clamp(TextConfig.LabelMinSize?.Width, width);

        private float clampHeight(float height) => clamp(TextConfig.LabelMinSize?.Height, height);

        private float clamp(float? minValue, float value)
        {
            if (minValue is null || value > minValue) return value;
            return minValue.Value;
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
                if (glText.SetProperties(baseSize, Text, TextConfig, maxWidth, scaleUp, scaleDown, CaretPosition, CaretXOffset, RenderCaret, cropText, measureOnly))
                {
                    onBoundingBoxShouldChange();
                }
            }
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

        private void subscribeTextConfigChanges()
        {
            unsubscribeTextConfigChanges();
            var config = TextConfig;
            _lastTextConfig = config;
            if (config != null) config.PropertyChanged += onTextConfigPropertyChanged;
        }

        private void unsubscribeTextConfigChanges()
        {
            var config = _lastTextConfig;
            if (config != null) config.PropertyChanged -= onTextConfigPropertyChanged;
        }

        private void onTextConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            onBoundingBoxShouldChange();
        }

        private void onBoundingBoxShouldChange()
        {
            _shouldUpdateBoundingBoxes = true;
        }

        private class Instruction : IRenderInstruction
        {
            private static readonly GLColor _white = new GLColor().Build(Colors.White);
            private readonly ObjectPool<Instruction> _instructionPool;
            private readonly IGLUtils _utils;
            private readonly IGLTextureRenderer _renderer;
            private readonly GLText _text;

            private Size _resolution;
            private AGSBoundingBoxes _afterCropBoxes;

            public Instruction(ObjectPool<Instruction> instructionPool, IGLUtils utils, 
                               IGLTextureRenderer renderer, GLText text)
            {
                _instructionPool = instructionPool;
                _renderer = renderer;
                _utils = utils;
                _text = text;
            }

            public void Setup(Size resolution, AGSBoundingBoxes afterCropBoxes)
            {
                _resolution = resolution;
                _afterCropBoxes = afterCropBoxes;
            }

            public void Release()
            {
                _instructionPool.Release(this);
            }

            public void Render()
            {
                _text.Refresh();
                var currentResolution = _utils.CurrentResolution;
                _utils.AdjustResolution(_resolution.Width, _resolution.Height);

                _renderer.Render(_text.Texture, _afterCropBoxes, _white);

                _utils.AdjustResolution((int)currentResolution.Width, (int)currentResolution.Height);
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

            public AGSBoundingBoxes BoundingBoxes
            {
                private get { return _boundingBoxes; }
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
