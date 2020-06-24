using System;
using System.Linq;
using System.Threading;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class FolderNodeViewProvider: ITreeNodeViewProvider
    {
        private readonly ITreeNodeViewProvider _inner;
        private readonly IGameFactory _factory;
        private const string FOLDER_HOVERED = "FolderHovered";

        private static int _nextNodeId;

        public FolderNodeViewProvider(ITreeNodeViewProvider inner, IGameFactory factory)
        {
            _inner = inner;
            _factory = factory;
        }

        public void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, bool isCollapsed, bool isHovered, bool isSelected)
        {
            isHovered |= item.Properties.Bools.GetValue(FOLDER_HOVERED);
            _inner.BeforeDisplayingNode(item, nodeView, isCollapsed, isHovered, isSelected);
            var folderIcon = (ILabel) nodeView.ExpandButton.TreeNode.Children.First(c => c.ID.StartsWith("FolderIcon", StringComparison.InvariantCulture));
            folderIcon.Text = isSelected ? FontIcons.FolderOpen : FontIcons.Folder;
            folderIcon.TextConfig.Brush = isHovered ? GameViewColors.HoveredTextBrush : GameViewColors.TextBrush;
        }

        public ITreeNodeView CreateNode(ITreeStringNode node, IRenderLayer layer, IObject parent)
        {
            var view = _inner.CreateNode(node, layer, parent);
            int nodeId = Interlocked.Increment(ref _nextNodeId);
            var folderIcon = _factory.UI.GetLabel($"FolderIcon_{node.Text}_{nodeId}", "", 25f, 25f, 15f, 0f, view.ExpandButton, AGSTextConfig.Clone(FontIcons.IconConfig));
            folderIcon.Text = FontIcons.Folder;
            folderIcon.IsPixelPerfect = false;
            folderIcon.Enabled = true;
            folderIcon.MouseEnter.Subscribe(() =>
            {
                node.Properties.Bools.SetValue(FOLDER_HOVERED, true);
                view.OnRefreshDisplayNeeded.Invoke();
            });
            folderIcon.MouseLeave.Subscribe(() =>
            {
                node.Properties.Bools.SetValue(FOLDER_HOVERED, false);
                view.OnRefreshDisplayNeeded.Invoke();
            });
            folderIcon.MouseClicked.Subscribe(async (args) => await view.TreeItem.MouseClicked.InvokeAsync(args));
            return view;
        }
    }
}
