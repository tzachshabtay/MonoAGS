using System;
using System.Collections.Generic;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
    public class RenderOrderSelector : IComparer<IObject>
    {
        private readonly Stack<IObject> _stack1, _stack2;
        public RenderOrderSelector()
        {
            _stack1 = new Stack<IObject>(5);
            _stack2 = new Stack<IObject>(5);
        }

#if DEBUG
        public static bool Printouts { get; set; }
#endif

        #region IComparer implementation

        public int Compare(IObject s1, IObject s2)
		{
		    Trace.Assert(s1 != null);
		    Trace.Assert(s2 != null);
            float resultF = compare(s1, s2);
            int result = resultF > 0f ? 1 : resultF < 0 ? -1 : String.CompareOrdinal(s1.ID, s2.ID);
#if DEBUG
            if (Printouts)
            {
                s1.Properties.Ints.SetValue($"Sort {s2.ID ?? "null"}", result);
                s2.Properties.Ints.SetValue($"Sort {s1.ID ?? "null"}", -result);
            }
#endif
			return result;
		}
			
#endregion

		private float compare(IObject s1, IObject s2)
		{
            if (s1 == s2) return 0f;
			int layer1 = getRenderLayer(s1);
			int layer2 = getRenderLayer(s2);
			if (layer1 != layer2) return layer2 - layer1;

			if (isParentOf(s1, s2)) return 1f;
            if (isParentOf(s2, s1)) return -1f;

            getParentChain(s1, _stack1);
            getParentChain(s2, _stack2);
			while (true)
			{
                IObject newParent1 = _stack1.Count == 0 ? null : _stack1.Pop();
                IObject newParent2 = _stack2.Count == 0 ? null : _stack2.Pop();
                if (newParent1 != newParent2)
                {
                    float result = compareObj(newParent1, newParent2);
                    if (!MathUtils.FloatEquals(result, 0f)) return result;
                }
                if (newParent1 == null || newParent2 == null || (newParent1 == s1 && newParent2 == s2))
                {
					//Trying to avoid ambiguity, so using X as a last resort
					return getX(s2) - getX(s1);
                }
			}
		}

        private float compareObj(IObject o1, IObject o2)
        {
			int layer1 = getRenderLayer(o1);
			int layer2 = getRenderLayer(o2);
			if (layer1 != layer2) return layer2 - layer1;

			float z1 = getZ(o1);
			float z2 = getZ(o2);
            if (!MathUtils.FloatEquals(z1, z2)) return z2 - z1;
            return 0f;
        }

		private bool isParentOf(IObject child, IObject parent)
		{
			do
			{
				if (child == parent.TreeNode.Node) 
					return true;
				child = child.TreeNode.Parent;
			} while (child != null);
			return false;
		}

		private int getRenderLayer(IObject obj)
		{
			if (obj == null) return 0;
			if (obj.RenderLayer != null) return obj.RenderLayer.Z;
            return 0;
		}

        private void getParentChain(IObject obj, Stack<IObject> stack)
        {
            stack.Clear();
            while (obj != null) 
            {
                stack.Push(obj);
                obj = obj.TreeNode.Parent;
            }
        }

		private float getZ(IObject obj)
		{
            float zSprite = obj?.CurrentSprite?.Z ?? 0f;
            return (obj?.Z ?? 0f) + zSprite;
		}

        private float getX(IObject obj)
        {
            float xSprite = obj?.CurrentSprite?.X ?? 0f;
            return (obj?.X ?? 0f) + xSprite;
        }
	}
}
