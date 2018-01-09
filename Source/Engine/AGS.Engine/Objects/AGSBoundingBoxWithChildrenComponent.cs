﻿using System;
using System.Diagnostics;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSBoundingBoxWithChildrenComponent : AGSComponent, IBoundingBoxWithChildrenComponent, ILockStep
    {
        private IBoundingBoxComponent _boundingBox;
        private AGSBoundingBox _preUnlockBoundingBox, _preUnlockPreCropBoundingBox, _boundingBoxWithChildren, _preCropBoundingBoxWithChildren;
        private int _shouldFireOnUnlock, _pendingLocks;
        private IInObjectTreeComponent _tree;
        private IEntity _entity;
        private readonly IGameState _state;
        private bool _isDirty;
        private EntityListSubscriptions<IObject> _subscriptions;

        public AGSBoundingBoxWithChildrenComponent(IGameState state, IGameEvents events)
        {
            _state = state;
            EntitiesToSkip = new AGSConcurrentHashSet<string>();
            OnBoundingBoxWithChildrenChanged = new AGSEvent();
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public ref AGSBoundingBox BoundingBoxWithChildren { get { return ref _boundingBoxWithChildren; } }

        public ref AGSBoundingBox PreCropBoundingBoxWithChildren { get { return ref _preCropBoundingBoxWithChildren; } }

        public IBlockingEvent OnBoundingBoxWithChildrenChanged { get; private set; }

        public IConcurrentHashSet<string> EntitiesToSkip { get; private set; }

        public ILockStep LockStep { get { return this;} }

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);
            entity.Bind<IBoundingBoxComponent>(c =>
            {
                _boundingBox = c;
                c.OnBoundingBoxesChanged.Subscribe(onBoundingBoxChanged);
                onSomethingChanged();
            }, c => { c.OnBoundingBoxesChanged.Unsubscribe(onBoundingBoxChanged); _boundingBox = null; });
            entity.Bind<IInObjectTreeComponent>(c => { _tree = c; subscribeTree(c.TreeNode); onSomethingChanged(); },
                                       c => { unsubscribeTree(c.TreeNode); _tree = null; });
        }

        public void Lock()
        {
            Interlocked.Increment(ref _pendingLocks);
        }

        public void PrepareForUnlock()
        {
            if (!_isDirty) return;
            bool shouldFire = recalculateBeforeUnlock();
            _shouldFireOnUnlock += (shouldFire ? 1 : 0);
            if (shouldFire)
            {
                Interlocked.Increment(ref _shouldFireOnUnlock);
                _boundingBoxWithChildren = _preUnlockBoundingBox;
                _preCropBoundingBoxWithChildren = _preUnlockPreCropBoundingBox;
            }
        }

        public void Unlock()
        {
            if (Interlocked.Decrement(ref _pendingLocks) > 0) return;
            if (Interlocked.Exchange(ref _shouldFireOnUnlock, 0) >= 1)
            {
                OnBoundingBoxWithChildrenChanged.Invoke();
            }
        }

        private void onRepeatedlyExecute()
        {
            if (!_isDirty) return;
            refresh();
        }

        private void onSomethingChanged()
        {
            _isDirty = true;
        }

        private void onBoundingBoxChanged()
        {
            onSomethingChanged();
        }

        private void onObjBoundingBoxChanged()
        {
            onSomethingChanged();
        }

        private void onObjLabelSizeChanged()
        {
            onSomethingChanged();
        }

        private void subscribeTree(ITreeNode<IObject> node)
        {
            var visibleSubscription = new EntitySubscription<IVisibleComponent>(onVisiblePropertyChanged, 
                propertyNames: nameof(IVisibleComponent.UnderlyingVisible));
            var boundingBoxSubscription = new EntitySubscription<IBoundingBoxComponent>(null,
                c => c.OnBoundingBoxesChanged.Subscribe(onObjBoundingBoxChanged), c => c.OnBoundingBoxesChanged.Unsubscribe(onObjBoundingBoxChanged));
            var imageSubscription = new EntitySubscription<IImageComponent>(null, c =>
            {
                var labelRenderer = c.CustomRenderer as ILabelRenderer;
                labelRenderer?.OnLabelSizeChanged.Subscribe(onObjLabelSizeChanged);
            }, c =>
            {
                var labelRenderer = c.CustomRenderer as ILabelRenderer;
                labelRenderer?.OnLabelSizeChanged.Unsubscribe(onObjLabelSizeChanged);
            });
            _subscriptions = new EntityListSubscriptions<IObject>(node.Children, true, onSomethingChanged, 
                                                                  visibleSubscription, boundingBoxSubscription);
        }

        private void unsubscribeTree(ITreeNode<IObject> node)
        {
            _subscriptions?.Unsubscribe();
        }

        private void onVisiblePropertyChanged() => onSomethingChanged();

        private void refresh()
        {
            bool shouldFire = recalculate();
            if (shouldFire) OnBoundingBoxWithChildrenChanged.Invoke();
        }

        private bool recalculate()
        {
            if (_pendingLocks > 0) return false;
            var lastBox = BoundingBoxWithChildren;
            var lastPreBox = PreCropBoundingBoxWithChildren;
            _boundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.RenderBox);
            _preCropBoundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.PreCropRenderBox);
            _isDirty = false;
            return (!lastBox.Equals(BoundingBoxWithChildren) || !lastPreBox.Equals(PreCropBoundingBoxWithChildren));
        }

        private bool recalculateBeforeUnlock()
        {
            var lastBox = BoundingBoxWithChildren;
            var lastPreBox = PreCropBoundingBoxWithChildren;
            _preUnlockBoundingBox = getBoundingBox(_tree, _boundingBox, boxes => boxes.RenderBox);
            _preUnlockPreCropBoundingBox = getBoundingBox(_tree, _boundingBox, boxes => boxes.PreCropRenderBox);
            _isDirty = false;
            return (!lastBox.Equals(_preUnlockBoundingBox) || !lastPreBox.Equals(_preUnlockPreCropBoundingBox));
        }

        private AGSBoundingBox getBoundingBox(IInObjectTreeComponent tree, IBoundingBoxComponent box, Func<AGSBoundingBoxes, AGSBoundingBox> getBox)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            var boxes = box?.GetBoundingBoxes(_state.Viewport);
            if (boxes != null)
            {
                var boundingBox = getBox(boxes);

                minX = boundingBox.MinX;
                maxX = boundingBox.MaxX;
                minY = boundingBox.MinY;
                maxY = boundingBox.MaxY;
            }

            if (tree?.TreeNode != null)
            {
                foreach (var child in tree.TreeNode.Children)
                {
                    if (child == null || !child.UnderlyingVisible || EntitiesToSkip.Contains(child.ID)) continue;

                    //note: the label renderer check is needed for textboxes. We have a "with caret" version of the textbox flashing in and out
                    //but we don't want to set it to visible and not visible because it hurts performance to keep changing the display list (which triggers sorting)
                    //so we hide the the text and text background with label renderer properties. Because it's hidden the textbox may not be updated which correct bounding boxes.
                    var labelRenderer = child.CustomRenderer as ILabelRenderer;
                    if (labelRenderer != null && !labelRenderer.TextVisible && !labelRenderer.TextBackgroundVisible) continue;

                    var childBox = getBoundingBox(child, child, getBox);
                    if (!childBox.IsValid) continue;
                    if (minX > childBox.MinX) minX = childBox.MinX;
                    if (maxX < childBox.MaxX) maxX = childBox.MaxX;
                    if (minY > childBox.MinY) minY = childBox.MinY;
                    if (maxY < childBox.MaxY) maxY = childBox.MaxY;
                }
            }
            if (MathUtils.FloatEquals(minX, float.MaxValue)) return default;
            return new AGSBoundingBox(minX, maxX, minY, maxY);
        }
    }
}
