using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSSizeWithChildrenComponent : AGSComponent, ISizeWithChildrenComponent
    {
        private ITranslate _translate;
        private IScale _scale;
        private IInObjectTree _tree;

        public AGSSizeWithChildrenComponent()
        {
            OnSizeWithChildrenChanged = new AGSEvent<object>();
        }

        public SizeF SizeWithChildren { get; private set; }

        public IEvent<object> OnSizeWithChildrenChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITranslateComponent>(c => { _translate = c; refresh(); }, _ => _translate = null);
            entity.Bind<IScaleComponent>(c =>
            {
                _scale = c; 
                _scale.OnScaleChanged.Subscribe(onObjectChanged); 
                refresh();
            }, c => { c.OnScaleChanged.Unsubscribe(onObjectChanged); _scale = null; });
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


		private void refresh()
        {
            SizeWithChildren = new SizeF(getWidthWithChildren(), getHeightWithChildren());
            OnSizeWithChildrenChanged.Invoke(null);
        }

		private float getWidthWithChildren()
		{
			return getObjLength(o => o.X, o => o.Width);
		}

		private float getHeightWithChildren()
		{
			return getObjLength(o => o.Y, o => o.Height);
		}

        private float getObjLength(Func<ITranslate, float> getMin, Func<IScale, float> getLength)
		{
            var translate = _translate;
            var scale = _scale;
            if (translate == null || scale == null) return 0f;
            var minMax = getMinMax(_tree, translate, scale, getMin, getLength);
			return minMax.Item2 - minMax.Item1;
		}

        private Tuple<float, float> getMinMax(IInObjectTree tree, ITranslate translate, IScale scale,
                                              Func<ITranslate, float> getMin, Func<IScale, float> getLength)
		{
            float length = getLength(scale);
            float min = getMin(translate);
			float max = min + length;

            if (tree != null)
            {
                foreach (var child in tree.TreeNode.Children)
                {
                    if (!child.UnderlyingVisible) continue;
                    var minMax = getMinMax(child, child, child, getMin, getLength);
                    if (minMax.Item1 < min) min = minMax.Item1;
                    if (minMax.Item2 > max) max = minMax.Item2;
                }
            }

			return new Tuple<float, float>(min, max);
		}
    }
}
