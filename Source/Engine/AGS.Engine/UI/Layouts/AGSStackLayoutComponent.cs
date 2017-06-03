using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        IInObjectTree _tree;

        public AGSStackLayoutComponent(IGame game)
        {
            Direction = LayoutDirection.Vertical;
            RelativeSpacing = -1f; //a simple vertical layout top to bottom by default.
            game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public LayoutDirection Direction { get; set; }
        public float AbsoluteSpacing { get; set; }
        public float RelativeSpacing { get; set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _tree = entity.GetComponent<IInObjectTree>();
        }

        private void onRepeatedlyExecute(object sender, AGSEventArgs args)
        {
            float location = 0f;

            foreach (var child in _tree.TreeNode.Children)
            {
                if (!child.Visible) continue;
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
                if (!child.Visible) continue;
                var minMax = getMinMax(child, getMin, getLength);
                if (minMax.Item1 < min) min = minMax.Item1;
                if (minMax.Item2 > max) max = minMax.Item2;
            }

            return new Tuple<float, float>(min, max);
        }
	}
}
