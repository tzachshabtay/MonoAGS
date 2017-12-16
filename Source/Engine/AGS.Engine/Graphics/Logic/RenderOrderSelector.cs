using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class RenderOrderSelector : IComparer<IObject>
    {
        public RenderOrderSelector()
        {
        }

        public bool Backwards { get; set; }

#if DEBUG
        public static bool Printouts { get; set; }
#endif

        #region IComparer implementation

        public int Compare(IObject s1, IObject s2)
		{
            float resultF = compare(s1, s2);
            int result = resultF > 0f ? 1 : resultF < 0 ? -1 : string.Compare(s1.ID, s2.ID);
			if (Backwards) result *= -1;
#if DEBUG
            if (Printouts)
            {
                s1.Properties.Ints.SetValue($"Sort {(Backwards ? "backwards " : "")}{s2.ID ?? "null"}", result);
                s2.Properties.Ints.SetValue($"Sort {(Backwards ? "backwards " : "")}{s1.ID ?? "null"}", -result);
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

			IObject parent1 = null;
			IObject parent2 = null;
			while (true)
			{
                IObject newParent1 = getNewParent(parent1, s1);
                IObject newParent2 = getNewParent(parent2, s2);
                if (newParent1 != newParent2)
                {
                    float result = compareObj(newParent1, newParent2);
                    if (result != 0f) return result;
                }
                if (newParent1 == null || newParent2 == null || (newParent1 == s1 && newParent2 == s2))
                {
					//Trying to avoid ambiguity, so using X as a last resort
					return getX(s2) - getX(s1);
                }
				parent1 = newParent1;
                parent2 = newParent2;
			}
		}

        private float compareObj(IObject o1, IObject o2)
        {
			int layer1 = getRenderLayer(o1);
			int layer2 = getRenderLayer(o2);
			if (layer1 != layer2) return layer2 - layer1;

			float z1 = getZ(o1);
			float z2 = getZ(o2);
            if (z1 != z2) return z2 - z1;
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

        private IObject getNewParent(IObject parent, IObject obj)
        {
			var newParent = obj;

			while (newParent != null && newParent.TreeNode.Parent != (parent == null ? null : parent.TreeNode.Node))
			{
				newParent = newParent.TreeNode.Parent;
			}
            return newParent;
        }

		private float getZ(IObject parent, IObject obj, out IObject newParent)
		{
			newParent = obj;

			while (newParent != null && newParent.TreeNode.Parent != (parent == null ? null : parent.TreeNode.Node))
			{
				newParent = newParent.TreeNode.Parent;
			}
			return getZ(newParent ?? obj);
		}

		private float getZ(IObject obj)
		{
            float zAnimation = obj.Animation == null || obj.Animation.Sprite == null ? 0f : obj.Animation.Sprite.Z;
			return obj.Z + zAnimation;
		}

        private float getX(IObject obj)
        {
            float xAnimation = obj.Animation == null || obj.Animation.Sprite == null ? 0f : obj.Animation.Sprite.X;
            return obj.X + xAnimation;
        }
	}
}

