using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSBoundingBoxWithChildrenComponent : AGSComponent, IBoundingBoxWithChildrenComponent
    {
        private IBoundingBoxComponent _boundingBox;
        private IInObjectTree _tree;
        private IEntity _entity;

        public AGSBoundingBoxWithChildrenComponent()
        {
            OnBoundingBoxWithChildrenChanged = new AGSEvent<object>();
        }

        public AGSBoundingBox BoundingBoxWithChildren { get; private set; }

        public AGSBoundingBox PreCropBoundingBoxWithChildren { get; private set; }

        public IEvent<object> OnBoundingBoxWithChildrenChanged { get; private set; }

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

        private void onObjectChanged(object args)
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
            obj.OnBoundingBoxesChanged.Subscribe(onObjectChanged);
            obj.OnUnderlyingVisibleChanged.Subscribe(onObjectChanged);
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            if (labelRenderer != null) labelRenderer.OnLabelSizeChanged.Subscribe(onObjectChanged);
        }

        private void unsubscribeObject(IObject obj)
        {
            obj.OnBoundingBoxesChanged.Unsubscribe(onObjectChanged);
            obj.OnUnderlyingVisibleChanged.Unsubscribe(onObjectChanged);
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            if (labelRenderer != null) labelRenderer.OnLabelSizeChanged.Unsubscribe(onObjectChanged);
        }


        private void refresh()
        {
            BoundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.RenderBox);
            PreCropBoundingBoxWithChildren = getBoundingBox(_tree, _boundingBox, boxes => boxes.PreCropRenderBox);
            OnBoundingBoxWithChildrenChanged.Invoke(null);
        }

        private AGSBoundingBox getBoundingBox(IInObjectTree tree, IBoundingBoxComponent box, Func<AGSBoundingBoxes, AGSBoundingBox> getBox)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            if (box != null)
            {
                var boxes = box.GetBoundingBoxes();
                if (boxes != null)
                {
                    var boundingBox = getBox(boxes);

                    minX = boundingBox.MinX;
                    maxX = boundingBox.MaxX;
                    minY = boundingBox.MinY;
                    maxY = boundingBox.MaxY;
                }
            }
            if (tree != null && tree.TreeNode != null)
            {
                foreach (var child in tree.TreeNode.Children)
                {
                    if (!child.UnderlyingVisible) continue;
                    var childBox = getBoundingBox(child, child, getBox);
                    if (childBox.IsInvalid) continue;
                    if (minX > childBox.MinX) minX = childBox.MinX;
                    if (maxX < childBox.MaxX) maxX = childBox.MaxX;
                    if (minY > childBox.MinY) minY = childBox.MinY;
                    if (maxY < childBox.MaxY) maxY = childBox.MaxY;
                }
            }
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (minX == float.MaxValue) return default(AGSBoundingBox);
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            return new AGSBoundingBox(minX, maxX, minY, maxY);
        }
    }
}
