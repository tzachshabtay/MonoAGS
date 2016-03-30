using System;
using AGS.API;

namespace AGS.Engine
{
	public class InTreeComponent<TItem> : AGSComponent, IInTree<TItem> where TItem : class, IInTree<TItem>
	{
		public override void Init(IEntity entity)
		{
			base.Init(entity);
			TreeNode = new AGSTreeNode<TItem> ((TItem)entity);
		}

		public ITreeNode<TItem> TreeNode { get; set; }
	}
}

