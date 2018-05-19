using AGS.API;

namespace AGS.Engine
{
	public class InObjectTreeComponent : AGSComponent, IInObjectTreeComponent
    {
        public override void Init(IEntity entity)
        {
            base.Init(entity);
            TreeNode = new AGSTreeNode<IObject>((IObject)entity);
        }

        public ITreeNode<IObject> TreeNode { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            TreeNode?.Dispose();
        }
    }
}

