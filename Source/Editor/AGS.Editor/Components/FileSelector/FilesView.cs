using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    [RequiredComponent(typeof(IInventoryWindowComponent))]
    public class FilesView : AGSComponent
    {
        private IInventoryWindowComponent _invWindow;
        private string _folder;
        private ITextConfig _textConfig, _hoverTextConfig;
        private ButtonAnimation _labelIdle, _labelHover, _iconIdle, _iconHover;
        private readonly IFileSystem _files;
        private readonly IGameFactory _factory;

        private string[] imageSuffixes = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
        private string[] audioSuffixes = { ".wav", ".ogg", ".flac" };

        public FilesView(IFileSystem files, IGameFactory factory)
        {
            _files = files;
            _factory = factory;
            configureTextConfig();
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IInventoryWindowComponent>(c => { _invWindow = c; configureInvWindow(); refreshInvWindow(); }, _ => _invWindow = null);
        }

        public string Folder 
        {
            get => _folder;
            set
            {
                _folder = value;
                refreshInvWindow();
            }
        }

        private void configureInvWindow()
        {
        }

        private void configureTextConfig()
        {
            var textConfig = AGSTextConfig.Clone(GameViewColors.TextboxTextConfig);
            textConfig.Alignment = Alignment.MiddleCenter;
            textConfig.Font = _factory.Fonts.LoadFont(textConfig.Font.FontFamily, 8f, FontStyle.Regular);
            textConfig.AutoFit = AutoFit.TextShouldWrapAndLabelShouldFitHeight;
            textConfig.PaddingRight = 10f;
            _textConfig = textConfig;
            _hoverTextConfig = AGSTextConfig.ChangeColor(textConfig, GameViewColors.HoveredText, Colors.White, 0f);

            _labelIdle = new ButtonAnimation(null, _textConfig, null);
            _labelHover = new ButtonAnimation(null, _hoverTextConfig, null);
            _iconIdle = new ButtonAnimation(null, FontIcons.LargeIconConfig, null);
            _iconHover = new ButtonAnimation(null, FontIcons.LargeIconConfigHovered, null);
        }

        private async void refreshInvWindow()
        {
            var folder = _folder;
            var invWindow = _invWindow;
            if (folder == null || invWindow == null) return;
            var files = await Task.Run(() => _files.GetFiles(folder));
            foreach (var item in invWindow.Inventory.Items)
            {
                item.Graphics?.DestroyWithChildren();
            }
            invWindow.Inventory.Items.Clear();
            foreach (var file in files)
            {
                IObject graphics = await getGraphics(file);
                var item = _factory.Inventory.GetInventoryItem(graphics, graphics);
                invWindow.Inventory.Items.Add(item);
            }
        }

        private Task<IObject> getGraphics(string file)
        {
            if (imageSuffixes.Any(file.EndsWith))
            {
                return getImage(file); 
            }
            if (audioSuffixes.Any(file.EndsWith))
            {
                return getAudio(file);
            }
            return getFile(file);
        }

        private async Task<IObject> getImage(string file)
        {
            const float padding = 50f;
            file = Path.GetFullPath(file);
            var size = _invWindow.ItemSize;
            var image = await _factory.Graphics.LoadImageAsync(file);
            var obj = _factory.UI.GetPanel($"File_{file}", image, 0f, 0f, Entity as IObject);
            obj.Enabled = true;
            obj.ClickThrough = false;
            obj.IsPixelPerfect = false;
            obj.CurrentSprite.Pivot = (0.5f, 1f);
            obj.CurrentSprite.ScaleTo(size.Width - padding, size.Height - padding);
            obj.CurrentSprite.Y = -padding / 2f;
            var label = _factory.UI.GetLabel($"FileLabel_{file}", getFileDisplayName(file),
                    size.Width, 1f, -size.Width / 2f, -size.Height + 5f, obj, _textConfig);
            label.Enabled = true;
            Action onHover = () => obj.CurrentSprite.ScaleTo(size.Width - padding/2f, size.Height - padding/2f);
            Action onLeave = () => obj.CurrentSprite.ScaleTo(size.Width - padding, size.Height - padding);
            HoverEffect.Add(onHover, onLeave, (obj, null, null), (label, _labelIdle, _labelHover));
            return obj;
        }

        private async Task<IObject> getAudio(string file)
        {
            await Task.Yield();
            var size = _invWindow.ItemSize;
            var icon = _factory.UI.GetLabel($"File_{file}", "", size.Width, size.Height, 0f, 0f, Entity as IObject,
                AGSTextConfig.Clone(FontIcons.LargeIconConfig));
            icon.Text = FontIcons.AudioFile;
            icon.IsPixelPerfect = false;
            icon.Enabled = true;
            icon.Pivot = (0.5f, 1f);
            var label = _factory.UI.GetLabel($"FileLabel_{file}", getFileDisplayName(file),
                    size.Width, 1f, size.Width / 2f, 20f, icon, _textConfig);
            label.Pivot = (0.5f, 1f);

            var idleConfig = AGSTextConfig.ChangeColor(FontIcons.ButtonConfig, GameViewColors.Button, Colors.White, 0f);
            var idle = new ButtonAnimation(null, idleConfig, Colors.Transparent);
            var hover = new ButtonAnimation(null, AGSTextConfig.ChangeColor(FontIcons.ButtonConfig, Colors.DarkOrange, Colors.White, 0f), null);
            var pushed = new ButtonAnimation(null, AGSTextConfig.ChangeColor(FontIcons.ButtonConfig, GameViewColors.PushedButton, Colors.White, 0f), null);
            var playButton = _factory.UI.GetButton($"AudioButtin_{file}", idle, hover, pushed, 
                size.Width / 2f, size.Height / 2f - 10f, icon, width: size.Width / 6f, height: size.Height / 6f);
            playButton.Text = FontIcons.Play;
            playButton.Pivot = new PointF(0.5f, 0.5f);
            playButton.Visible = false;
            IAudioClip clip = null;
            ISound sound = null;
            TaskCompletionSource<object> tcs = null;
            Action onPlayPause = async () =>
            {
                playButton.Text = playButton.Text == FontIcons.Play ? FontIcons.Pause : FontIcons.Play;
                if (tcs == null)
                {
                    tcs = new TaskCompletionSource<object>();
                    clip = await _factory.Sound.LoadAudioClipAsync(Path.GetFullPath(file), $"tmp_{file}");
                    tcs.TrySetResult(null);
                }
                else await tcs.Task;
                if (playButton.Text == FontIcons.Play)
                {
                    sound?.Pause();
                }
                else
                {
                    if (sound == null) sound = clip.Play(true);
                    else sound.Resume();
                }

            };
            playButton.MouseClicked.Subscribe(onPlayPause);

            Action onHover = () => { icon.Text = FontIcons.File; playButton.Visible = true; };
            Action onLeave = () => { if (tcs != null) return; icon.Text = FontIcons.AudioFile; playButton.Visible = false; };
            HoverEffect.Add(onHover, onLeave, (icon, _iconIdle, _iconHover), (label, _labelIdle, _labelHover), (playButton, null, null));
            return icon;
        }

        private async Task<IObject> getFile(string file)
        {
            await Task.Yield();
            var size = _invWindow.ItemSize;
            var icon = _factory.UI.GetLabel($"File_{file}", "", size.Width, size.Height, 0f, 0f, Entity as IObject,
                FontIcons.LargeIconConfig);
            icon.Text = FontIcons.File;
            icon.IsPixelPerfect = false;
            icon.Enabled = true;
            icon.Pivot = (0.5f, 1f);
            var label = _factory.UI.GetLabel($"FileLabel_{file}", getFileDisplayName(file),
                    size.Width, 1f, size.Width / 2f, 20f, icon, _textConfig);
            label.Pivot = (0.5f, 1f);
            HoverEffect.Add((icon, _iconIdle, _iconHover), (label, _labelIdle, _labelHover));
            return icon;
        }

        private string getFileDisplayName(string file)
        {
            file = Path.GetFileName(file);
            StringBuilder sb = new StringBuilder(file.Length * 2);
            int index = 0;
            const int lineLength = 16;
            const int keepingSuffix = 8;
            const int trimStart = lineLength + keepingSuffix + 1;
            foreach (var c in file)
            {
                index++;
                if (index == lineLength + 1)
                {
                   sb.Append(' '); //Adding spaces as System.Drawing only seems to support word-wrap
                }
                if (index == trimStart && file.Length > lineLength)
                {
                    sb.Append("...");
                    continue;
                }
                if (index > trimStart && file.Length > lineLength && index < file.Length - keepingSuffix)
                {
                    continue; 
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
