using System.ComponentModel;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeNodeViewProvider : ITreeNodeViewProvider
    {
        private readonly IGameFactory _factory;
        private static int _nextNodeId;

        public AGSTreeNodeViewProvider(IGameFactory factory)
        {
            _factory = factory;
        }

        public void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, 
                                         bool isCollapsed, bool isHovered, bool isSelected)
        {
            var textConfig = isHovered ? item.HoverTextConfig : item.IdleTextConfig;
            var label = nodeView.TreeItem as ITextComponent;
            if (label != null)
            {
                label.TextConfig = textConfig;
                label.Text = item.Text;
                label.TextBackgroundVisible = isSelected;
            }
            nodeView.TreeItem.Tint = isSelected ? Colors.DarkSlateBlue : Colors.Transparent;
            var expandButton = nodeView.ExpandButton;
            if (expandButton != null)
            {
                var expandTextConfig = textConfig;
                if (item.TreeNode.ChildrenCount == 0) expandTextConfig = AGSTextConfig.ChangeColor(textConfig, textConfig.Brush.Color.WithAlpha(0), textConfig.OutlineBrush.Color, textConfig.OutlineWidth);
                expandButton.TextConfig = expandTextConfig;
                expandButton.Text = isCollapsed ? "+" : "-";
                expandButton.Enabled = expandButton.TextVisible;
            }
        }

        public ITreeNodeView CreateNode(ITreeStringNode item, IRenderLayer layer, IObject parent)
        {
            var buttonWidth = 20f;
            var buttonHeight = 60f;
            var idle = new ButtonAnimation(new EmptyImage(buttonWidth, buttonHeight), Colors.Black);
            var hovered = new ButtonAnimation(new EmptyImage(buttonWidth, buttonHeight), Colors.Yellow);
            var pushed = new ButtonAnimation(new EmptyImage(buttonWidth, buttonHeight), Colors.DarkSlateBlue);
            int nodeId = Interlocked.Increment(ref _nextNodeId);
            var itemTextId = (item.Text ?? "") + "_" + nodeId;
            var parentPanel = _factory.UI.GetPanel("TreeNodeParentPanel_" + itemTextId, 0f, 0f, 0f, 0f, parent, false);
            var horizontalPanel = _factory.UI.GetPanel("TreeNodeHorizontalPanel_" + itemTextId, 0f, 0f, 0f, 0f, parentPanel, false);
            var expandButton = _factory.UI.GetButton("ExpandButton_" + itemTextId, idle, hovered, pushed, 0f, 0f, horizontalPanel, addToUi: false);
            var label = _factory.UI.GetLabel("TreeNodeLabel_" + itemTextId, item.Text, 0f, 0f, buttonWidth, 0f, horizontalPanel,
                                             _factory.Fonts.GetTextConfig(paddingTop: 0f, paddingBottom: 0f, autoFit: AutoFit.LabelShouldFitText), addToUi: false);
            var verticalPanel = _factory.UI.GetPanel("TreeNodeVerticalPanel_" + itemTextId, 0f, 0f, 0f, 0f, parentPanel, false);
            horizontalPanel.RenderLayer = layer;
            verticalPanel.RenderLayer = layer;
            parentPanel.RenderLayer = layer;
            expandButton.RenderLayer = layer;
            label.RenderLayer = layer;
            expandButton.Z = label.Z - 1;
            parentPanel.Pivot = (0f, 1f);
            horizontalPanel.Tint = Colors.Transparent;
            parentPanel.Tint = Colors.Transparent;
            verticalPanel.Tint = Colors.Transparent;
            expandButton.Tint = Colors.Transparent;
            expandButton.TextBackgroundVisible = false;
            label.Tint = Colors.Transparent;
            label.TextBackgroundVisible = false;
            label.Enabled = true;
            expandButton.IsPixelPerfect = false;
            horizontalPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            var layout = horizontalPanel.AddComponent<IStackLayoutComponent>();
            layout.RelativeSpacing = 1f;
            layout.Direction = LayoutDirection.Horizontal;
            layout.StartLayout();
            layout.ForceRefreshLayout();

            PropertyChangedEventHandler onPropertyChanged = (sender, e) =>
            {
                if (e.PropertyName != nameof(ITreeStringNode.Text))
                    return;
                label.Text = item.Text;
            };
            item.PropertyChanged += onPropertyChanged;
            label.OnDisposed(() => item.PropertyChanged -= onPropertyChanged);

            var nodeView = new AGSTreeNodeView(label, expandButton, parentPanel, verticalPanel, horizontalPanel);
            return nodeView;
        }
    }
}
