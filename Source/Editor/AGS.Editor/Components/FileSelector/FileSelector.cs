using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class FileSelector : AGSComponent
    {
        private readonly IGameFactory _factory;
        private ISplitPanelComponent _splitter;
        private FolderTree _folderTree;
        private FilesView _filesView;
        const float _gutterSize = 15f;

        public FileSelector(IGameFactory factory)
        {
            _factory = factory;
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<ISplitPanelComponent>(c => { _splitter = c; configureSplitter(); }, c => _splitter = null);
        }

        public override void Dispose()
        {
            base.Dispose();
            _folderTree.OnFolderSelected.Unsubscribe(onFolderSelected);
        }

        private void configureSplitter()
        {
            _splitter.IsHorizontal = true;

            var leftScrollingPanel = _factory.UI.GetPanel("FolderTree", 300f, 600f, 100f, 100f);
            leftScrollingPanel.Tint = GameViewColors.Panel;
            leftScrollingPanel.Border = _factory.Graphics.Borders.SolidColor(GameViewColors.Border, 3f);
            leftScrollingPanel.RenderLayer = new AGSRenderLayer(AGSLayers.UI.Z - 1000);
            var leftPanel = _factory.UI.CreateScrollingPanel(leftScrollingPanel);
            var tree = leftPanel.AddComponent<ITreeViewComponent>();
            tree.LeftPadding = 10f;
            tree.TopPadding = 30f;
            _folderTree = leftPanel.AddComponent<FolderTree>();
            _folderTree.DefaultFolder = "../../../../../Demo";

            _splitter.SplitLineOffset = (15f, 15f); //Offsetting for the scrollbar
            _splitter.TopPanel = leftPanel;

            var rightScrollingPanel = _factory.UI.GetPanel("FilesView", 800f, 600f, 400f, 100f);
            rightScrollingPanel.Tint = GameViewColors.Panel;
            rightScrollingPanel.Border = _factory.Graphics.Borders.SolidColor(GameViewColors.Border, 3f);
            rightScrollingPanel.RenderLayer = new AGSRenderLayer(AGSLayers.UI.Z - 1000);
            var rightPanel = _factory.UI.CreateScrollingPanel(rightScrollingPanel);
            var inv = rightPanel.AddComponent<IInventoryWindowComponent>();
            inv.Inventory = new AGSInventory();
            inv.ItemSize = (120f, 120f);
            inv.PaddingLeft = 20f;
            inv.PaddingRight = 20f;
            _filesView = rightPanel.AddComponent<FilesView>();
            _filesView.Folder = "../../../../../Demo/DemoQuest/Assets/Sounds";

            _folderTree.OnFolderSelected.Subscribe(folder => _filesView.Folder = folder);

            _splitter.BottomPanel = rightPanel;
        }

        private void onFolderSelected(string folder)
        {
            _filesView.Folder = folder;
        }
    }
}
