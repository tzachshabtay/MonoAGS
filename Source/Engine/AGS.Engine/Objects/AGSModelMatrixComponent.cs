using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSModelMatrixComponent : AGSComponent, IModelMatrixComponent
    {
        private bool _isDirty;
        private ModelMatrices _matrices;
        private readonly AGSEventArgs _args = new AGSEventArgs();

        private IAnimationContainer _animation;
        private IInObjectTree _tree;
        private IScaleComponent _scale;
        private ITranslateComponent _translate;
        private IRotateComponent _rotate;
        private IImageComponent _image;
        private IHasRoom _room;
        private IDrawableInfo _drawable;
        private IEntity _entity;
        private IObject _parent;
        private ISprite _sprite;

        private readonly Size _virtualResolution;
        private PointF _areaScaling;
        private SizeF? _customImageSize;
        private PointF? _customResolutionFactor;
        private readonly float? _nullFloat = null;

        public AGSModelMatrixComponent(IRuntimeSettings settings)
        {
            _isDirty = true;
            _matrices = new ModelMatrices();
            _virtualResolution = settings.VirtualResolution;
            OnMatrixChanged = new AGSEvent<AGSEventArgs>();
        }

        public static readonly PointF NoScaling = new PointF(1f, 1f);

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            _animation = entity.GetComponent<IAnimationContainer>();
            _tree = entity.GetComponent<IInObjectTree>();
            _scale = entity.GetComponent<IScaleComponent>();
            _translate = entity.GetComponent<ITranslateComponent>();
            _rotate = entity.GetComponent<IRotateComponent>();
            _image = entity.GetComponent<IImageComponent>();
            _room = entity.GetComponent<IHasRoom>();
            _drawable = entity.GetComponent<IDrawableInfo>();
        }

        public override void AfterInit()
        {
            base.AfterInit();
            _parent = _tree.TreeNode.Parent;
            _sprite = getSprite();

            subscribeSprite(_sprite);
            _tree.TreeNode.OnParentChanged.Subscribe(onParentChanged);
            if (_parent != null) _parent.OnMatrixChanged.Subscribe(onSomethingChanged);
            _scale.OnScaleChanged.Subscribe(onSomethingChanged);
            _translate.OnLocationChanged.Subscribe(onSomethingChanged);
            _rotate.OnAngleChanged.Subscribe(onSomethingChanged);
            _image.OnAnchorChanged.Subscribe(onSomethingChanged);
            _drawable.OnIgnoreScalingAreaChanged.Subscribe(onSomethingChanged);
            _drawable.OnRenderLayerChanged.Subscribe(onSomethingChanged);
        }

        public ModelMatrices GetModelMatrices() 
        { 
            return shouldRecalculate() ? recalculateMatrices() : _matrices; 
        }

        public IEvent<AGSEventArgs> OnMatrixChanged { get; private set; }

        private void onSomethingChanged(object sender, AGSEventArgs args)
        {
            _isDirty = true;
        }

        private void onParentChanged(object sender, AGSEventArgs args)
        {
            if (_parent != null) _parent.OnMatrixChanged.Unsubscribe(onSomethingChanged);
            _parent = _tree.TreeNode.Parent;
            if (_parent != null) _parent.OnMatrixChanged.Subscribe(onSomethingChanged);
            onSomethingChanged(sender, args);
        }

        private ISprite getSprite()
        {
            return _animation.Animation == null ? null : _animation.Animation.Sprite;
        }

        private void subscribeSprite(ISprite sprite)
        {
            changeSpriteSubscription(sprite, (ev) => ev.Subscribe(onSomethingChanged));
        }

        private void unsubscribeSprite(ISprite sprite)
        {
            changeSpriteSubscription(sprite, (ev) => ev.Unsubscribe(onSomethingChanged));
        }

        private void changeSpriteSubscription(ISprite sprite, Action<IEvent<AGSEventArgs>> change)
        {
            if (sprite == null) return;
            change(sprite.OnLocationChanged);
            change(sprite.OnAngleChanged);
            change(sprite.OnScaleChanged);
            change(sprite.OnAnchorChanged);
        }

        private bool shouldRecalculate() 
        {
            PointF areaScaling = getAreaScaling();
            if (!_areaScaling.Equals(areaScaling)) 
            {
                _areaScaling = areaScaling;
                _isDirty = true;
            }
            var currentSprite = getSprite();
            if (currentSprite != _sprite)
            {
                unsubscribeSprite(_sprite);
                _sprite = currentSprite;
                subscribeSprite(_sprite);
                _isDirty = true;
            }
            var renderer = _image.CustomRenderer;
            if (renderer != null)
            {
                var customImageSize = renderer.CustomImageSize;
                if ((customImageSize == null && _customImageSize != null) || 
                    (customImageSize != null && _customImageSize == null) ||
                    !customImageSize.Value.Equals(_customImageSize.Value))
                {
                    _customImageSize = customImageSize;
                    _isDirty = true;
                }
                var customFactor = renderer.CustomImageResolutionFactor;
                if ((customFactor == null && _customResolutionFactor != null) ||
                    (customFactor != null && _customResolutionFactor == null) ||
                    !customFactor.Value.Equals(_customResolutionFactor.Value))
                {
                    _customResolutionFactor = customFactor;
                    _isDirty = true;
                }
            }
            return _isDirty;
        }

        private ModelMatrices recalculateMatrices()
        {
            recalculate();
            OnMatrixChanged.FireEvent(this, _args);
            return _matrices;
        }

        private void recalculate()
        {
            var objResolution = _drawable.RenderLayer == null ? _virtualResolution : _drawable.RenderLayer.IndependentResolution ?? _virtualResolution;
            var resolutionMatches = _customResolutionFactor == null ? 
                objResolution.Equals(_virtualResolution) : 
                _customResolutionFactor.Value.Equals(NoScaling);
            var resolutionFactor = _customResolutionFactor ?? new PointF(objResolution.Width / _virtualResolution.Width, objResolution.Height / _virtualResolution.Height);

            if (_entity.ID == "Quit Button")
            {
            }

            var renderMatrix = getMatrix(resolutionFactor);
            var hitTestMatrix = resolutionMatches ? renderMatrix : getMatrix(NoScaling);
            _matrices.InObjResolutionMatrix = renderMatrix;
            _matrices.InVirtualResolutionMatrix = hitTestMatrix;
            _isDirty = false;
        }

        private Matrix4 getMatrix(PointF resolutionFactor) 
        {
            var sprite = _animation.Animation.Sprite;
            Matrix4 spriteMatrix = getModelMatrix(sprite, sprite, sprite, sprite, 
                                                  NoScaling, NoScaling, true);
            Matrix4 objMatrix = getModelMatrix(_scale, _rotate, _translate, 
                                               _image, _areaScaling, resolutionFactor, true);

            var modelMatrix = spriteMatrix * objMatrix;
            var parent = _tree.TreeNode.Parent;
            while (parent != null)
            {
                //var parentMatrices = parent.GetModelMatrices();
                //Matrix4 parentMatrix = resolutionFactor.Equals(GLMatrixBuilder.NoScaling) ? parentMatrices.InVirtualResolutionMatrix : parentMatrices.InObjResolutionMatrix;
                Matrix4 parentMatrix = getModelMatrix(parent, parent, parent, parent, NoScaling, resolutionFactor, false);
                modelMatrix = modelMatrix * parentMatrix;
                parent = parent.TreeNode.Parent;
            }
            return modelMatrix;
        }

        private Matrix4 getModelMatrix(IScale scale, IRotate rotate, ITranslate translate, IHasImage image, 
                                       PointF areaScaling, PointF resolutionTransform, bool useCustomImageSize)
        {
            if (scale == null) return Matrix4.Identity;
            float? customWidth = _customImageSize == null || !useCustomImageSize ? 
                _nullFloat : _customImageSize.Value.Width;
            float? customHeight = _customImageSize == null || !useCustomImageSize ? 
                _nullFloat : _customImageSize.Value.Height;
            float width = (customWidth ?? scale.Width) * resolutionTransform.X;
            float height = (customHeight ?? scale.Height) * resolutionTransform.Y;
            PointF anchorOffsets = getAnchorOffsets(image.Anchor, width, height);
            Matrix4 anchor = Matrix4.CreateTranslation(new Vector3(-anchorOffsets.X, -anchorOffsets.Y, 0f));
            Matrix4 scaleMat = Matrix4.CreateScale(new Vector3(scale.ScaleX * areaScaling.X,
                scale.ScaleY * areaScaling.Y, 1f));
            Matrix4 rotation = Matrix4.CreateRotationZ(rotate.Angle);
            float x = translate.X * resolutionTransform.X;
            float y = translate.Y * resolutionTransform.Y;
            Matrix4 transform = Matrix4.CreateTranslation(new Vector3(x, y, 0f));
            return anchor * scaleMat * rotation * transform;
        }

        private PointF getAnchorOffsets(PointF anchor, float width, float height)
        {
            float x = MathUtils.Lerp(0f, 0f, 1f, width, anchor.X);
            float y = MathUtils.Lerp(0f, 0f, 1f, height, anchor.Y);
            return new PointF(x, y);
        }

        private PointF getAreaScaling()
        {
            if (_drawable.IgnoreScalingArea) return NoScaling;
            foreach (IArea area in _room.Room.GetMatchingAreas(_translate.Location.XY, _entity.ID))
            {
                IScalingArea scaleArea = area.GetComponent<IScalingArea>();
                if (scaleArea == null || (!scaleArea.ScaleObjectsX && !scaleArea.ScaleObjectsY)) continue;
                float scale = scaleArea.GetScaling(scaleArea.Axis == ScalingAxis.X ? _translate.X : _translate.Y);
                return new PointF(scaleArea.ScaleObjectsX ? scale : 1f, scaleArea.ScaleObjectsY ? scale : 1f);
            }
            return NoScaling;
        }
    }
}
