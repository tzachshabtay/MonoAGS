using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeNodeView : ITreeNodeView
    {
        public AGSTreeNodeView(IUIControl treeItem, IButton expandButton, IPanel parentPanel, IPanel verticalPanel, IPanel horizontalPanel)
        {
            TreeItem = treeItem;
            ExpandButton = expandButton;
            ParentPanel = parentPanel;
            VerticalPanel = verticalPanel;
            HorizontalPanel = horizontalPanel;
            OnRefreshDisplayNeeded = new AGSEvent();
        }

        public IUIControl TreeItem { get; }

        public IButton ExpandButton { get; }

        public IPanel ParentPanel { get; }

        public IPanel VerticalPanel { get; }

        public IPanel HorizontalPanel { get; }

        public IBlockingEvent OnRefreshDisplayNeeded { get; }
    }
}
