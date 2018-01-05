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
        private ConcurrentDictionary<IViewport, ViewportBoundingBoxes> _boundingBoxes;
        private AGSBoundingBox _hitTestBox, _intermediateBox;
        private IModelMatrixComponent _matrix;
        private ICropSelfComponent _crop;
        private IAnimationComponent _animation;
        private IDrawableInfoComponent _drawable;
        private ITextureOffsetComponent _textureOffset;
        private IGameSettings _settings;
        private readonly IGLViewportMatrixFactory _layerViewports;
        private readonly IBoundingBoxBuilder _boundingBoxBuilder;
        private readonly IGameState _state;
        private IEntity _entity;
        private readonly AGSCropInfo _defaultCropInfo = default;

        public AGSBoundingBoxComponent(IRuntimeSettings settings, IGLViewportMatrixFactory layerViewports,
                                       IBoundingBoxBuilder boundingBoxBuilder, IGameState state, IGameEvents events)
        {
            _boundingBoxes = new ConcurrentDictionary<IViewport, ViewportBoundingBoxes>(new IdentityEqualityComparer<IViewport>());
            _boundingBoxes.TryAdd(state.Viewport, new ViewportBoundingBoxes(state.Viewport));
            _settings = settings;
            _state = state;
            OnBoundingBoxesChanged = new AGSEvent();
            _layerViewports = layerViewports;
            _boundingBoxBuilder = boundingBoxBuilder;
            boundingBoxBuilder.OnNewBoxBuildRequired.Subscribe(onHitTextBoxShouldChange);
            events.OnRoomChanging.Subscribe(onHitTextBoxShouldChange);
            onHitTextBoxShouldChange();
        }

        public IBlockingEvent OnBoundingBoxesChanged { get; }

        [Property(Browsable = false)]
        public ILockStep BoundingBoxLockStep => this;

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);

            entity.Bind<IModelMatrixComponent>(c => { c.OnMatrixChanged.Subscribe(onHitTextBoxShouldChange); _matrix = c; },
                                               c => { c.OnMatrixChanged.Unsubscribe(onHitTextBoxShouldChange); _matrix = null; });
            entity.Bind<ICropSelfComponent>(c => { c.PropertyChanged += onCropShouldChange; _crop = c; },
                                            c => { c.PropertyChanged -= onCropShouldChange; _crop = null; });
            entity.Bind<IImageComponent>(c => c.PropertyChanged += onImageChanged,
                                         c => c.PropertyChanged -= onImageChanged);
            entity.Bind<IAnimationComponent>(c => _animation = c, _animation => _animation = null);
            entity.Bind<IDrawableInfoComponent>(c => { c.PropertyChanged += onDrawableChanged; _drawable = c; },
                                       c => { c.PropertyChanged -= onDrawableChanged; _drawable = null; });
            entity.Bind<ITextureOffsetComponent>(c => { c.PropertyChanged += onTextureOffsetChanged; _textureOffset = c; onAllViewportsShouldChange(); }, 
                                                 c => { c.PropertyChanged -= onTextureOffsetChanged; _textureOffset = null; onAllViewportsShouldChange(); });
            
        }

        public void Lock()
        {
            foreach (var boxes in _boundingBoxes.Values)
            {
                boxes.PreLockBoundingBoxes = new AGSBoundingBoxes
                {
                    HitTestBox = boxes.BoundingBoxes.HitTestBox,
                    PreCropRenderBox = boxes.BoundingBoxes.PreCropRenderBox,
                    RenderBox = boxes.BoundingBoxes.RenderBox,
                    TextureBox = boxes.BoundingBoxes.TextureBox
                };
            }
            Interlocked.Increment(ref _pendingLocks);
        }

        public void PrepareForUnlock()
        {
            bool isDirty = anyChangesForLock();
            _shouldFireOnUnlock += (isDirty ? 1 : 0);
            if (!isDirty) return;
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
                    (!(_drawable?.IgnoreViewport ?? false) && _boundingBoxes.Values.Any(v => v.IsDirty)));
        }

        public AGSBoundingBoxes GetBoundingBoxes(IViewport viewport)
        {
            var viewportBoxes = _boundingBoxes.GetOrAdd(viewport, _ => new ViewportBoundingBoxes(viewport));
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
            var image = _animation?.Animation?.Sprite?.Image;
            var drawable = _drawable;
            var matrix = _matrix;
            if (image == null || drawable == null || matrix == null) 
                return boundingBoxes;
            var boxes = recalculate(viewport, viewportBoxes);
            OnBoundingBoxesChanged.Invoke();
            return boxes;
        }

        private AGSBoundingBoxes recalculate(IViewport viewport, ViewportBoundingBoxes viewportBoxes)
        {
            var boundingBoxes = viewportBoxes.BoundingBoxes;
            var animation = _animation;
			var drawable = _drawable;
			var matrix = _matrix;
            bool isHitTestBoxDirty = _isHitTestBoxDirty;

            if (animation?.Animation?.Sprite?.Image == null || drawable == null || matrix == null)
            {
                _isHitTestBoxDirty = false;
                _isCropDirty = false;
                _areViewportsDirty = false;
                viewportBoxes.IsDirty = false;
                return boundingBoxes;
            }

            if (isHitTestBoxDirty)
            {
                _isCropDirty = true;
                _isHitTestBoxDirty = false;
                updateHitTestBox(animation, drawable, matrix);
            }
            var crop = _crop;

            _isCropDirty = false;
            _areViewportsDirty = false;
            viewportBoxes.IsDirty = false;

            var layerViewport = _layerViewports.GetViewport(drawable.RenderLayer.Z);
			Size resolution;
			PointF resolutionFactor;
			bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _settings.VirtualResolution,
                                                 drawable, null, out resolutionFactor, out resolution);

            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : layerViewport.GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);
            AGSBoundingBox intermediateBox, hitTestBox;
            hitTestBox = _hitTestBox;

			var sprite = animation.Animation.Sprite;
			float width = sprite.BaseSize.Width;
			float height = sprite.BaseSize.Height;

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
			boundingBoxes.PreCropRenderBox = renderBox;
			renderBox = cropInfo.BoundingBox;
            boundingBoxes.RenderBox = renderBox;
            if (cropInfo.TextureBox != null)
            {
                boundingBoxes.TextureBox = cropInfo.TextureBox;
            }
            else
            {
                var textureOffset = _textureOffset;
                if (width != sprite.Image.Width || height != sprite.Image.Height ||
                    (!textureOffset?.TextureOffset.Equals(Vector2.Zero) ?? false))
                {
                    var offset = textureOffset?.TextureOffset ?? PointF.Empty;
                    setProportionalTextureSize(boundingBoxes, sprite, width, height, offset);
                }
                else boundingBoxes.TextureBox = null;
            }

            if (cropInfo.Equals(_defaultCropInfo))
            {
                boundingBoxes.HitTestBox = default;
            }
            else
            {
                hitTestBox = hitTestBox.Crop(BoundingBoxType.HitTest, crop, hitTestCropScale).BoundingBox;
                boundingBoxes.HitTestBox = hitTestBox;
            }

            return boundingBoxes;
		}

        private static void setProportionalTextureSize(AGSBoundingBoxes boundingBoxes,
                           ISprite sprite, float width, float height, PointF textureOffset)
        {
            float left = textureOffset.X;
            float top = textureOffset.Y;
            float right = width / sprite.Image.Width + textureOffset.X;
            float bottom = height / sprite.Image.Height + textureOffset.Y;
            boundingBoxes.TextureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                                                                new Vector2(left, top), new Vector2(right, top));
        }

        private void updateHitTestBox(IAnimationComponent animation, IDrawableInfoComponent drawable, IModelMatrixComponent matrix)
        {
            var modelMatrices = matrix.GetModelMatrices();
            var modelMatrix = modelMatrices.InVirtualResolutionMatrix;

			Size resolution;
			PointF resolutionFactor;
			bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _settings.VirtualResolution,
												 drawable, null, out resolutionFactor, out resolution);
            
			var sprite = animation.Animation.Sprite;
            float width = sprite.BaseSize.Width / resolutionFactor.X;
			float height = sprite.BaseSize.Height / resolutionFactor.Y;
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

            public ViewportBoundingBoxes(IViewport viewport)
            {
                IsDirty = true;
                _viewport = viewport;
                BoundingBoxes = new AGSBoundingBoxes();
                viewport.PropertyChanged += onViewportChanged;
            }

            public AGSBoundingBoxes PreLockBoundingBoxes { get; set; }
            public AGSBoundingBoxes BoundingBoxes { get; set; }
            public bool IsDirty { get; set; }

            private void onViewportChanged(object sender, PropertyChangedEventArgs args)
            {
                IsDirty = true;
            }
        }
	}
}
