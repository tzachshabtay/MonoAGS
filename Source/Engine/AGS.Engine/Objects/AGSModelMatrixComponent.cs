using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSModelMatrixComponent : AGSComponent, IModelMatrixComponent, ILockStep
    {
        private bool _isDirty;
        private int _shouldFireOnUnlock, _pendingLocks;
        private ModelMatrices _matrices, _preLockMatrices;

        private IInObjectTreeComponent _tree;
        private IScaleComponent _scale;
        private ITranslateComponent _translate;
        private IRotateComponent _rotate;
        private IWorldPositionComponent _worldPosition;
        private IImageComponent _image;
        private IHasRoomComponent _room;
        private IDrawableInfoComponent _drawable;
        private ITextComponent _text;
        private IObject _parent;
        private ISprite _sprite;
        private IJumpOffsetComponent _jump;
        private IRoom _lastRoom;

        private readonly Size _virtualResolution;
        private PointF _areaScaling;
        private SizeF? _customImageSize;
        private PointF? _customResolutionFactor;
        private readonly float? _nullFloat = null;
        private Dictionary<string, List<IComponentBinding>> _areaBindings;

        public AGSModelMatrixComponent(IRuntimeSettings settings)
        {
            _isDirty = true;
            _matrices = new ModelMatrices();
            _areaBindings = new Dictionary<string, List<IComponentBinding>>();
            _virtualResolution = settings.VirtualResolution;
            OnMatrixChanged = new AGSEvent();
        }

        public static readonly PointF NoScaling = new PointF(1f, 1f);

        [Property(Browsable = false)]
        public ILockStep ModelMatrixLockStep { get { return this; } }

        public override void AfterInit()
        {
            Entity.Bind<IHasRoomComponent>(
                c => { _room = c; refreshAreaScaling(); subscribeRoom(); onSomethingChanged(); },
                c => { unsubscribeRoom(c); _room = null; refreshAreaScaling(); onSomethingChanged(); });
            
            Entity.Bind<IScaleComponent>(
                c => { _scale = c; c.PropertyChanged += onScaleChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onScaleChanged; _scale = null; onSomethingChanged(); });
            Entity.Bind<ITranslateComponent>(
                c => { _translate = c; c.PropertyChanged += onTranslateChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onTranslateChanged; _translate = null; onSomethingChanged(); }
            );
            Entity.Bind<IWorldPositionComponent>(
                c => { _worldPosition = c; c.PropertyChanged += onWorldPositionChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onWorldPositionChanged; _translate = null; onSomethingChanged(); }
            );
            Entity.Bind<IJumpOffsetComponent>(
                c => { _jump = c; c.PropertyChanged += onJumpOffsetChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onJumpOffsetChanged; _jump = null; onSomethingChanged(); }
            );
            Entity.Bind<IRotateComponent>(
                c => { _rotate = c; c.PropertyChanged += onRotateChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onRotateChanged; _rotate = null; onSomethingChanged(); }
            );
            Entity.Bind<IImageComponent>(
                c => { _image = c; c.PropertyChanged += onImageChanged; onSomethingChanged(); },
                c => { c.PropertyChanged -= onImageChanged; _image = null; onSomethingChanged(); }
			);
            Entity.Bind<ITextComponent>(
                c => { _text = c; subscribeTextComponent(); },
                _ => { unsubscribeTextComponent(); _text = null; }
            );

            Entity.Bind<IDrawableInfoComponent>(
                c => 
            {
                _drawable = c;
                c.PropertyChanged += onDrawableChanged;
                onSomethingChanged();
            },c =>
            {
                c.PropertyChanged -= onDrawableChanged;
                _drawable = null;
				onSomethingChanged();
            });

            Entity.Bind<IInObjectTreeComponent>(
				c =>
			{
				_tree = c;
				_parent = _tree.TreeNode.Parent;
				_tree.TreeNode.OnParentChanged.Subscribe(onParentChanged);
				_parent?.OnMatrixChanged.Subscribe(onSomethingChanged);
				onSomethingChanged();
			}, c =>
			{
				c.TreeNode.OnParentChanged.Unsubscribe(onParentChanged);
				c.TreeNode.Parent?.OnMatrixChanged.Unsubscribe(onSomethingChanged);
				_tree = null;
				_parent = null;
				onSomethingChanged();
			});
        }

        public override void Dispose()
        {
            base.Dispose();
            unsubscribeSprite(_sprite);
            unsubscribeRoomAreas();
        }

        public ref ModelMatrices GetModelMatrices() 
        {
            if (_pendingLocks > 0) return ref _preLockMatrices;
            if (!_isDirty)
            {
                return ref _matrices;
            }
            return ref recalculateMatrices();
        }

        public void Lock()
        {
            _preLockMatrices = _matrices;
            Interlocked.Increment(ref _pendingLocks);
        }

        public void PrepareForUnlock()
        {
            _shouldFireOnUnlock += (_isDirty ? 1 : 0);
            if (!_isDirty) return;
            recalculate();
        }

        public void Unlock()
        {
            if (Interlocked.Decrement(ref _pendingLocks) > 0) return;
            if (Interlocked.Exchange(ref _shouldFireOnUnlock, 0) >= 1)
            {
                OnMatrixChanged.Invoke();
            }
        }

        public IBlockingEvent OnMatrixChanged { get; private set; }

        public static PointF GetPivotOffsets(PointF pivot, float width, float height)
        {
            float x = MathUtils.Lerp(0f, 0f, 1f, width, pivot.X);
            float y = MathUtils.Lerp(0f, 0f, 1f, height, pivot.Y);
            return new PointF(x, y);
        }

        public static bool GetVirtualResolution(bool flattenLayerResolution, Size virtualResolution, IDrawableInfoComponent drawable, 
                                         PointF? customResolutionFactor, out PointF resolutionFactor, out Size resolution)
        {
            //Priorities for virtual resolution: layer's resolution comes first, if not then the custom resolution (which is the text scaling resolution for text, otherwise null),
            //and if not use the virtual resolution.
            var layerResolution = drawable?.RenderLayer?.IndependentResolution;
            if (layerResolution != null)
            {
                if (flattenLayerResolution)
                {
                    resolutionFactor = NoScaling;
                    resolution = virtualResolution;
                    return false;
                }
                resolution = layerResolution.Value;
                resolutionFactor = new PointF(resolution.Width / (float)virtualResolution.Width, resolution.Height / (float)virtualResolution.Height);
                return layerResolution.Value.Equals(virtualResolution);
            }
            else if (customResolutionFactor != null)
            {
                resolutionFactor = customResolutionFactor.Value;
                resolution = new Size((int)(virtualResolution.Width * customResolutionFactor.Value.X), 
                                      (int)(virtualResolution.Height * customResolutionFactor.Value.Y));
                return customResolutionFactor.Value.Equals(NoScaling);
            }
            else
            {
                resolutionFactor = NoScaling;
                resolution = virtualResolution;
                return true;
            }
        }

        private void onSpritePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ISprite.X) && args.PropertyName != nameof(ISprite.Y) &&
                args.PropertyName != nameof(ISprite.ScaleX) && args.PropertyName != nameof(ISprite.ScaleY) &&
                args.PropertyName != nameof(ISprite.Width) && args.PropertyName != nameof(ISprite.Height) &&
                args.PropertyName != nameof(ISprite.Angle) && args.PropertyName != nameof(ISprite.Pivot)) return;
            onSomethingChanged();
        }

        private void onTranslateChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.X) && args.PropertyName != nameof(ITranslateComponent.Y)) return;
            onSomethingChanged();
        }

        private void onWorldPositionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IWorldPositionComponent.WorldXY)) return;
            refreshAreaScaling();
            onSomethingChanged();
        }

        private void onScaleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScale.ScaleX) && args.PropertyName != nameof(IScale.ScaleY) &&
                args.PropertyName != nameof(IScale.Width) && args.PropertyName != nameof(IScale.Height)) return;
            onSomethingChanged();
        }

        private void onRotateChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IRotateComponent.Angle)) return;
            onSomethingChanged();
        }

        private void onJumpOffsetChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IJumpOffsetComponent.JumpOffset)) return;
            onSomethingChanged();
        }

        private void onImageChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IImageComponent.Pivot) 
                && args.PropertyName != nameof(IImageComponent.CurrentSprite)) return;
            if (args.PropertyName == nameof(IImageComponent.CurrentSprite))
            {
                unsubscribeSprite(_sprite);
                _sprite = _image?.CurrentSprite;
                subscribeSprite(_sprite);
            }
            onSomethingChanged();
        }

        private void subscribeTextComponent()
        {
            _customImageSize = _text.CustomImageSize;
            _customResolutionFactor = _text.CustomImageResolutionFactor;
            _text.PropertyChanged += onTextPropertyChanged;
            onSomethingChanged();
        }

        private void unsubscribeTextComponent()
        {
            _customImageSize = null;
            _customResolutionFactor = null;
            _text.PropertyChanged -= onTextPropertyChanged;
            onSomethingChanged();
        }

        private void onTextPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var text = _text;
            if (text == null) return;
            if (args.PropertyName == nameof(ITextComponent.CustomImageSize))
            {
                _customImageSize = text.CustomImageSize;
                onSomethingChanged();
                return;
            }
            if (args.PropertyName == nameof(ITextComponent.CustomImageResolutionFactor))
            {
                _customResolutionFactor = text.CustomImageResolutionFactor;
                onSomethingChanged();
                return;
            }
        }

        private void onDrawableChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IDrawableInfoComponent.RenderLayer) &&
                args.PropertyName != nameof(IDrawableInfoComponent.IgnoreScalingArea)) return;
            if (args.PropertyName == nameof(IDrawableInfoComponent.IgnoreScalingArea))
            {
                refreshAreaScaling();
            }
            onSomethingChanged();
        }

        private void onSomethingChanged()
        {
            _isDirty = true;
        }

        private void onParentChanged()
        {
            _parent?.OnMatrixChanged.Unsubscribe(onSomethingChanged);
            _parent = _tree == null ? null : _tree.TreeNode.Parent;
            _parent?.OnMatrixChanged.Subscribe(onSomethingChanged);
            onSomethingChanged();
        }

        private void subscribeSprite(ISprite sprite)
        {
            if (sprite == null) return;
            sprite.PropertyChanged += onSpritePropertyChanged;
        }

        private void unsubscribeSprite(ISprite sprite)
        {
            if (sprite == null) return;
            sprite.PropertyChanged -= onSpritePropertyChanged;
        }

        private ref ModelMatrices recalculateMatrices()
        {
            recalculate();
            OnMatrixChanged.Invoke();
            return ref _matrices;
        }

        private void recalculate()
        {
            PointF resolutionFactor;
            _isDirty = false;
            bool resolutionMatches = GetVirtualResolution(true, _virtualResolution, _drawable, _customResolutionFactor,
                                                   out resolutionFactor, out Size _);

            var renderMatrix = getMatrix(resolutionFactor);
            var hitTestMatrix = resolutionMatches ? renderMatrix : resolutionFactor.Equals(NoScaling) ? getMatrix(new PointF((float)_virtualResolution.Width/_drawable.RenderLayer.IndependentResolution.Value.Width,
                                                                                                                             (float)_virtualResolution.Height/_drawable.RenderLayer.IndependentResolution.Value.Height)) : getMatrix(NoScaling);
            _matrices.InObjResolutionMatrix = renderMatrix;
            _matrices.InVirtualResolutionMatrix = hitTestMatrix;
        }

        private Matrix4 getMatrix(PointF resolutionFactor) 
        {
            var sprite = _image?.CurrentSprite;
            Matrix4 spriteMatrix = sprite == null ? Matrix4.Identity : getModelMatrix(sprite, sprite, sprite, sprite, null,
                                                  NoScaling, resolutionFactor, true);
            Matrix4 objMatrix = getModelMatrix(_scale, _rotate, _translate, _image,
                                               _jump, _areaScaling, resolutionFactor, true);

            var modelMatrix = spriteMatrix * objMatrix;
            var parent = _tree == null ? null : _tree.TreeNode.Parent;
            while (parent != null)
            {
                //var parentMatrices = parent.GetModelMatrices(); todo: figure out how to reuse parent matrix instead of recalculating it here (need to change the while to if)
                //Matrix4 parentMatrix = resolutionFactor.Equals(NoScaling) ? parentMatrices.InVirtualResolutionMatrix : parentMatrices.InObjResolutionMatrix;
                Matrix4 parentMatrix = getModelMatrix(parent, parent, parent, parent, parent.GetComponent<IJumpOffsetComponent>(),
                    NoScaling, resolutionFactor, false);
                modelMatrix = modelMatrix * parentMatrix;
                parent = parent.TreeNode.Parent;
            }
            return modelMatrix;
        }

        private Matrix4 getModelMatrix(IScale scale, IRotate rotate, ITranslate translate, IHasImage image,
                                       IJumpOffsetComponent jump, PointF areaScaling, PointF resolutionTransform, bool useCustomImageSize)
        {
            if (scale == null) return Matrix4.Identity;
            float? customWidth = _customImageSize == null || !useCustomImageSize ? 
                _nullFloat : (_customImageSize.Value.Width * scale.ScaleX);
            float? customHeight = _customImageSize == null || !useCustomImageSize ? 
                _nullFloat : (_customImageSize.Value.Height * scale.ScaleY);
            float width = (customWidth ?? scale.Width) * areaScaling.X * resolutionTransform.X;
            float height = (customHeight ?? scale.Height) * areaScaling.Y * resolutionTransform.Y;
            PointF pivotOffsets = GetPivotOffsets(image == null ? PointF.Empty : image.Pivot, 
                                                    width, height);
            Matrix4 pivotMat = Matrix4.CreateTranslation(new Vector3(-pivotOffsets.X, -pivotOffsets.Y, 0f));
            Matrix4 scaleMat = Matrix4.CreateScale(new Vector3(scale.ScaleX * areaScaling.X,
                scale.ScaleY * areaScaling.Y, 1f));
            float radians = rotate == null ? 0f : MathUtils.DegreesToRadians(rotate.Angle);
            Matrix4 rotationMat = Matrix4.CreateRotationZ(radians);
            float x = translate == null ? 0f : translate.X * resolutionTransform.X;
            float y = translate == null ? 0f : translate.Y * resolutionTransform.Y;
            if (jump != null)
            {
                x += jump.JumpOffset.X * resolutionTransform.X;
                y += jump.JumpOffset.Y * resolutionTransform.Y;
            }
            Matrix4 translateMat = Matrix4.CreateTranslation(new Vector3(x, y, 0f));
            return scaleMat * pivotMat * rotationMat * translateMat;
        }

        private void subscribeRoom()
        {
            _room.PropertyChanged += onRoomPropertyChanged;
            subscribeRoomAreas();
        }

        private void subscribeRoomAreas()
        {
            var room = _room?.Room;
            if (room == null) return;
            room.Areas.OnListChanged.Subscribe(onAreasChanged);
            foreach (var area in room.Areas) subscribeArea(area);
        }

        private void onRoomPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IHasRoomComponent.Room)) return;
            unsubscribeRoomAreas();
            _lastRoom = _room.Room;
            subscribeRoomAreas();
            refreshAreaScaling();
        }

        private void unsubscribeRoom(IHasRoomComponent room)
        {
            room.PropertyChanged -= onRoomPropertyChanged;
            unsubscribeRoomAreas();
        }

        private void unsubscribeRoomAreas()
        {
            var room = _lastRoom;
            if (room == null) return;
            room.Areas.OnListChanged.Unsubscribe(onAreasChanged);
            foreach (var area in room.Areas) unsubscribeArea(area);
        }

        private void onAreasChanged(AGSListChangedEventArgs<IArea> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var area in args.Items) subscribeArea(area.Item);
            }
            else foreach (var area in args.Items) unsubscribeArea(area.Item);
            refreshAreaScaling();
        }

        private void subscribeArea(IArea area)
        {
            var areaBinding = area.Bind<IAreaComponent>(c => c.PropertyChanged += onAreaPropertyChanged, c => c.PropertyChanged -= onAreaPropertyChanged);
            var scaleBinding = area.Bind<IScalingArea>(c => c.PropertyChanged += onScalingAreaChanged, c => c.PropertyChanged -= onScalingAreaChanged);
            var restrictBinding = area.Bind<IAreaRestriction>(c =>
            {
                c.RestrictionList.OnListChanged.Subscribe(onAreaRestrictListChanged);
                c.PropertyChanged += onAreaRestrictionChanged;
            }, c => 
            {
                c.RestrictionList.OnListChanged.Unsubscribe(onAreaRestrictListChanged);
                c.PropertyChanged -= onAreaRestrictionChanged;
            });
            var bindings = _areaBindings.GetOrAdd(area.ID, () => new List<IComponentBinding>());
            bindings.Add(scaleBinding);
            bindings.Add(areaBinding);
            bindings.Add(restrictBinding);
        }

        private void onAreaRestrictListChanged(AGSHashSetChangedEventArgs<string> obj)
        {
            refreshAreaScaling();
        }

        private void onAreaRestrictionChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshAreaScaling();
        }

        private void onAreaPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshAreaScaling();
        }

        private void unsubscribeArea(IArea area)
        {
            var areaComp = area.GetComponent<IAreaComponent>();
            if (areaComp != null) areaComp.PropertyChanged -= onAreaPropertyChanged;
            var scale = area.GetComponent<IScalingArea>();
            if (scale != null) scale.PropertyChanged -= onScalingAreaChanged;
            var restriction = area.GetComponent<IAreaRestriction>();
            if (restriction != null)
            {
                restriction.PropertyChanged -= onAreaRestrictionChanged;
                restriction.RestrictionList.OnListChanged.Unsubscribe(onAreaRestrictListChanged);
            }
            _areaBindings.TryGetValue(area.ID, out var bindings);
            if (bindings == null) return;
            foreach (var binding in bindings) binding.Unbind();
        }

        private void onScalingAreaChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScalingArea.MinScaling) &&
                args.PropertyName != nameof(IScalingArea.MaxScaling) &&
                args.PropertyName != nameof(IScalingArea.Axis) &&
                args.PropertyName != nameof(IScalingArea.ScaleObjectsX) &&
                args.PropertyName != nameof(IScalingArea.ScaleObjectsY)) return;
            refreshAreaScaling();
        }

        private void refreshAreaScaling()
        {
            _areaScaling = getAreaScaling();
            onSomethingChanged();
        }

        private PointF getAreaScaling()
        {
            var room = _room?.Room;
            if (room == null || (_drawable?.IgnoreScalingArea ?? false)) return NoScaling;

            //Problem: we'd like to always use the world position when checking if the entity is standing
            //in a scaling area. However, the world position is calculated from the bounding box, which
            //in itself is calculated from the matrix, which is calculated here... 
            //So at best this will always be in one frame delay, and at worst can cause real scaling jittering issues.
            //So this is why we're checking and in cases the entity doesn't have a parent (which is the vast majority of scenarios)
            //we'll continue using the local coordinates, at least until a better solution is found.
            var position = _tree.TreeNode.Parent == null ? _translate.Position.XY : _worldPosition.WorldXY;

            foreach (IArea area in room.GetMatchingAreas(position, Entity.ID))
            {
                IScalingArea scaleArea = area.GetComponent<IScalingArea>();
                if (scaleArea == null || (!scaleArea.ScaleObjectsX && !scaleArea.ScaleObjectsY)) continue;
                float scale = scaleArea.GetScaling(scaleArea.Axis == ScalingAxis.X ? position.X : position.Y);
                return new PointF(scaleArea.ScaleObjectsX ? scale : 1f, scaleArea.ScaleObjectsY ? scale : 1f);
            }
            return NoScaling;
        }
    }
}
