using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSCropChildrenComponent : AGSComponent, ICropChildrenComponent
    {
        private IInObjectTree _tree;
        private IScaleComponent _scale;
        private PointF _startPoint;
        private IBoundingBoxComponent _boundingBox;
        private bool _isDirty;

        public AGSCropChildrenComponent()
        {
            EntitiesToSkipCrop = new AGSConcurrentHashSet<string>();
            EntitiesToSkipCrop.OnListChanged.Subscribe(_ => rebuildTree(_tree));
        }

        public bool CropChildrenEnabled { get; set; }

        public PointF StartPoint { get { return _startPoint; } set { _startPoint = value; rebuildTree(_tree); } }

        public IConcurrentHashSet<string> EntitiesToSkipCrop { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IBoundingBoxComponent>(c => { _boundingBox = c; }, _ => { _boundingBox = null; });
            entity.Bind<IInObjectTree>(c => { subscribeTree(c); _tree = c; }, c => { unsubscribeTree(c); _tree = null; });
            entity.Bind<IScaleComponent>(c => { c.OnScaleChanged.Subscribe(onTreeChanged); _scale = c; rebuildTree(_tree); },
                                         c => { c.OnScaleChanged.Unsubscribe(onTreeChanged); _scale = null; });
        }

        private void subscribeTree(IInObjectTree node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Subscribe(onTreeChanged);
            foreach (var child in node.TreeNode.Children)
            {
                subscribeTree(child);
            }
            subscribeObject(node.TreeNode.Node);
        }

        private void unsubscribeTree(IInObjectTree node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Unsubscribe(onTreeChanged);
            foreach (var child in node.TreeNode.Children)
            {
                unsubscribeTree(child);
            }
            unsubscribeObject(node.TreeNode.Node);
        }

        private void subscribeObject(IObject obj)
        {
            obj.OnLocationChanged.Subscribe(onTreeChanged);
            obj.OnUnderlyingVisibleChanged.Subscribe(onTreeChanged);
        }

        private void unsubscribeObject(IObject obj)
        {
            obj.OnLocationChanged.Unsubscribe(onTreeChanged);
            obj.OnUnderlyingVisibleChanged.Unsubscribe(onTreeChanged);
        }

        private void onTreeChanged(AGSListChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var item in args.Items) subscribeTree(item.Item);
            }
            else
            {
                foreach (var item in args.Items) unsubscribeTree(item.Item);
            }
            rebuildTree(_tree);
            _isDirty = true;
        }

        private void onTreeChanged(object obj)
        {
            rebuildTree(_tree);
            _isDirty = true;
        }

        private void rebuildTree(IInObjectTree tree)
        {
            if (tree == null) return;
            rebuildJump(tree);
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

        private void removeCrop(IObject obj)
        {
            obj.RemoveComponents<ICropSelfComponent>();
            foreach (var child in obj.TreeNode.Children)
            {
                removeCrop(child);
            }
        }

        private void rebuildJump(IInObjectTree tree)
        {
			foreach (var child in tree.TreeNode.Children)
			{
				if (!child.Visible || EntitiesToSkipCrop.Contains(child.ID)) continue;
				var jump = child.AddComponent<IJumpOffsetComponent>();
				jump.JumpOffset = new PointF(-StartPoint.X, -StartPoint.Y);
                rebuildJump(child);
			}
        }

        private void prepareCrop(IObject obj)
        {
            var boundingBox = _boundingBox;
            if (boundingBox == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes();
            if (boundingBoxes == null || obj.GetBoundingBoxes() == null) return;
            ICropSelfComponent cropSelf;
            var labelRenderer = obj.CustomRenderer as GLLabelRenderer;
            if (labelRenderer != null && labelRenderer.TextBoundingBoxes != null)
            {
                cropSelf = labelRenderer.CustomTextCrop;
                if (cropSelf == null)
                {
                    cropSelf = new AGSCropSelfComponent();
                    ChildCropper cropper = new ChildCropper("Label: " + obj.ID, () => _isDirty, cropSelf, () => boundingBoxes.RenderBox,
                                                            () => StartPoint);
                    cropSelf.OnBeforeCrop.Subscribe(cropper.CropIfNeeded);
                    cropSelf.Init(obj);
                    cropSelf.AfterInit();
                    labelRenderer.CustomTextCrop = cropSelf;
                }
            }
            cropSelf = obj.GetComponent<ICropSelfComponent>();
            if (cropSelf == null)
            {
                cropSelf = new AGSCropSelfComponent();
                cropSelf.CropEnabled = false;
                ChildCropper cropper = new ChildCropper(obj.ID, () => _isDirty, cropSelf, () => boundingBoxes.RenderBox,
															() => StartPoint);
                cropSelf.OnBeforeCrop.Subscribe(cropper.CropIfNeeded);
                obj.AddComponent(cropSelf);
            }
        }

        private class ChildCropper
        {
            private readonly Func<bool> _isDirty;
            private readonly ICropSelfComponent _crop;
            private readonly Func<AGSBoundingBox> _parentBox;
            private readonly Func<PointF> _startPoint;
            private readonly string _id;

            public ChildCropper(string id, Func<bool> isDirty, ICropSelfComponent crop, Func<AGSBoundingBox> parentBox, 
                                Func<PointF> startPoint)
            {
                _id = id;
                _isDirty = isDirty;
                _crop = crop;
                _parentBox = parentBox;
                _startPoint = startPoint;
            }

            public void CropIfNeeded(BeforeCropEventArgs eventArgs)
            {
                if (!_isDirty() || eventArgs.BoundingBoxType == BoundingBoxType.HitTest) return;
                _crop.CropEnabled = true;
                var parentBox = _parentBox();
                var childBox = eventArgs.BoundingBox;
                var startPoint = _startPoint();

				float minXParent = parentBox.MinX;
				float maxXParent = parentBox.MaxX;
				float minYParent = parentBox.MinY;
				float maxYParent = parentBox.MaxY;
				float minXChild = childBox.MinX - startPoint.X;
				float maxXChild = childBox.MaxX - startPoint.X;
				float minYChild = childBox.MinY - startPoint.Y;
				float maxYChild = childBox.MaxY - startPoint.Y;
				float startX = minXParent;
                float startY = minYParent;
                float x = childBox.MinX > startX ? 0f : startX - childBox.MinX;
                float y = childBox.MinY > startY ? 0f : startY - childBox.MinY;
				float width = Math.Min(maxXParent, maxXChild) - Math.Max(minXParent, minXChild);
				float height = Math.Min(maxYParent, maxYChild) - Math.Max(minYParent, minYChild);
				if (width < 0) width = 0f;
				if (height < 0) height = 0f;
				_crop.CropArea = new RectangleF(x, y, width, height);
            }
        }
    }
}
