using AGS.API;

namespace AGS.Engine
{
	public class InObjectTreeComponent : AGSComponent, IInObjectTree
    {
        public override void Init(IEntity entity)
        {
            base.Init(entity);
            TreeNode = new AGSTreeNode<IObject>((IObject)entity);
        }

        public ITreeNode<IObject> TreeNode { get; set; }
    }
}

