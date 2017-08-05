using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeNodeViewProvider : ITreeNodeViewProvider
    {
        private readonly IGameFactory _factory;

        public AGSTreeNodeViewProvider(IGameFactory factory)
        {
            _factory = factory;
        }

        public void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, 
                                         bool isCollapsed, bool isHovered, bool isSelected)
        {
            nodeView.TreeItem.TextConfig = isHovered ? item.HoverTextConfig : item.IdleTextConfig;
            nodeView.TreeItem.Text = item.Text;
            nodeView.TreeItem.Tint = isSelected ? Colors.DarkSlateBlue : Colors.Transparent;
            var expandButton = nodeView.ExpandButton;
            if (expandButton != null)
            {
                expandButton.TextConfig = nodeView.TreeItem.TextConfig;
                expandButton.Text = isCollapsed ? "+" : "-";
                expandButton.TextVisible = item.TreeNode.ChildrenCount > 0;
                expandButton.Enabled = expandButton.TextVisible;
            }
        }

        public ITreeNodeView CreateNode(ITreeStringNode item, IRenderLayer layer)
        {
            var buttonWidth = 20f;
            var buttonHeight = 60f;
            IAnimation idle = new AGSSingleFrameAnimation(new EmptyImage(buttonWidth, buttonHeight), _factory.Graphics);
            idle.Sprite.Tint = Colors.Black;
            IAnimation hovered = new AGSSingleFrameAnimation(new EmptyImage(buttonWidth, buttonHeight), _factory.Graphics);
            hovered.Sprite.Tint = Colors.Yellow;
            IAnimation pushed = new AGSSingleFrameAnimation(new EmptyImage(buttonWidth, buttonHeight), _factory.Graphics);
            pushed.Sprite.Tint = Colors.DarkSlateBlue;
            var itemTextId = item.Text ?? "" + "_" + Guid.NewGuid();
            var parentPanel = _factory.UI.GetPanel("TreeNodeParentPanel_" + itemTextId, 0f, 0f, 0f, 0f, addToUi: false);
            var horizontalPanel = _factory.UI.GetPanel("TreeNodeHorizontalPanel_" + itemTextId, 0f, 0f, 0f, 0f, parentPanel, false);
            var expandButton = _factory.UI.GetButton("ExpandButton_" + itemTextId, idle, hovered, pushed, 0f, 0f, horizontalPanel, addToUi: false);
            var label = _factory.UI.GetLabel("TreeNodeLabel_" + itemTextId, item.Text, 0f, 0f, buttonWidth, 0f, horizontalPanel,
                                             new AGSTextConfig(paddingTop: 0f, paddingBottom: 0f, autoFit: AutoFit.LabelShouldFitText), addToUi: false);
            var verticalPanel = _factory.UI.GetPanel("TreeNodeVerticalPanel_" + itemTextId, 0f, 0f, 0f, 0f, parentPanel, false);
            horizontalPanel.RenderLayer = layer;
            verticalPanel.RenderLayer = layer;
            parentPanel.RenderLayer = layer;
            expandButton.RenderLayer = layer;
            label.RenderLayer = layer;
            horizontalPanel.Tint = Colors.Transparent;
            parentPanel.Tint = Colors.Transparent;
            verticalPanel.Tint = Colors.Transparent;
            expandButton.Tint = Colors.Transparent;
            label.Tint = Colors.Transparent;
            label.Enabled = true;
            expandButton.PixelPerfect(false);
            horizontalPanel.AddComponent<ISizeWithChildrenComponent>();
            var layout = horizontalPanel.AddComponent<IStackLayoutComponent>();
            layout.RelativeSpacing = 1f;
            layout.Direction = LayoutDirection.Horizontal;
            layout.StartLayout();

            var nodeView = new AGSTreeNodeView(label, expandButton, parentPanel, verticalPanel, horizontalPanel);
            return nodeView;
        }
    }
}
