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

        /// <summary>
        /// Applying a default sort index (the index the rendered item was added to the display list) will help 
        /// to reduce inconsistencies in the sorting when two items have the same z index and render layer (if the sorting 
        /// result is 0 for 2 items, they can be randomly sorted  which can effect other sorted items).
        /// </summary>
        public const string SortDefaultIndex = "SortDefaultIndex";

		#region IComparer implementation

		public int Compare(IObject s1, IObject s2)
		{
			int result = (int)compare(s1, s2);
            if (result == 0)
            {
                result = s2.Properties.Ints.GetValue(SortDefaultIndex) - s1.Properties.Ints.GetValue(SortDefaultIndex);
            }
			if (Backwards) result *= -1;
			#if DEBUG
            s1.Properties.Ints.SetValue(string.Format("Sort {0}{1}", Backwards ? "backwards " : "", s2.ID ?? "null"), result);
            s2.Properties.Ints.SetValue(string.Format("Sort {0}{1}", Backwards ? "backwards " : "", s1.ID ?? "null"), -result);
			#endif
			return result;
		}
			
		#endregion

		private float compare(IObject s1, IObject s2)
		{
			if (s1 == s2) return 0;
			int layer1 = getRenderLayer(s1);
			int layer2 = getRenderLayer(s2);
			if (layer1 != layer2) return layer2 - layer1;

			if (isParentOf(s1, s2)) return 1;
			if (isParentOf(s2, s1)) return -1;

			IObject parent1 = null;
			IObject parent2 = null;
			while (true)
			{
				IObject newParent1;
				IObject newParent2;
				float z1 = getZ(parent1, s1, out newParent1);
				float z2 = getZ(parent2, s2, out newParent2);
				if (z1 != z2) return z2 - z1;
                if (newParent1 == null || newParent2 == null || (newParent1 == s1 && newParent2 == s2))
				{
					z1 = newParent1 == null ? parent1 == null ? 0 : getZ(parent1) : getZ(newParent1);
					z2 = newParent2 == null ? parent2 == null ? 0 : getZ(parent2) : getZ(newParent2);
                    if (z2 != z1)
                    {
                        return z2 - z1;
                    }
                    //Trying to avoid ambiguity, so using X as a last resort
                    var x1 = newParent1 == null ? parent1 == null ? 0 : getX(parent1) : getX(newParent1);
                    var x2 = newParent2 == null ? parent2 == null ? 0 : getX(parent2) : getX(newParent2);
                    return x2 - x1;
				}
				parent1 = newParent1;
				parent2 = newParent2;
			}
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
			return getRenderLayer(obj.TreeNode.Parent);
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

