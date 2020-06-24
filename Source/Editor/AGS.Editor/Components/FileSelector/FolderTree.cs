using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            OnFolderSelected = new AGSEvent<string>();
        }

        public IBlockingEvent<string> OnFolderSelected { get; }

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

        public override void Dispose()
        {
            base.Dispose();
            _treeView?.OnNodeExpanded.Unsubscribe(onNodeExpanded);
            _treeView?.OnNodeSelected.Unsubscribe(onNodeSelected);
        }

        private async void buildTreeModel()
        {
            _root = await buildTreeModel(DefaultFolder);
            refreshTreeUI();
        }

        private async Task<Folder> buildTreeModel(string path, int levels = 1)
        {
            var dirs = (await Task.Run(() => _device.FileSystem.GetDirectories(path))).ToArray();
            List<Folder> folders = new List<Folder>(dirs.Length);
            foreach (var dir in dirs)
            {
                if (levels == 0)
                {
                    folders.Add(new Folder(Path.GetFileName(dir), dir, Array.Empty<Folder>()));
                }
                else
                {
                    folders.Add(await buildTreeModel(dir, levels - 1));
                }
            }
            return new Folder(Path.GetFileName(path), path, folders.ToArray());
        }

        private void configureTreeUI()
        {
            var tree = _treeView;
            if (tree == null) return;
            tree.NodeViewProvider = new FolderNodeViewProvider(tree.NodeViewProvider, _factory);
            tree.OnNodeSelected.Subscribe(onNodeSelected);
        }

        private void onNodeSelected(NodeEventArgs args)
        {
            if (args.Node is FolderNode folder)
            {
                OnFolderSelected.Invoke(folder.FullPath);
            }
        }

        private void refreshTreeUI()
        {
            var root = _root;
            var tree = _treeView;
            if (root == null || tree == null) return;

            tree.Tree = _root.ToNode(GameViewColors.ButtonTextConfig.Font);
            tree.Expand(tree.Tree);

            tree.OnNodeExpanded.Subscribe(onNodeExpanded);
        }

        private async void onNodeExpanded(NodeEventArgs args)
        {
            var tree = _treeView;
            if (tree == null) return;
            if (args.Node is FolderNode node)
            {
                var parent = args.Node.TreeNode.Parent;
                if (parent == null) return;
                var index = parent.TreeNode.Children.IndexOf(args.Node.TreeNode.Node);
                var path = node.FullPath;
                var folder = await buildTreeModel(path);
                var newNode = folder.ToNode(GameViewColors.ButtonTextConfig.Font);
                if (node.Equals(newNode))
                {
                    return;
                }
                args.Node.TreeNode.SetParent(null);
                parent.TreeNode.InsertChild(index, newNode);
                tree.RefreshLayout();
                tree.OnNodeExpanded.Unsubscribe(onNodeExpanded);
                tree.Expand(newNode);
                tree.OnNodeExpanded.Subscribe(onNodeExpanded);
            }
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

            public FolderNode ToNode(IFont font)
            {
                FolderNode node = new FolderNode(Name, font, FullPath);
                if (Folders.Length > 0)
                {
                    node.TreeNode.AddChildren(Folders.Select(f => f.ToNode(font)).Cast<ITreeStringNode>().ToList());
                }
                return node;
            }
        }

        private class FolderNode : AGSTreeStringNode
        {
            public FolderNode(string text, IFont font, string path) : base(text, font)
            {
                FullPath = path;
            }

            public string FullPath { get; }

            public override bool Equals(object obj) => obj is FolderNode other && Equals(other);

            public bool Equals(FolderNode node)
            {
                if (node == null) return false;
                if (FullPath != node.FullPath) return false;
                if (TreeNode.ChildrenCount != node.TreeNode.ChildrenCount) return false;
                for (int i = 0; i < TreeNode.ChildrenCount; i++)
                {
                    var myChild = TreeNode.Children[i];
                    var theirChild = node.TreeNode.Children[i];
                    if (!myChild.Equals(theirChild)) return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return 2018552787 + EqualityComparer<string>.Default.GetHashCode(FullPath);
            }
        }
    }
}