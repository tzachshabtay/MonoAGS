﻿using System;
using System.ComponentModel;
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
        private IInObjectTree _tree;
        private IEntity _entity;
        private readonly IGameState _state;
        private bool _isDirty;

        public AGSBoundingBoxWithChildrenComponent(IGameState state)
        {
            _state = state;
            EntitiesToSkip = new AGSConcurrentHashSet<string>();
            OnBoundingBoxWithChildrenChanged = new AGSEvent();
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
                c.OnBoundingBoxesChanged.Subscribe(onObjectChanged);
                refresh();
            }, c => { c.OnBoundingBoxesChanged.Unsubscribe(onObjectChanged); _boundingBox = null; });
            entity.Bind<IInObjectTree>(c => { _tree = c; subscribeTree(c.TreeNode); refresh(); },
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

        private void onTreeChanged(AGSListChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var item in args.Items)
                {
                    subscribeObject(item.Item);
                    subscribeTree(item.Item.TreeNode);
                }
            }
            else
            {
                foreach (var item in args.Items)
                {
                    unsubscribeObject(item.Item);
                    unsubscribeTree(item.Item.TreeNode);
                }
            }
            refresh();
        }

        private void onObjectChanged()
        {
            refresh();
        }

        private void subscribeTree(ITreeNode<IObject> node)
        {
            node.Children.OnListChanged.Subscribe(onTreeChanged);
            foreach (var child in node.Children)
            {
                subscribeObject(child);
                subscribeTree(child.TreeNode);
            }
        }

        private void unsubscribeTree(ITreeNode<IObject> node)
        {
            node.Children.OnListChanged.Subscribe(onTreeChanged);
            foreach (var child in node.Children)
            {
                unsubscribeObject(child);
                unsubscribeTree(child.TreeNode);
            }
        }

        private void subscribeObject(IObject obj)
        {
            obj.Bind<IBoundingBoxComponent>(c => c.OnBoundingBoxesChanged.Subscribe(onObjectChanged), c => c.OnBoundingBoxesChanged.Unsubscribe(onObjectChanged));
            obj.Bind<IVisibleComponent>(c => c.PropertyChanged += onVisiblePropertyChanged, c => c.PropertyChanged -= onVisiblePropertyChanged);
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            labelRenderer?.OnLabelSizeChanged.Subscribe(onObjectChanged);
        }

        private void unsubscribeObject(IObject obj)
        {
            obj.OnBoundingBoxesChanged.Unsubscribe(onObjectChanged); //todo: unbind
            var visible = obj.GetComponent<IVisibleComponent>();
            if (visible != null) visible.PropertyChanged -= onVisiblePropertyChanged;
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            labelRenderer?.OnLabelSizeChanged.Unsubscribe(onObjectChanged);
        }

        private void onVisiblePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IVisibleComponent.UnderlyingVisible)) return;
            onObjectChanged();
        }

        private void refresh()
        {
            _isDirty = true;
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

        private AGSBoundingBox getBoundingBox(IInObjectTree tree, IBoundingBoxComponent box, Func<AGSBoundingBoxes, AGSBoundingBox> getBox)
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
                    var childBox = getBoundingBox(child, child, getBox);
                    if (childBox.IsInvalid) continue;
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
