using AGS.API;

namespace AGS.Engine
{
    public class AGSBoundingBoxComponent : AGSComponent, IBoundingBoxComponent
    {
        private bool _isDirty;
        private AGSBoundingBoxes _boundingBoxes;
        private IModelMatrixComponent _matrix;
        private ICropSelfComponent _crop;
        private IAnimationContainer _animation;
        private IDrawableInfo _drawable;
        private IRoom _room;
        private IGameSettings _settings;
        private readonly GLMatrices _matrices = new GLMatrices();
        private readonly IGLViewportMatrixFactory _layerViewports;
        private readonly IBoundingBoxBuilder _boundingBoxBuilder;
        private readonly IGameState _state;

        public AGSBoundingBoxComponent(IGameSettings settings, IGLViewportMatrixFactory layerViewports,
                                       IBoundingBoxBuilder boundingBoxBuilder, IGameState state, IGameEvents events)
        {
            _settings = settings;
            _state = state;
            OnBoundingBoxesChanged = new AGSEvent<object>();
            _layerViewports = layerViewports;
            _boundingBoxBuilder = boundingBoxBuilder;
            events.OnRoomChanging.Subscribe(onRoomChanged);
            onRoomChanged(null);
        }

        public IEvent<object> OnBoundingBoxesChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IModelMatrixComponent>(c => { c.OnMatrixChanged.Subscribe(onSomethingChanged); _matrix = c; },
                                               c => { c.OnMatrixChanged.Unsubscribe(onSomethingChanged); _matrix = null; });
            entity.Bind<ICropSelfComponent>(c => { c.OnCropAreaChanged.Subscribe(onSomethingChanged); _crop = c; },
                                            c => { c.OnCropAreaChanged.Unsubscribe(onSomethingChanged); _crop = null; });
            entity.Bind<IImageComponent>(c => c.OnImageChanged.Subscribe(onSomethingChanged),
                                         c => c.OnImageChanged.Unsubscribe(onSomethingChanged));
            entity.Bind<IAnimationContainer>(c => _animation = c, _animation => _animation = null);
            entity.Bind<IDrawableInfo>(c => { c.OnRenderLayerChanged.Subscribe(onSomethingChanged); c.OnIgnoreViewportChanged.Subscribe(onSomethingChanged); _drawable = c; },
                                       c => { c.OnRenderLayerChanged.Unsubscribe(onSomethingChanged); c.OnIgnoreViewportChanged.Unsubscribe(onSomethingChanged); _drawable = null; });
        }

		public AGSBoundingBoxes GetBoundingBoxes()
		{
            if (!_isDirty) return _boundingBoxes;
            var animation = _animation;
            var room = _room;
            var drawable = _drawable;
            var matrix = _matrix;
            var crop = _crop;
            if (animation == null || animation.Animation == null || animation.Animation.Sprite == null ||
                animation.Animation.Sprite.Image == null || drawable == null || matrix == null) return _boundingBoxes;

            var layerViewport = _layerViewports.GetViewport(drawable.RenderLayer.Z);
			Size resolution;
			PointF resolutionFactor;
			bool resolutionMatches = AGSModelMatrixComponent.GetVirtualResolution(false, _settings.VirtualResolution,
                                                 drawable, null, out resolutionFactor, out resolution);

            var viewport = room == null ? null : room.Viewport;
            if (viewport == null) return null;
            var viewportMatrix = drawable.IgnoreViewport ? Matrix4.Identity : layerViewport.GetMatrix(viewport, drawable.RenderLayer.ParallaxSpeed);

            var modelMatrices = matrix.GetModelMatrices();
			_matrices.ModelMatrix = modelMatrices.InVirtualResolutionMatrix;
			_matrices.ViewportMatrix = viewportMatrix;

            var sprite = animation.Animation.Sprite;
			float width =  sprite.Image.Width / resolutionFactor.X;
			float height = sprite.Image.Height / resolutionFactor.Y;

            var boundingBoxes = _boundingBoxes;
            if (boundingBoxes == null) boundingBoxes = new AGSBoundingBoxes();
            var scale = _boundingBoxBuilder.Build(boundingBoxes, width, height, _matrices, resolutionMatches, true);
			AGSBoundingBox hitTestBox = boundingBoxes.HitTestBox;

			if (!resolutionMatches)
			{
				_matrices.ModelMatrix = modelMatrices.InObjResolutionMatrix;
				_boundingBoxBuilder.Build(boundingBoxes, sprite.Image.Width,
					sprite.Image.Height, _matrices, true, false);
			}
			AGSBoundingBox renderBox = boundingBoxes.RenderBox;
			var cropInfo = renderBox.Crop(BoundingBoxType.Render, crop, resolutionFactor, scale);
            if (cropInfo.Equals(default(AGSCropInfo)))
            {
                boundingBoxes = null;
            }
            else
            {
                renderBox = cropInfo.BoundingBox;
                hitTestBox = hitTestBox.Crop(BoundingBoxType.HitTest, crop, AGSModelMatrixComponent.NoScaling, scale).BoundingBox;
                boundingBoxes.RenderBox = renderBox;
                boundingBoxes.HitTestBox = hitTestBox;
                boundingBoxes.TextureBox = cropInfo.TextureBox;
            }
            _isDirty = false;
            _boundingBoxes = boundingBoxes;
            OnBoundingBoxesChanged.Invoke(null);
            return _boundingBoxes;
		}

        private void onRoomChanged(object state)
        {
            var room = _room;
            IViewport viewport = null;
            if (room != null) viewport = room.Viewport;
            if (viewport != null)
            {
                viewport.OnAngleChanged.Unsubscribe(onSomethingChanged);
                viewport.OnScaleChanged.Unsubscribe(onSomethingChanged);
                viewport.OnPositionChanged.Unsubscribe(onSomethingChanged);
            }
            _room = _state.Room;
            room = _room;
            viewport = null;
            if (room != null) viewport = room.Viewport;
            if (viewport != null)
            {
				viewport.OnAngleChanged.Subscribe(onSomethingChanged);
				viewport.OnScaleChanged.Subscribe(onSomethingChanged);
				viewport.OnPositionChanged.Subscribe(onSomethingChanged);
			}
            onSomethingChanged(state);    
        }

        private void onSomethingChanged(object state)
        {
            _isDirty = true;
        }
	}
}
