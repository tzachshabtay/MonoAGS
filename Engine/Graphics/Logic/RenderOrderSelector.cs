
using System;
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

		#region IComparer implementation

		public int Compare(IObject s1, IObject s2)
		{
			int result = (int)compare(s1, s2);
			if (Backwards) result *= -1;
			return result;
		}
			
		#endregion

		private float compare(IObject s1, IObject s2)
		{
			int layer1 = getRenderLayer(s1);
			int layer2 = getRenderLayer(s2);
			if (layer1 != layer2) return layer2 - layer1;

			IObject parent1 = null;
			IObject parent2 = null;
			while (true)
			{
				IObject newParent1;
				IObject newParent2;
				float z1 = getZ(parent1, s1, out newParent1);
				float z2 = getZ(parent2, s2, out newParent2);
				if (z1 != z2) return z2 - z1;
				if (newParent1 == null || newParent2 == null || newParent1 == s1 || newParent2 == s2)
				{
					z1 = newParent1 == null ? parent1 == null ? 0 : getZ(parent1) : getZ(newParent1);
					z2 = newParent2 == null ? parent2 == null ? 0 : getZ(parent2) : getZ(newParent2);
					return z2 - z1;
				}
				parent1 = newParent1;
				parent2 = newParent2;
			}
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
			while (newParent.TreeNode.Parent != parent)
			{
				newParent = newParent.TreeNode.Parent;
			}
			return getZ(newParent);
		}

		private float getZ(IObject obj)
		{
			float zAnimation = obj.Animation == null ? 0f : obj.Animation.Sprite.Z;
			return obj.Z + zAnimation;
		}
	}
}

