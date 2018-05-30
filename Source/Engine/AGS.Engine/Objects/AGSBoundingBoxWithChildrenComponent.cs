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
        private readonly IGameEvents _events;
        private bool _isDirty;
        private EntityListSubscriptions<IObject> _subscriptions;

        public AGSBoundingBoxWithChildrenComponent(IGameState state, IGameEvents events)
        {
            _state = state;
            _events = events;
            EntitiesToSkip = new AGSConcurrentHashSet<string>();
            OnBoundingBoxWithChildrenChanged = new AGSEvent();
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        [Property(Browsable = false)]
        public ref AGSBoundingBox BoundingBoxWithChildren { get { return ref _boundingBoxWithChildren; } }

        [Property(Browsable = false)]
        public ref AGSBoundingBox PreCropBoundingBoxWithChildren { get { return ref _preCropBoundingBoxWithChildren; } }

        public IBlockingEvent OnBoundingBoxWithChildrenChanged { get; private set; }

        public IConcurrentHashSet<string> EntitiesToSkip { get; private set; }

        [Property(Browsable = false)]
        public ILockStep LockStep { get { return this;} }

        public bool DebugPrintouts { get; set; }

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

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            _entity = null;
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
            var textSubscription = new EntitySubscription<ITextComponent>(null, c =>
            {
                c.OnLabelSizeChanged.Subscribe(onObjLabelSizeChanged);
            }, c =>
            {
                c.OnLabelSizeChanged.Unsubscribe(onObjLabelSizeChanged);
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
            _isDirty = false;
            _boundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.ViewportBox, DebugPrintouts ? $"Box ({_entity.ID})" : null);
            _preCropBoundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.PreCropViewportBox, DebugPrintouts ? $"Box Pre Crop ({_entity.ID})" : null);
            if (DebugPrintouts)
            {
                Debug.WriteLine($"Pre crop for {_entity.ID}: {_preCropBoundingBoxWithChildren.ToString()}");
            }
            return (!lastBox.Equals(BoundingBoxWithChildren) || !lastPreBox.Equals(PreCropBoundingBoxWithChildren));
        }

        private bool recalculateBeforeUnlock()
        {
            var lastBox = BoundingBoxWithChildren;
            var lastPreBox = PreCropBoundingBoxWithChildren;
            _preUnlockBoundingBox = getBoundingBox(_tree, _boundingBox, boxes => boxes.ViewportBox, DebugPrintouts ? $"Box before unlock ({_entity.ID})" : null);
            _preUnlockPreCropBoundingBox = getBoundingBox(_tree, _boundingBox, boxes => boxes.PreCropViewportBox, DebugPrintouts ? $"Box Pre Crop before unlock ({_entity.ID})" : null);
            if (DebugPrintouts)
            {
                Debug.WriteLine($"Pre crop (before unlock) for ${_entity.ID}: {_preUnlockBoundingBox.ToString()}");
            }
            _isDirty = false;
            return (!lastBox.Equals(_preUnlockBoundingBox) || !lastPreBox.Equals(_preUnlockPreCropBoundingBox));
        }

        private AGSBoundingBox getBoundingBox(IInObjectTreeComponent tree, IBoundingBoxComponent box,
                          Func<AGSBoundingBoxes, AGSBoundingBox> getBox, string printoutId, int retries = 3)
        {
            try
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;

                var boxes = box?.GetBoundingBoxes(_state.Viewport);
                if (boxes != null)
                {
                    var boundingBox = getBox(boxes);

                    if (boundingBox.IsValid)
                    {
                        minX = boundingBox.MinX;
                        maxX = boundingBox.MaxX;
                        minY = boundingBox.MinY;
                        maxY = boundingBox.MaxY;
                    }

                    if (printoutId != null)
                    {
                        Debug.WriteLine($"{printoutId}: {minX}-{maxX}, {minY}-{maxY}");
                    }
                }

                if (tree?.TreeNode != null)
                {
                    foreach (var child in tree.TreeNode.Children)
                    {
                        if (child == null || !child.UnderlyingVisible || EntitiesToSkip.Contains(child.ID)) continue;

                        //note: the text component check is needed for textboxes. We have a "with caret" version of the textbox flashing in and out
                        //but we don't want to set it to visible and not visible because it hurts performance to keep changing the display list (which triggers sorting)
                        //so we hide the the text and text background with label renderer properties. Because it's hidden the textbox may not be updated which correct bounding boxes.
                        var textComponent = child.GetComponent<ITextComponent>();
                        if (textComponent != null && !textComponent.TextVisible && !child.IsImageVisible) continue;

                        var childBox = getBoundingBox(child, child, getBox, printoutId == null ? null : child.ID);
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
            catch (InvalidOperationException e) //retrying in case children collection was modified from another thread
            {
                Debug.WriteLine($"BoundingBoxWithChildren: Exception when iterating children. retries = {retries}, error: {e.Message}");
                if (retries <= 0) throw;
                return getBoundingBox(tree, box, getBox, printoutId, retries - 1);
            }
        }
    }
}
