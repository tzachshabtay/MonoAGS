using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeNodeView : ITreeNodeView
    {
        public AGSTreeNodeView(ILabel treeItem, IButton expandButton, IPanel parentPanel, IPanel verticalPanel)
        {
            TreeItem = treeItem;
            ExpandButton = expandButton;
            ParentPanel = parentPanel;
            VerticalPanel = verticalPanel;
        }

        public ILabel TreeItem { get; private set; }

        public IButton ExpandButton { get; private set; }

        public IPanel ParentPanel { get; private set; }

        public IPanel VerticalPanel { get; private set; }
    }
}
