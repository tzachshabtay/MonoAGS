using AGS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace AGS.Engine
{
    public enum FileSelection
    {
        FileOnly,
        FolderOnly,
        FileAndFolder,
    }

    public class AGSSelectFileDialog
    {
        private readonly string _startPath, _title;
        private readonly FileSelection _fileSelection;
        private readonly IGame _game;
        private readonly ITextConfig _buttonsTextConfig, _filesTextConfig;
        private readonly IGLUtils _glUtils;

        private IInventory _inventory;
        private ITextBox _fileTextBox;
        private IObject _fileGraphics, _folderGraphics;
        const float FILE_TEXT_HEIGHT = 10f;
        static float ITEM_WIDTH = 20f;
        static int _runningIndex;
        const string PATH_PROPERTY = "FilePath";

        private TaskCompletionSource<bool> _tcs;
        private string _selectedItem;
        private IDevice _device => AGSGame.Device;

        private IBorderStyle _fileIcon, _fileIconSelected, _folderIcon, _folderIconSelected;
        private Predicate<string> _fileFilter;

        private AGSSelectFileDialog(IGame game, IGLUtils glUtils, string title, FileSelection fileSelection, Predicate<string> fileFilter = null, string startPath = null)
        {
            _glUtils = glUtils;
            _game = game;
            _title = title;
            _fileSelection = fileSelection;
            _fileFilter = fileFilter;
            _startPath = startPath ?? _device.FileSystem.GetCurrentDirectory() ?? "";
            _buttonsTextConfig = new AGSTextConfig(alignment: Alignment.BottomCenter,
                autoFit: AutoFit.TextShouldFitLabel, font: game.Factory.Fonts.LoadFont(null, 10f));
            _filesTextConfig = new AGSTextConfig(alignment: Alignment.BottomCenter,
                autoFit: AutoFit.TextShouldFitLabel, font: game.Factory.Fonts.LoadFont(null, 10f),
                brush: _device.BrushLoader.LoadSolidBrush(Colors.White));
            _tcs = new TaskCompletionSource<bool>(false);
        }

        public static async Task<string> SelectFile(string title, FileSelection fileSelection, Predicate<string> fileFilter = null, string startPath = null)
        {
            var dialog = new AGSSelectFileDialog(AGSGame.Game, AGSGame.Resolver.Container.Resolve<IGLUtils>(), title, fileSelection, fileFilter, startPath);
            return await dialog.Run();
        }

        public async Task<string> Run()
        {
            IGameFactory factory = _game.Factory;
            float panelWidth = _game.Settings.VirtualResolution.Width * 3 / 4f;
            float panelHeight = _game.Settings.VirtualResolution.Height * 3 / 4f;
            const float labelHeight = 20f;
            const float textBoxHeight = 20f;
            const float buttonHeight = 20f;
            float itemHeight = panelHeight / 8f;
            ITEM_WIDTH = panelWidth / 10f;
            float itemPaddingX = panelWidth / 10f;
            float itemPaddingY = panelHeight / 12f;
            const float scrollButtonWidth = 20f;
            const float scrollButtonHeight = 20f;
            const float scrollButtonOffsetX = 5f;
            const float scrollButtonOffsetY = 5f;
            const float okButtonWidth = 50f;
            const float okButtonHeight = 20f;
            const float okButtonPaddingX = 20f;
            const float okButtonPaddingY = 20f;
            float okCancelWidth = okButtonWidth * 2 + okButtonPaddingX;
            float okButtonX = panelWidth / 2f - okCancelWidth / 2f;
            float cancelButtonX = okButtonX + okButtonWidth + okButtonPaddingX;
            float panelX = _game.Settings.VirtualResolution.Width / 2f - panelWidth / 2f;
            float panelY = _game.Settings.VirtualResolution.Height / 2f - panelHeight / 2f;
            ITextConfig textBoxConfig = new AGSTextConfig(alignment: Alignment.BottomLeft,
                autoFit: AutoFit.TextShouldCrop, font: _game.Factory.Fonts.LoadFont(null, 10f));

            IPanel panel = factory.UI.GetPanel("SelectFilePanel", panelWidth, panelHeight, panelX, panelY);
            panel.RenderLayer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            panel.SkinTags.Add(AGSSkin.DialogBoxTag);
            panel.Skin?.Apply(panel);
            panel.AddComponent<IModalWindowComponent>().GrabFocus();
            ILabel titleLabel = factory.UI.GetLabel("SelectFileTitle", _title, panelWidth, labelHeight, 0f, panelHeight - labelHeight, panel, _buttonsTextConfig);
            _fileTextBox = factory.UI.GetTextBox("SelectFileTextBox", 0f, panelHeight - labelHeight - textBoxHeight, panel, _startPath, textBoxConfig, width: panelWidth, height: textBoxHeight);

            _inventory = new AGSInventory();
            IInventoryWindow invWindow = factory.Inventory.GetInventoryWindow("SelectFileInventory", panelWidth - scrollButtonWidth - scrollButtonOffsetX * 2,
                                                                              panelHeight - labelHeight - buttonHeight - textBoxHeight - okButtonPaddingY, ITEM_WIDTH + itemPaddingX, itemHeight + itemPaddingY, 0f, okButtonPaddingY + okButtonHeight, _inventory);
            invWindow.Z = 1;
            IButton okButton = factory.UI.GetButton("SelectFileOkButton", (string)null, null, null, okButtonX, okButtonPaddingY, panel, "OK", _buttonsTextConfig, width: okButtonWidth, height: okButtonHeight);
            IButton cancelButton = factory.UI.GetButton("SelectFileCancelButton", (string)null, null, null, cancelButtonX, okButtonPaddingY, panel, "Cancel", _buttonsTextConfig, width: okButtonWidth, height: okButtonHeight);
            IButton scrollDownButton = factory.UI.GetButton("SelectFileScrollDown", (string)null, null, null, panelWidth - scrollButtonWidth - scrollButtonOffsetX, okButton.Y + okButtonHeight + scrollButtonOffsetY, panel, "", _buttonsTextConfig, width: scrollButtonWidth, height: scrollButtonHeight);
            IButton scrollUpButton = factory.UI.GetButton("SelectFileScrollUp", (string)null, null, null, panelWidth - scrollButtonWidth - scrollButtonOffsetX, panelHeight - labelHeight - textBoxHeight - scrollButtonHeight - scrollButtonOffsetY, panel, "", _buttonsTextConfig, width: scrollButtonWidth, height: scrollButtonHeight);
            invWindow.TreeNode.SetParent(panel.TreeNode);

            cancelButton.MouseClicked.Subscribe(onCancelClicked);
            okButton.MouseClicked.SubscribeToAsync(onOkClicked);

            scrollDownButton.MouseClicked.Subscribe(_ => invWindow.ScrollDown());
            scrollUpButton.MouseClicked.Subscribe(_ => invWindow.ScrollUp());

            var iconFactory = factory.Graphics.Icons;
            _fileIcon = iconFactory.GetFileIcon();
            _fileIconSelected = iconFactory.GetFileIcon(true);
            _folderIcon = iconFactory.GetFolderIcon();
            _folderIconSelected = iconFactory.GetFolderIcon(true);

            var arrowDownIcon = getIcon("ArrowDown", factory, scrollButtonWidth, scrollButtonHeight, 
                                        iconFactory.GetArrowIcon(ArrowDirection.Down), scrollDownButton.RenderLayer);
            arrowDownIcon.Pivot = new PointF();
            arrowDownIcon.Enabled = false;
            arrowDownIcon.TreeNode.SetParent(scrollDownButton.TreeNode);
            _game.State.UI.Add(arrowDownIcon);

            var arrowUpIcon = getIcon("ArrowUp", factory, scrollButtonWidth, scrollButtonHeight,
                                      iconFactory.GetArrowIcon(ArrowDirection.Up), scrollUpButton.RenderLayer);
            arrowUpIcon.Pivot = new PointF();
            arrowUpIcon.Enabled = false;
            arrowUpIcon.TreeNode.SetParent(scrollUpButton.TreeNode);
            _game.State.UI.Add(arrowUpIcon);

            _fileGraphics = getIcon("FileGraphics", factory, ITEM_WIDTH, itemHeight, _fileIcon, scrollUpButton.RenderLayer);
            _folderGraphics = getIcon("FolderGraphics", factory, ITEM_WIDTH, itemHeight, _folderIcon, scrollUpButton.RenderLayer);

            fillAllFiles(_startPath);

            _fileTextBox.OnPressingKey.Subscribe(onTextBoxKeyPressed);

            bool okGiven = await _tcs.Task;
            clearInventory();
            panel.GetComponent<IModalWindowComponent>().LoseFocus();
            destroy(panel);
            destroy(_fileGraphics);
            destroy(_folderGraphics);
            destroy(_fileTextBox);
            var result = _fileTextBox.Text;
            if (!okGiven) return null;
            return result;
        }

        private IObject getIcon(string id, IGameFactory factory, float width, float height, IBorderStyle icon, IRenderLayer renderLayer)
        {
            var obj = factory.Object.GetObject(id);
            obj.Tint = Colors.Transparent;
            obj.Image = new EmptyImage(width, height);
            obj.RenderLayer = renderLayer;
            obj.Pivot = new PointF(0.5f, 0.5f);
            obj.IgnoreScalingArea = true;
            obj.IgnoreViewport = true;
            obj.Border = icon;
            return obj;
        }

        private void destroy(IObject obj)
        {
            obj.DestroyWithChildren(_game.State);
        }

        private void onCancelClicked(MouseButtonEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            _tcs.TrySetResult(false);
        }

        private async Task onOkClicked(MouseButtonEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            var item = _selectedItem ?? _fileTextBox.Text;
            _fileTextBox.Text = item;
            if (_device.FileSystem.DirectoryExists(item))
            {
                if (_fileSelection == FileSelection.FileOnly)
                {
                    await AGSMessageBox.DisplayAsync("Please select a file.");
                    return;
                }
            }
            else if (_fileSelection == FileSelection.FolderOnly)
            {
                await AGSMessageBox.DisplayAsync("Please select a folder.");
                return;
            }
            _tcs.TrySetResult(true);
        }

        private void onTextBoxKeyPressed(TextBoxKeyPressingEventArgs args)
        {
            if (args.PressedKey != Key.Enter) return;
            args.ShouldCancel = true;
            string path = _fileTextBox.Text;
            if (_device.FileSystem.DirectoryExists(path))
                fillAllFiles(path);
            else if (_device.FileSystem.FileExists(path))
            {
                onFileSelected(path);
            }
        }

        private void onFileSelected(string path)
        {
            if (_fileSelection == FileSelection.FolderOnly) return;
            _fileTextBox.Text = path;
            _fileTextBox.IsFocused = false;
            _tcs.TrySetResult(true);
        }

        private void subscribeClicks(IObject fileObj, Action<MouseButtonEventArgs> onClick, Action<MouseButtonEventArgs> onDoubleClick)
        {
            IUIEvents uiEvents = fileObj.AddComponent<IUIEvents>();
            uiEvents.MouseClicked.Subscribe(onClick);
            uiEvents.MouseDoubleClicked.Subscribe(onDoubleClick);
            fileObj.OnDisposed(() =>
            {
                uiEvents.MouseClicked.Unsubscribe(onClick);
                uiEvents.MouseDoubleClicked.Unsubscribe(onDoubleClick);
            });
        }

        private void clearInventory()
        {
            foreach (var item in _inventory.Items)
            {
                destroy(item.Graphics);
            }
            _inventory.Items.Clear();
        }

        private void fillAllFiles(string folder)
        {
            _selectedItem = null;
            clearInventory();
            var allFiles = _device.FileSystem.GetFiles(folder).Where(f => _fileFilter == null || _fileFilter(f)).ToList();
            var allDirs = folder == "" ? _device.FileSystem.GetLogicalDrives().ToList() : _device.FileSystem.GetDirectories(folder).ToList();
            const string back = "..";
            if (folder != "") allDirs.Insert(0, back);
            List<IObject> fileItems = new List<IObject>(allFiles.Count);
            List<IObject> dirItems = new List<IObject>(allDirs.Count);
            foreach (var dir in allDirs)
            {
                var fileObj = addFileItem(dir, _folderGraphics);
                dirItems.Add(fileObj);
                var dirTmp = dir;
                Action<MouseButtonEventArgs> onDoubleClick = _ =>
                {
                    string path = (dirTmp == back) ? goBack(folder) : combine(folder, getLastName(dirTmp));
                    _fileTextBox.Text = path;
                    //todo: unsubscribe all events on current files + dirs
                    fillAllFiles(path);
                };
                Action<MouseButtonEventArgs> onClick = _ =>
                {
                    foreach (var fileItem in fileItems) fileItem.Border = _fileIcon;
                    foreach (var dirItem in dirItems) dirItem.Border = _folderIcon;
                    _selectedItem = fileObj.Properties.Strings.GetValue(PATH_PROPERTY);
                    fileObj.Border = _folderIconSelected;
                };
                subscribeClicks(fileObj, onClick, onDoubleClick);
            }
            foreach (var file in allFiles)
            {
                var fileObj = addFileItem(file, _fileGraphics);
                fileItems.Add(fileObj);
                Action<MouseButtonEventArgs> onDoubleClick = _ =>
                {
                    onFileSelected(fileObj.Properties.Strings.GetValue(PATH_PROPERTY));
                };
                Action<MouseButtonEventArgs> onClick = _ =>
                {
                    foreach (var fileItem in fileItems) fileItem.Border = _fileIcon;
                    foreach (var dirItem in dirItems) dirItem.Border = _folderIcon;
                    fileObj.Border = _fileIconSelected;
                };
                subscribeClicks(fileObj, onClick, onDoubleClick);
            }
        }        

        private IObject addFileItem(string file, IObject graphics)
        {
            graphics = clone($"FileItem_{file}_{_runningIndex++}", _game.Factory, graphics);
            graphics.Properties.Strings.SetValue(PATH_PROPERTY, file);
            ILabel fileLabel = _game.Factory.UI.GetLabel("FileItemLabel_" + file, getLastName(file), 
                ITEM_WIDTH, FILE_TEXT_HEIGHT, 0f, -10f, graphics, _filesTextConfig);
            graphics.RenderLayer = new AGSRenderLayer(AGSLayers.UI.Z - 3);
            fileLabel.RenderLayer = new AGSRenderLayer(AGSLayers.UI.Z - 4);
            var item = _game.Factory.Inventory.GetInventoryItem(graphics, null);
            _inventory.Items.Add(item);
            return graphics;
        }

        private static string goBack(string path)
        {
            int index = getFolderIndex(path);
            if (index < 0) return path;
            if (index == path.Length - 1)
            {
                return "";
            }
            path = path.Substring(0, index);
            if (getFolderIndex(path) < 0) return $"{path}\\";
            return path;
        }

        private static string combine(string folder, string childFolder)
        {
            if (string.IsNullOrEmpty(folder)) return childFolder;
            if (folder.Contains("/")) return $"{folder}/{childFolder}";
            return $"{folder}\\{childFolder}";
        }

        private static string getLastName(string path)
        {
            int index = getFolderIndex(path);
            if (index >= 0 && index < path.Length - 1) path = path.Substring(index + 1);
            return path;
        }

        private static int getFolderIndex(string path)
        {
            int index = path.LastIndexOf("\\");
            if (index < 0) index = path.LastIndexOf("/");
            return index;
        }

        private static IObject clone(string id, IGameFactory factory, IObject obj)
        {
            IObject newObj = factory.Object.GetObject(id);
            newObj.Pivot = obj.Pivot;
            newObj.Position = obj.Position;
            newObj.Tint = obj.Tint;
            newObj.Image = obj.Image;
            newObj.DisplayName = obj.DisplayName;
            newObj.RenderLayer = obj.RenderLayer;
            newObj.IgnoreViewport = obj.IgnoreViewport;
            newObj.IgnoreScalingArea = obj.IgnoreScalingArea;
            newObj.Border = obj.Border;
            if (obj.Animation != null) newObj.StartAnimation(obj.Animation.Clone());
            newObj.BaseSize = new SizeF(obj.Width / obj.ScaleX, obj.Height / obj.ScaleY);
            newObj.Scale = obj.Scale;
            return newObj;
        }
    }
}