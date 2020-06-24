﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
    public class AGSCropChildrenComponent : AGSComponent, ICropChildrenComponent
    {
        private IInObjectTreeComponent _tree;
        private PointF _startPoint;
        private IBoundingBoxComponent _boundingBox;
        private IDrawableInfoComponent _drawable;
        private bool _isDirty, _isCropEnabled;
        private readonly IGameState _state;
        private readonly IGameSettings _settings;
        private readonly ConcurrentDictionary<string, IComponentBinding[]> _bindings;
        private readonly Action<AGSHashSetChangedEventArgs<string>> _onEntitiesToSkipCropChangedCallback;
        private readonly Action<AGSListChangedEventArgs<IObject>> _onChildrenChangedCallback;
        private readonly PropertyChangedEventHandler _onLocationChangedCallback, _onVisibleChangedCallback;

        public AGSCropChildrenComponent(IGameState state, IGameSettings settings)
        {
            _state = state;
            _settings = settings;
            _bindings = new ConcurrentDictionary<string, IComponentBinding[]>();
            _isCropEnabled = true;

            _onChildrenChangedCallback = onChildrenChanged;
            _onEntitiesToSkipCropChangedCallback = onEntitiesToSkipCropChanged;
            _onLocationChangedCallback = onLocationChanged;
            _onVisibleChangedCallback = onVisibleChanged;

            EntitiesToSkipCrop = new AGSConcurrentHashSet<string>();
            EntitiesToSkipCrop.OnListChanged.Subscribe(_onEntitiesToSkipCropChangedCallback);
        }

        public bool CropChildrenEnabled
        {
            get => _isCropEnabled;
            set
            {
                if (_isCropEnabled == value) return;
                _isCropEnabled = value;
                if (value)
                {
                    rebuildJump(_tree);
                    rebuildEntireTree();
                }
            }
        }

        public PointF StartPoint { get => _startPoint; set { _startPoint = value; rebuildJump(_tree); rebuildEntireTree(); } }

        public IConcurrentHashSet<string> EntitiesToSkipCrop { get; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IBoundingBoxComponent>(c => { _boundingBox = c; }, _ => { _boundingBox = null; });
            Entity.Bind<IInObjectTreeComponent>(c => { subscribeTree(c); _tree = c; }, c => { unsubscribeTree(c); _tree = null; });
            Entity.Bind<IDrawableInfoComponent>(c => { _drawable = c; }, _ => { _drawable = null; });
            rebuildEntireTree();
        }

        public override void Dispose()
        {
            base.Dispose();
            var tree = _tree;
            if (tree != null) unsubscribeTree(tree);
            EntitiesToSkipCrop?.OnListChanged?.Unsubscribe(_onEntitiesToSkipCropChangedCallback);
        }

        private void onEntitiesToSkipCropChanged(AGSHashSetChangedEventArgs<string> _) => rebuildEntireTree();

        private void subscribeTree(IInObjectTreeComponent node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Subscribe(_onChildrenChangedCallback);
            foreach (var child in node.TreeNode.Children)
            {
                subscribeTree(child);
            }
            subscribeObject(node.TreeNode.Node);
        }

        private void unsubscribeTree(IInObjectTreeComponent node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Unsubscribe(_onChildrenChangedCallback);
            foreach (var child in node.TreeNode.Children)
            {
                unsubscribeTree(child);
            }
            unsubscribeObject(node.TreeNode.Node);
        }

        private void subscribeObject(IObject obj)
        {
            var bindings = new IComponentBinding[2];
            bindings[0] = obj.Bind<ITranslateComponent>(c => c.PropertyChanged += _onLocationChangedCallback, c => c.PropertyChanged -= _onLocationChangedCallback);
            bindings[1] = obj.Bind<IVisibleComponent>(c => c.PropertyChanged += _onVisibleChangedCallback, c => c.PropertyChanged -= _onVisibleChangedCallback);
            _bindings[obj.ID] = bindings;
            obj.OnDisposed(() => unsubscribeObject(obj));
        }

        private void unsubscribeObject(IObject obj)
        {
            IComponentBinding[] bindings;
            if (_bindings.TryRemove(obj.ID, out bindings))
            {
                foreach (var binding in bindings)
                {
                    binding.Unbind();
                }
            }
        }

        private void onVisibleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
            rebuildJump(_tree);
            onTreeChanged();
        }

        private void onLocationChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.Position)) return;
            onTreeChanged();
        }

        private void onChildrenChanged(AGSListChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var item in args.Items) subscribeTree(item.Item);
                rebuildJump(_tree);
            }
            else
            {
                foreach (var item in args.Items) unsubscribeTree(item.Item);
            }
            rebuildEntireTree();
            _isDirty = true;
        }

        private void onTreeChanged()
        {
            rebuildEntireTree();
            _isDirty = true;
        }

        private void rebuildEntireTree()
        {
            rebuildTree(_tree);
        }

        private void rebuildTree(IInObjectTreeComponent tree, int retries = 3)
        {
            try
            {
                if (tree == null || !_isCropEnabled) return;
                foreach (var child in tree.TreeNode.Children)
                {
                    if (!child.Visible) continue;
                    if (EntitiesToSkipCrop.Contains(child.ID))
                    {
                        if (child.HasComponent<ICropSelfComponent>()) removeCrop(child);
                        continue;
                    }
                    prepareCrop(child);
                    rebuildTree(child);
                }
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"CropChildrenComponent: Exception when iterating children. retries = {retries}, error: {e.Message}");
                if (retries <= 0) throw;
                rebuildTree(tree, retries - 1);
            }
        }

        private void removeCrop(IObject obj)
        {
            obj.RemoveComponent<ICropSelfComponent>();
            foreach (var child in obj.TreeNode.Children)
            {
                removeCrop(child);
            }
        }

        private void rebuildJump(IInObjectTreeComponent tree)
        {
            if (tree == null) return;
            foreach (var child in tree.TreeNode.Children)
			{
                if (!child.Visible || EntitiesToSkipCrop.Contains(child.ID)) continue;
				var jump = child.AddComponent<IJumpOffsetComponent>();
                if (jump != null)
                {
                    jump.JumpOffset = new PointF(-StartPoint.X, -StartPoint.Y);
                }
			}
        }

        private AGSBoundingBox getBoundingBox(AGSBoundingBoxes boundingBoxes, IObject obj, ITextComponent textComponent)
        {
            //If a text object does not use an independent resolution, it might be scaled up (using CustomImageResolutionFactor) and the 
            //bounding box for cropping might then be in a different resolution than the parent so we need to compensate, by resizing
            //the parent box to match the child box.
            //todo: this currently probably doesn't cover the case when the child and parent has different independent resolutions from each other.
            if (obj.RenderLayer?.IndependentResolution != null)
                return boundingBoxes.ViewportBox;

            float factorX = (textComponent?.CustomImageResolutionFactor?.X ?? 1f);
            factorX /= ((_drawable?.RenderLayer?.IndependentResolution?.Width ?? (float)_settings.VirtualResolution.Width) / _settings.VirtualResolution.Width);

            float factorY = (textComponent?.CustomImageResolutionFactor?.Y ?? 1f);
            factorY /= ((_drawable?.RenderLayer?.IndependentResolution?.Height ?? (float)_settings.VirtualResolution.Height) / _settings.VirtualResolution.Height);

            return boundingBoxes.ViewportBox.Multiply(factorX, factorY);
        }

        private void prepareCrop(IObject obj)
        {
            var boundingBox = _boundingBox;
            if (boundingBox == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes(_state.Viewport);
            if (boundingBoxes == null || obj.GetBoundingBoxes(_state.Viewport) == null) return;
            var cropSelf = obj.GetComponent<ICropSelfComponent>();
            var textComponent = obj.GetComponent<ITextComponent>();
            if (textComponent != null)
            {
                var textCropSelf = textComponent.CustomTextCrop;
                if (textCropSelf == null)
                {
                    textCropSelf = new AGSCropSelfComponent { CropText = true, NeverGuaranteedToFullyCrop = cropSelf?.NeverGuaranteedToFullyCrop ?? false};
                    ChildCropper cropper = new ChildCropper("Label: " + obj.ID, () => _isDirty, textCropSelf,
                                                            () => getBoundingBox(boundingBoxes, obj, textComponent));
                    textCropSelf.OnBeforeCrop.Subscribe(cropper.CropIfNeeded);
                    textCropSelf.Init(obj, typeof(ICropSelfComponent));
                    textCropSelf.AfterInit();
                    textComponent.CustomTextCrop = textCropSelf;
                    var self = textCropSelf;
                    obj.OnDisposed(() => self.OnBeforeCrop?.Unsubscribe(cropper.CropIfNeeded));
                }
            }
            if (cropSelf == null)
            {
                cropSelf = new AGSCropSelfComponent();
                cropSelf.CropEnabled = false;
                ChildCropper cropper = new ChildCropper(obj.ID, () => _isDirty, cropSelf, 
                                                        () => boundingBoxes.ViewportBox);
                cropSelf.OnBeforeCrop.Subscribe(cropper.CropIfNeeded);
                if (obj.AddComponent<ICropSelfComponent>(cropSelf))
                {
                    obj.OnDisposed(() => cropSelf.OnBeforeCrop.Unsubscribe(cropper.CropIfNeeded));
                }
                else
                {
                    cropSelf.OnBeforeCrop.Unsubscribe(cropper.CropIfNeeded);
                    cropSelf.Dispose();
                }
            }
        }

        private class ChildCropper
        {
            private readonly Func<bool> _isDirty;
            private readonly ICropSelfComponent _crop;
            private readonly Func<AGSBoundingBox> _parentBox;
            // ReSharper disable once NotAccessedField.Local
            private readonly string _id; //used for debugging purposes only

            public ChildCropper(string id, Func<bool> isDirty, ICropSelfComponent crop, Func<AGSBoundingBox> parentBox)
            {
                Trace.Assert(crop != null);
                Trace.Assert(parentBox != null);
                _id = id;
                _isDirty = isDirty;
                _crop = crop;
                _parentBox = parentBox;
            }

            public void CropIfNeeded(BeforeCropEventArgs eventArgs)
            {
                if (!_isDirty() || eventArgs.BoundingBoxType == BoundingBoxType.HitTest) return;
                _crop.CropEnabled = true;
                var parentBox = _parentBox();
                var childBox = eventArgs.BoundingBox;

				float minXParent = parentBox.MinX;
				float maxXParent = parentBox.MaxX;
				float minYParent = parentBox.MinY;
				float maxYParent = parentBox.MaxY;
				float minXChild = childBox.MinX;
				float maxXChild = childBox.MaxX;
				float minYChild = childBox.MinY;
				float maxYChild = childBox.MaxY;
				float leftParent = minXParent;
                float bottomParent = minYParent;
                float leftChild = minXChild;
                float bottomChild = minYChild;
                float left = leftParent - leftChild;
                float bottom = bottomParent - bottomChild;
				float width = Math.Min(maxXParent, maxXChild) - Math.Max(minXParent, minXChild);
				float height = Math.Min(maxYParent, maxYChild) - Math.Max(minYParent, minYChild);
				if (width < 0) width = 0f;
				if (height < 0) height = 0f;
				_crop.CropArea = new RectangleF(left, bottom, width, height);
            }
        }
    }
}
