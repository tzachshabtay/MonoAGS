using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSCropChildrenComponent : AGSComponent, ICropChildrenComponent
    {
        private IInObjectTree _tree;
        private IScaleComponent _scale;
        private PointF _startPoint;
        private ICollider _collider;

        public bool CropChildrenEnabled { get; set; }

        public PointF StartPoint { get { return _startPoint; } set { _startPoint = value; rebuildTree(_tree);} }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ICollider>(c => { _collider = c; }, _ => { _collider = null; });
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
            node.TreeNode.Node.OnLocationChanged.Subscribe(onTreeChanged);
		}

		private void unsubscribeTree(IInObjectTree node)
		{
			if (node == null) return;
			node.TreeNode.Children.OnListChanged.Unsubscribe(onTreeChanged);
			foreach (var child in node.TreeNode.Children)
			{
				unsubscribeTree(child);
			}
            node.TreeNode.Node.OnLocationChanged.Unsubscribe(onTreeChanged);
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
		}

        private void onTreeChanged(object obj)
        {
            rebuildTree(_tree);
        }

        private void rebuildTree(IInObjectTree tree)
        {
            if (tree == null) return;
            foreach (var child in tree.TreeNode.Children)
            {
                crop(child);
                rebuildTree(child);
            }
        }

        private void crop(IObject obj)
        {
            var scale = _scale;
            if (scale == null || obj.BoundingBoxes == null) return;
            var collider = _collider;
            if (collider == null || collider.BoundingBoxes == null) return;
			if (obj.Width == 0f || obj.Height == 0f) return;
            var childBox = obj.BoundingBoxes.RenderBox;
            var parentBox = collider.BoundingBoxes.RenderBox;
            if (childBox.MaxX == childBox.MinX && childBox.MaxY == childBox.MinY) return;
            if (parentBox.MaxX == parentBox.MinX && parentBox.MaxY == parentBox.MinY) return;
			var cropSelf = obj.AddComponent<ICropSelfComponent>();
            float minXParent = parentBox.MinX;
            float maxXParent = parentBox.MaxX;
			float minYParent = parentBox.MinY;
            float maxYParent = parentBox.MaxY;
            float minXChild = childBox.MinX;
            float maxXChild = childBox.MaxX;
            float minYChild = childBox.MinY;
            float maxYChild = childBox.MaxY;
            float width = Math.Min(maxXParent, maxXChild) - Math.Max(minXParent, minXChild);
            float height = Math.Min(maxYParent, maxYChild) - Math.Max(minYParent, minYChild);
            float x = 0f;
            float y = 0f;
            cropSelf.CropArea = new RectangleF(x, y, width, height);
        }
    }
}
