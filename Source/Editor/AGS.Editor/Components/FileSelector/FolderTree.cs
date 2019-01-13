using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    [RequiredComponent(typeof(ITreeViewComponent))]
    public class FolderTree: AGSComponent
    {
        private ITreeViewComponent _treeView;
        private string _defaultFolder;
        private readonly IDevice _device;
        private Folder _root;
        private readonly IGameFactory _factory;

        public FolderTree(IDevice device, IGameFactory factory)
        {
            _device = device;
            _factory = factory;
        }

        public string DefaultFolder
        {
            get => _defaultFolder;
            set
            {
                _defaultFolder = value;
                buildTreeModel(); 
            }
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<ITreeViewComponent>(c => { _treeView = c; configureTreeUI(); refreshTreeUI(); }, _ => _treeView = null);
        }

        private async void buildTreeModel()
        {
            _root = await buildTreeModel(DefaultFolder);
            refreshTreeUI();
        }

        private async Task<Folder> buildTreeModel(string path)
        {
            var dirs = (await Task.Run(() => _device.FileSystem.GetDirectories(path))).ToArray();
            List<Folder> folders = new List<Folder>(dirs.Length);
            foreach (var dir in dirs)
            {
                folders.Add(await buildTreeModel(dir));
            }
            return new Folder(Path.GetFileName(path), path, folders.ToArray());
        }

        private void configureTreeUI()
        {
            var tree = _treeView;
            if (tree == null) return;
            tree.NodeViewProvider = new FolderNodeViewProvider(tree.NodeViewProvider, _factory);
        }

        private void refreshTreeUI()
        {
            var root = _root;
            var tree = _treeView;
            if (root == null || tree == null) return;

            tree.Tree = _root.ToNode(GameViewColors.ButtonTextConfig.Font);
            tree.Expand(tree.Tree);
        }

        private class Folder
        {
            public Folder(string name, string fullPath, Folder[] folders)
            {
                Name = name;
                FullPath = fullPath;
                Folders = folders;
            }

            public string FullPath { get; }
            public string Name { get; }
            public Folder[] Folders { get; }

            public ITreeStringNode ToNode(IFont font)
            {
                AGSTreeStringNode node = new AGSTreeStringNode(Name, font);
                if (Folders.Length > 0)
                {
                    node.TreeNode.AddChildren(Folders.Select(f => f.ToNode(font)).ToList());
                }
                return node;
            }
        }
    }
}