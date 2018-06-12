using AGS.API;

namespace AGS.Engine
{
	public class InObjectTreeComponent : AGSComponent, IInObjectTreeComponent
    {
        public override void Init()
        {
            base.Init();
            TreeNode = new AGSTreeNode<IObject>((IObject)Entity);
        }

        public ITreeNode<IObject> TreeNode { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            TreeNode?.Dispose();
        }
    }
}

