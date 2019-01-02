using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSBoundingBoxComponent : AGSComponent, IBoundingBoxComponent, ILockStep
    {
        private bool _isHitTestBoxDirty, _isCropDirty, _areViewportsDirty;
        private int _shouldFireOnUnlock, _pendingLocks;
        private ViewportBoundingBoxes _mainViewportBoundingBoxes;
        private ConcurrentDictionary<IViewport, ViewportBoundingBoxes> _boundingBoxes;
        private AGSBoundingBox _hitTestBox, _intermediateBox;
        private IModelMatrixComponent _matrix;
        private ICropSelfComponent _crop;
        private IImageComponent _image;
        private IScaleComponent _scale;
        private IDrawableInfoComponent _drawable;
        private ITextureOffsetComponent _textureOffset;
        private IGameSettings _settings;
        private readonly IBoundingBoxBuilder _boundingBoxBuilder;
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private readonly AGSCropInfo _defaultCropInfo = default;
        private readonly Func<IViewport, ViewportBoundingBoxes> _createNewViewportBoundingBoxes;
        private readonly Action _onHitTextBoxShouldChangeCallback;
        private readonly PropertyChangedEventHandler _onCropShouldChangeCallback;
        private readonly PropertyChangedEventHandler _onImageChangedCallback;
        private readonly PropertyChangedEventHandler _onDrawableChangedCallback;
        private readonly PropertyChangedEventHandler _onTextureOffsetChangedCallback;

        public AGSBoundingBoxComponent(IRuntimeSettings settings,
                                       IBoundingBoxBuilder boundingBoxBuilder, IGameState state, IGameEvents events,
                                       IBlockingEvent onBoundingBoxChanged)
        {
            _createNewViewportBoundingBoxes = viewport => new ViewportBoundingBoxes(viewport);
            _boundingBoxes = new ConcurrentDictionary<IViewport, ViewportBoundingBoxes>(new IdentityEqualityComparer<IViewport>());
            _mainViewportBoundingBoxes = new ViewportBoundingBoxes(state.Viewport);
            _settings = settings;
            _events = events;
            _state = state;
            OnBoundingBoxesChanged = onBoundingBoxChanged;
            _boundingBoxBuilder = boundingBoxBuilder;

            _onHitTextBoxShouldChangeCallback = onHitTextBoxShouldChange;
            _onCropShouldChangeCallback = onCropShouldChange;
            _onImageChangedCallback = onImageChanged;
            _onDrawableChangedCallback = onDrawableChanged;
            _onTextureOffsetChangedCallback = onTextureOffsetChanged;

            boundingBoxBuilder.OnNewBoxBuildRequired.Subscribe(_onHitTextBoxShouldChangeCallback);
            events.OnRoomChanging.Subscribe(_onHitTextBoxShouldChangeCallback);
            onHitTextBoxShouldChange();
        }

        public IBlockingEvent OnBoundingBoxesChanged { get; }

        [Property(Browsable = false)]
        public ILockStep BoundingBoxLockStep => this;

        [Property(Category = "World position")]
        public AGSBoundingBox WorldBoundingBox => _pendingLocks > 0 ? GetBoundingBoxes(_state.Viewport).WorldBox : _hitTestBox; 

        public override void Init()
        {
            base.Init();

            Entity.Bind<IModelMatrixComponent>(c => { c.OnMatrixChanged.Subscribe(_onHitTextBoxShouldChangeCallback); _matrix = c; },
                                               c => { c.OnMatrixChanged.Unsubscribe(_onHitTextBoxShouldChangeCallback); _matrix = null; });
            Entity.Bind<ICropSelfComponent>(c => { c.PropertyChanged += _onCropShouldChangeCallback; _crop = c; },
                                            c => { c.PropertyChanged -= _onCropShouldChangeCallback; _crop = null; });
            Entity.Bind<IImageComponent>(c => { _image = c; c.PropertyChanged += _onImageChangedCallback; },
                                         c => { _image = null; c.PropertyChanged -= _onImageChangedCallback; });
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IDrawableInfoComponent>(c => { c.PropertyChanged += _onDrawableChangedCallback; _drawable = c; },
                                                c => { c.PropertyChanged -= _onDrawableChangedCallback; _drawable = null; });
            Entity.Bind<ITextureOffsetComponent>(c => { c.PropertyChanged += _onTextureOffsetChangedCallback; _textureOffset = c; onAllViewportsShouldChange(); },
                                                 c => { c.PropertyChanged -= _onTextureOffsetChangedCallback; _textureOffset = null; onAllViewportsShouldChange(); });
        }

        public override void Dispose()
        {
            base.Dispose();
            _mainViewportBoundingBoxes?.Dispose();
            foreach (var viewportBox in _boundingBoxes?.Values ?? new List<ViewportBoundingBoxes>())
            {
                viewportBox?.Dispose();
            }
            _boundingBoxBuilder?.OnNewBoxBuildRequired?.Unsubscribe(_onHitTextBoxShouldChangeCallback);
            _events?.OnRoomChanging?.Unsubscribe(_onHitTextBoxShouldChangeCallback);
        }

        public void Lock()
        {
            lockBoxes(_mainViewportBoundingBoxes);
            foreach (var boxes in _boundingBoxes.Values)
            {
                lockBoxes(boxes);
            }
            Interlocked.Increment(ref _pendingLocks);
        }

        private void lockBoxes(ViewportBoundingBoxes boxes)
        {
            boxes.PreLockBoundingBoxes = new AGSBoundingBoxes
            {
                WorldBox = boxes.BoundingBoxes.WorldBox,
                PreCropViewportBox = boxes.BoundingBoxes.PreCropViewportBox,
                ViewportBox = boxes.BoundingBoxes.ViewportBox,
                TextureBox = boxes.BoundingBoxes.TextureBox
            };
        }

        public void PrepareForUnlock()
        {
            bool isDirty = anyChangesForLock();
            _shouldFireOnUnlock += (isDirty ? 1 : 0);
            if (!isDirty) return;
            recalculate(_state.Viewport, _mainViewportBoundingBoxes);
            foreach (var viewportBoxes in _boundingBoxes)
            {
                recalculate(viewportBoxes.Key, viewportBoxes.Value);
            }
        }

        public void Unlock()
        {
            if (Interlocked.Decrement(ref _pendingLocks) > 0) return;
            if (Interlocked.Exchange(ref _shouldFireOnUnlock, 0) >= 1)
            {
                OnBoundingBoxesChanged.Invoke();
            }
        }

        private bool anyChangesForLock()
        {
            return (_isHitTestBoxDirty || _isCropDirty || _areViewportsDirty ||
                    (!(_drawable?.IgnoreViewport ?? false) && (_mainViewportBoundingBoxes.IsDirty || _boundingBoxes.Values.Any(v => v.IsDirty))));
        }

        private ViewportBoundingBoxes getViewportBoundingBoxes(IViewport viewport)
        {
            if (viewport == _state.Viewport)
            {
                return _mainViewportBoundingBoxes;
            }
            return _boundingBoxes.GetOrAdd(viewport, _createNewViewportBoundingBoxes);
        }

        public AGSBoundingBoxes GetBoundingBoxes(IViewport viewport)
        {
            var viewportBoxes = getViewportBoundingBoxes(viewport);
            if (_pendingLocks > 0)
            {
                return viewportBoxes.PreLockBoundingBoxes;
            }
            var boundingBoxes = viewportBoxes.BoundingBoxes;
            if (!_isHitTestBoxDirty && !_isCropDirty && !_areViewportsDirty && !viewportBoxes.IsDirty)
                return boundingBoxes;
            if (!_isHitTestBoxDirty && !_isCropDirty && !_areViewportsDirty)
            {
                if (!viewportBoxes.IsDirty) return boundingBoxes;
                if (_drawable?.IgnoreViewport ?? false) return boundingBoxes;
            }
            var drawable = _drawable;
            var matrix = _matrix;
            if (drawable == null || matrix == null)
                return boundingBoxes;
            var boxes = recalculate(viewport, viewportBoxes);
            OnBoundingBoxesChanged.Invoke();
            return boxes;
        }

        private AGSBoundingBoxes recalculate(IViewport viewport, ViewportBoundingBoxes viewportBoxes)
        {
            var boundingBoxes = viewportBoxes.BoundingBoxes;
            var drawable = _drawable;
			var matrix = _matrix;
            var scale = _scale;
            var sprite = _image?.CurrentSprite;
            bool isHitTestBoxDirty = _isHitTestBoxDirty;

            if (scale == null || drawable == null || matrix == null)
            {
                return boundingBoxes;
            }

            var baseSize = sprite?.BaseSize ?? scale.BaseSize;
            if (isHitTestBoxDirty)
            {
                _isCropDirty = true;
                _isHitTestBoxDirty = false;
                updateHitTestBox(baseSize, drawable, matrix);
            }
            var crop = _crop;

            _areViewportsDirty = false;
            viewportBoxes.IsDirty = false;

			Size resolution;
			PointF resolutionFactor;
			bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _settings.VirtualResolution,
                                                 drawable, null, out resolutionFactor, out resolution);

            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : viewport.GetMatrix(drawable.RenderLayer);
            AGSBoundingBox intermediateBox, hitTestBox;
            hitTestBox = _hitTestBox;

            float width = baseSize.Width;
            float height = baseSize.Height;

			if (resolutionMatches)
            {
                intermediateBox = _intermediateBox;
            }
            else
            {
				var modelMatrices = matrix.GetModelMatrices();
                var modelMatrix = modelMatrices.InObjResolutionMatrix;
                intermediateBox = _boundingBoxBuilder.BuildIntermediateBox(width, height, ref modelMatrix);
            }

            PointF renderCropScale;
            var renderBox = _boundingBoxBuilder.BuildRenderBox(ref intermediateBox, ref viewportMatrix, out renderCropScale);

            PointF hitTestCropScale = renderCropScale;
            if (MathUtils.FloatEquals(hitTestCropScale.X, 1f) && MathUtils.FloatEquals(hitTestCropScale.Y, 1f))
            {
                hitTestCropScale = new PointF(_hitTestBox.Width / renderBox.Width, _hitTestBox.Height / renderBox.Height);
            }

            var cropInfo = renderBox.Crop(BoundingBoxType.Render, crop, renderCropScale);
			boundingBoxes.PreCropViewportBox = renderBox;
			renderBox = cropInfo.BoundingBox;
            boundingBoxes.ViewportBox = renderBox;
            if (cropInfo.TextureBox != null)
            {
                boundingBoxes.TextureBox = cropInfo.TextureBox;
            }
            else
            {
                var textureOffset = _textureOffset;
                var image = sprite?.Image;
                if (image != null && (width != image.Width || height != image.Height ||
                                      (!textureOffset?.TextureOffset.Equals(Vector2.Zero) ?? false)))
                {
                    var offset = textureOffset?.TextureOffset ?? PointF.Empty;
                    setProportionalTextureSize(boundingBoxes, image, width, height, offset);
                }
                else boundingBoxes.TextureBox = null;
            }

            if (cropInfo.Equals(_defaultCropInfo))
            {
                boundingBoxes.WorldBox = default;
            }
            else
            {
                hitTestBox = hitTestBox.Crop(BoundingBoxType.HitTest, crop, hitTestCropScale).BoundingBox;
                boundingBoxes.WorldBox = hitTestBox;
            }
            _isCropDirty = false;

            return boundingBoxes;
		}

        private static void setProportionalTextureSize(AGSBoundingBoxes boundingBoxes,
                           IImage image, float width, float height, PointF textureOffset)
        {
            float left = textureOffset.X;
            float top = textureOffset.Y;
            float right = width / image.Width + textureOffset.X;
            float bottom = height / image.Height + textureOffset.Y;
            boundingBoxes.TextureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                                                                new Vector2(left, top), new Vector2(right, top));
        }

        private void updateHitTestBox(SizeF size, IDrawableInfoComponent drawable, IModelMatrixComponent matrix)
        {
            var modelMatrices = matrix.GetModelMatrices();
            var modelMatrix = modelMatrices.InVirtualResolutionMatrix;

			Size resolution;
			PointF resolutionFactor;
			bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _settings.VirtualResolution,
												 drawable, null, out resolutionFactor, out resolution);

            float width = size.Width / resolutionFactor.X;
            float height = size.Height / resolutionFactor.Y;
            _intermediateBox = _boundingBoxBuilder.BuildIntermediateBox(width, height, ref modelMatrix);
            _hitTestBox = _boundingBoxBuilder.BuildHitTestBox(ref _intermediateBox);
		}

        private void onHitTextBoxShouldChange()
        {
            _isHitTestBoxDirty = true;
            onAllViewportsShouldChange();
        }

        private void onCropShouldChange(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ICropSelfComponent.CropArea)) return;
            _isCropDirty = true;
        }

        private void onAllViewportsShouldChange()
        {
            _areViewportsDirty = true;
        }

        private void onTextureOffsetChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITextureOffsetComponent.TextureOffset)) return;
            onAllViewportsShouldChange();
        }

        private void onImageChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IImageComponent.Image)) return;
            onHitTextBoxShouldChange();
        }

        private void onDrawableChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IDrawableInfoComponent.RenderLayer)) onHitTextBoxShouldChange();
            else if (args.PropertyName == nameof(IDrawableInfoComponent.IgnoreViewport)) onAllViewportsShouldChange();
        }

		//https://stackoverflow.com/questions/8946790/how-to-use-an-objects-identity-as-key-for-dictionaryk-v
		private class IdentityEqualityComparer<T> : IEqualityComparer<T> where T : class
        {
            public int GetHashCode(T value) => RuntimeHelpers.GetHashCode(value);

            public bool Equals(T left, T right) => left == right; // Reference identity comparison
        }

        private class ViewportBoundingBoxes
        {
            private IViewport _viewport;
            private readonly PropertyChangedEventHandler _onViewportChangedCallback;

            public ViewportBoundingBoxes(IViewport viewport)
            {
                IsDirty = true;
                _viewport = viewport;
                BoundingBoxes = new AGSBoundingBoxes();
                _onViewportChangedCallback = onViewportChanged;
                viewport.PropertyChanged += _onViewportChangedCallback;
            }

            public AGSBoundingBoxes PreLockBoundingBoxes { get; set; }
            public AGSBoundingBoxes BoundingBoxes { get; set; }
            public bool IsDirty { get; set; }

            private void onViewportChanged(object sender, PropertyChangedEventArgs args)
            {
                IsDirty = true;
            }

            public void Dispose()
            {
                _viewport.PropertyChanged -= _onViewportChangedCallback;
            }
        }
	}
}
