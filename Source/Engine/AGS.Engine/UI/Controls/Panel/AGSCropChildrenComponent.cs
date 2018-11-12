using System;
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
        private bool _isDirty, _isCropEnabled;
        private readonly IGameState _state;
        private readonly ConcurrentDictionary<string, IComponentBinding[]> _bindings;

        public AGSCropChildrenComponent(IGameState state)
        {
            _state = state;
            _bindings = new ConcurrentDictionary<string, IComponentBinding[]>();
            _isCropEnabled = true;
            EntitiesToSkipCrop = new AGSConcurrentHashSet<string>();
            EntitiesToSkipCrop.OnListChanged.Subscribe(onEntitiesToSkipCropChanged);
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
            rebuildEntireTree();
        }

        public override void Dispose()
        {
            base.Dispose();
            var tree = _tree;
            if (tree != null) unsubscribeTree(tree);
            EntitiesToSkipCrop?.OnListChanged?.Unsubscribe(onEntitiesToSkipCropChanged);
        }

        private void onEntitiesToSkipCropChanged(AGSHashSetChangedEventArgs<string> _) => rebuildEntireTree();

        private void subscribeTree(IInObjectTreeComponent node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Subscribe(onChildrenChanged);
            foreach (var child in node.TreeNode.Children)
            {
                subscribeTree(child);
            }
            subscribeObject(node.TreeNode.Node);
        }

        private void unsubscribeTree(IInObjectTreeComponent node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Unsubscribe(onChildrenChanged);
            foreach (var child in node.TreeNode.Children)
            {
                unsubscribeTree(child);
            }
            unsubscribeObject(node.TreeNode.Node);
        }

        private void subscribeObject(IObject obj)
        {
            var bindings = new IComponentBinding[2];
            bindings[0] = obj.Bind<ITranslateComponent>(c => c.PropertyChanged += onLocationChanged, c => c.PropertyChanged -= onLocationChanged);
            bindings[1] = obj.Bind<IVisibleComponent>(c => c.PropertyChanged += onVisibleChanged, c => c.PropertyChanged -= onVisibleChanged);
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
            var visible = obj.GetComponent<IVisibleComponent>();
            if (visible != null)
            {
                visible.PropertyChanged -= onVisibleChanged;
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

        private void prepareCrop(IObject obj)
        {
            var boundingBox = _boundingBox;
            if (boundingBox == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes(_state.Viewport);
            if (boundingBoxes == null || obj.GetBoundingBoxes(_state.Viewport) == null) return;
            ICropSelfComponent cropSelf;
            var textComponent = obj.GetComponent<ITextComponent>();
            if (textComponent?.TextBoundingBoxes != null)
            {
                cropSelf = textComponent.CustomTextCrop;
                if (cropSelf == null)
                {
                    cropSelf = new AGSCropSelfComponent();
                    ChildCropper cropper = new ChildCropper("Label: " + obj.ID, () => _isDirty, cropSelf, () => boundingBoxes.ViewportBox);
                    cropSelf.OnBeforeCrop.Subscribe(cropper.CropIfNeeded);
                    cropSelf.Init(obj, typeof(ICropSelfComponent));
                    cropSelf.AfterInit();
                    textComponent.CustomTextCrop = cropSelf;
                    var self = cropSelf;
                    obj.OnDisposed(() => self.OnBeforeCrop?.Unsubscribe(cropper.CropIfNeeded));
                }
            }
            cropSelf = obj.GetComponent<ICropSelfComponent>();
            if (cropSelf == null)
            {
                cropSelf = new AGSCropSelfComponent();
                cropSelf.CropEnabled = false;
                ChildCropper cropper = new ChildCropper(obj.ID, () => _isDirty, cropSelf, () => boundingBoxes.ViewportBox);
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
            private readonly string _id;

            public ChildCropper(string id, Func<bool> isDirty, ICropSelfComponent crop, Func<AGSBoundingBox> parentBox)
            {
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
                float left = leftChild > leftParent ? 0f : leftParent - leftChild;
                float bottom = bottomChild > bottomParent ? 0f : bottomParent - bottomChild;
				float width = Math.Min(maxXParent, maxXChild) - Math.Max(minXParent, minXChild);
				float height = Math.Min(maxYParent, maxYChild) - Math.Max(minYParent, minYChild);
				if (width < 0) width = 0f;
				if (height < 0) height = 0f;
				_crop.CropArea = new RectangleF(left, bottom, width, height);
            }
        }
    }
}
