using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        IInObjectTree _tree;

        public AGSStackLayoutComponent(IGame game)
        {
            RelativeSpacing = new PointF(0f, -1f); //a simple vertical layout top to bottom by default.
            game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public PointF AbsoluteSpacing { get; set; }
        public PointF RelativeSpacing { get; set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _tree = entity.GetComponent<IInObjectTree>();
        }

		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
            float x = 0f;
            float y = 0f;

            foreach (var child in _tree.TreeNode.Children) 
            {
                child.Location = new AGSLocation(x, y, child.Z);
                x += child.Width * RelativeSpacing.X + AbsoluteSpacing.X;
                y += child.Height * RelativeSpacing.Y + AbsoluteSpacing.Y;
            }
		}
	}
}
