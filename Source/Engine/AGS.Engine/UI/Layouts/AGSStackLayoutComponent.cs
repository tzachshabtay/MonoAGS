using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        private IInObjectTree _tree;
        private float _absoluteSpacing, _relativeSpacing;
        private LayoutDirection _direction;
        private bool _isPaused;
        private IEntity _entity;

        public AGSStackLayoutComponent()
        {
            _isPaused = true;
            OnLayoutChanged = new AGSEvent<object>();
            _direction = LayoutDirection.Vertical;
            _relativeSpacing = -1f; //a simple vertical layout top to bottom by default.
        }

        public LayoutDirection Direction { get { return _direction; } set { _direction = value; adjustLayout(); } }
        public float AbsoluteSpacing { get { return _absoluteSpacing; } set { _absoluteSpacing = value; adjustLayout(); } }
        public float RelativeSpacing { get { return _relativeSpacing; } set { _relativeSpacing = value; adjustLayout(); } }
        public IEvent<object> OnLayoutChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);
            entity.Bind<IInObjectTree>(c => { _tree = c; subscribeTree(c.TreeNode); adjustLayout(); }, 
                                       c => { unsubscribeTree(c.TreeNode); _tree = null; });
            adjustLayout();
        }

        public void StartLayout()
        {
            _isPaused = false;
            adjustLayout();
        }

        public void StopLayout()
        {
            _isPaused = true;
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
            adjustLayout();
        }

        private void onObjectChanged(object args)
        {
            adjustLayout();
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
            obj.OnScaleChanged.Subscribe(onObjectChanged);
            obj.OnImageChanged.Subscribe(onObjectChanged);
            obj.OnUnderlyingVisibleChanged.Subscribe(onObjectChanged);
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            if (labelRenderer != null) labelRenderer.OnLabelSizeChanged.Subscribe(onObjectChanged);
        }

        private void unsubscribeObject(IObject obj)
        {
            obj.OnImageChanged.Unsubscribe(onObjectChanged);
            obj.OnScaleChanged.Unsubscribe(onObjectChanged);
            obj.OnUnderlyingVisibleChanged.Unsubscribe(onObjectChanged);
            var labelRenderer = obj.CustomRenderer as ILabelRenderer;
            if (labelRenderer != null) labelRenderer.OnLabelSizeChanged.Unsubscribe(onObjectChanged);
        }

        private void adjustLayout()
        {
            if (_isPaused) return;
            float location = 0f;

            var tree = _tree;
            if (tree == null) return;
            foreach (var child in tree.TreeNode.Children)
            {
                if (!child.UnderlyingVisible) continue;
                float step;
                if (Direction == LayoutDirection.Vertical)
                {
                    child.Y = location;
                    step = getHeightWithChildren(child);
                }
                else
                {
                    child.X = location;
                    step = getWidthWithChildren(child);
                }
                location += step * RelativeSpacing + AbsoluteSpacing;
            }
            OnLayoutChanged.Invoke(null);
        }

        private float getWidthWithChildren(IObject obj)
        {
            return getObjLength(obj, o => o.X, o => o.Width);
        }

        private float getHeightWithChildren(IObject obj)
        {
            return getObjLength(obj, o => o.Y, o => o.Height);
        }

        private float getObjLength(IObject obj, Func<IObject, float> getMin, Func<IObject, float> getLength)
        {
            var minMax = getMinMax(obj, getMin, getLength);
            return minMax.Item2 - minMax.Item1;
        }

        private Tuple<float, float> getMinMax(IObject obj, Func<IObject, float> getMin, Func<IObject, float> getLength)
        {
            float length = getLength(obj);
            float min = getMin(obj);
            float max = min + length;

            foreach (var child in obj.TreeNode.Children)
            {
                if (!child.UnderlyingVisible) continue;
                var minMax = getMinMax(child, getMin, getLength);
                if (minMax.Item1 < min) min = minMax.Item1;
                if (minMax.Item2 > max) max = minMax.Item2;
            }

            return new Tuple<float, float>(min, max);
        }
	}
}
