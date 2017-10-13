using System.Threading.Tasks;
using AGS.API;
using Autofac;
using System;

namespace AGS.Engine
{
    public class AGSUIFactory : IUIFactory
    {
        private Resolver _resolver;
        private IGameState _gameState;
        private IGraphicsFactory _graphics;
        private IObjectFactory _object;

        public AGSUIFactory(Resolver resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj)
        {
            _resolver = resolver;
            _gameState = gameState;
            _graphics = graphics;
            _object = obj;
        }

        public IPanel GetPanel(string id, IImage image, float x, float y, IObject parent = null, bool addToUi = true)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            TypedParameter imageParameter = new TypedParameter(typeof(IImage), image);
            IPanel panel = _resolver.Container.Resolve<IPanel>(idParam, imageParameter);
            panel.X = x;
            panel.Y = y;
            panel.ClickThrough = true;
            setParent(panel, parent);
            if (addToUi)
                _gameState.UI.Add(panel);
            return panel;
        }

        public IPanel GetPanel(string id, float width, float height, float x, float y, IObject parent = null, bool addToUi = true)
        {
            EmptyImage image = new EmptyImage(width, height);
            return GetPanel(id, image, x, y, parent, addToUi);
        }

        public IPanel GetPanel(string id, string imagePath, float x, float y, IObject parent = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
        {
            IImage image = _graphics.LoadImage(imagePath, loadConfig);
            return GetPanel(id, image, x, y, parent, addToUi);
        }

        public async Task<IPanel> GetPanelAsync(string id, string imagePath, float x, float y, IObject parent = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
        {
            IImage image = await _graphics.LoadImageAsync(imagePath, loadConfig);
            return GetPanel(id, image, x, y, parent, addToUi);
        }

        public void CreateScrollingPanel(IPanel panel)
        {
			panel.AddComponent<ICropChildrenComponent>();
			panel.AddComponent<IBoundingBoxWithChildrenComponent>();
			IScrollingComponent scroll = panel.AddComponent<IScrollingComponent>();
            var horizSlider = GetSlider(string.Format("{0}_HorizontalSlider", panel.ID), null, null, 0f, 0f, 0f, panel);
			horizSlider.X = -panel.Width * panel.Anchor.X + 20f;
            horizSlider.Y = 20f;
			horizSlider.HandleGraphics.Anchor = new PointF(0f, 0.5f);
            horizSlider.Direction = SliderDirection.LeftToRight;
			horizSlider.Graphics.Anchor = new PointF(0f, 0.5f);
			horizSlider.Graphics.Image = new EmptyImage(panel.Width - 40f, 10f);
			horizSlider.Graphics.Border = AGSBorders.SolidColor(Colors.DarkGray, 3f, true);
			horizSlider.HandleGraphics.Border = AGSBorders.SolidColor(Colors.White, 2f, true);
            HoverEffect.Add(horizSlider.Graphics, Colors.Gray, Colors.LightGray);
			HoverEffect.Add(horizSlider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

			scroll.HorizontalScrollBar = horizSlider;

            var verSlider = GetSlider(string.Format("{0}_VerticalSlider", panel.ID), null, null, 0f, 0f, 0f, panel);
			verSlider.X = panel.Width - 20f;
            verSlider.Y = 40f;
			verSlider.HandleGraphics.Anchor = new PointF(0.5f, 0f);
            verSlider.Direction = SliderDirection.TopToBottom;
			verSlider.Graphics.Anchor = new PointF(0.5f, 0f);
			verSlider.Graphics.Image = new EmptyImage(10f, panel.Height - 80f);
			verSlider.Graphics.Border = AGSBorders.SolidColor(Colors.DarkGray, 3f, true);
			verSlider.HandleGraphics.Border = AGSBorders.SolidColor(Colors.White, 2f, true);
			HoverEffect.Add(verSlider.Graphics, Colors.Gray, Colors.LightGray);
			HoverEffect.Add(verSlider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

			scroll.VerticalScrollBar = verSlider;

            panel.OnScaleChanged.Subscribe(() => 
            {
                horizSlider.Graphics.Image = new EmptyImage(panel.Width - 40f, 10f);
                verSlider.Graphics.Image = new EmptyImage(10f, panel.Height - 80f);
                horizSlider.X = -panel.Width * panel.Anchor.X + 20f;
                verSlider.X = panel.Width - 20f;
            });
        }

        public ILabel GetLabel(string id, string text, float width, float height, float x, float y, IObject parent = null, ITextConfig config = null, bool addToUi = true)
        {
            SizeF baseSize = new SizeF(width, height);
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ILabel label = _resolver.Container.Resolve<ILabel>(idParam);
            label.LabelRenderSize = baseSize;
            label.Text = text;
            label.X = x;
            label.Y = y;
            label.Tint = Colors.Transparent;
            label.TextConfig = config ?? new AGSTextConfig();
            setParent(label, parent);
            if (addToUi)
                _gameState.UI.Add(label);
            return label;
        }

        public IButton GetButton(string id, ButtonAnimation idle, ButtonAnimation hovered, ButtonAnimation pushed,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            bool pixelArtButton = idle != null && idle.Animation != null && idle.Animation.Frames.Count > 0;
            if (width == -1f && pixelArtButton)
            {
                width = idle.Animation.Frames[0].Sprite.Width;
            }
            if (height == -1f && pixelArtButton)
            {
                height = idle.Animation.Frames[0].Sprite.Height;
            }
            idle = validateAnimation(id, idle, width, height);
            hovered = validateAnimation(id, hovered, width, height);
            pushed = validateAnimation(id, pushed, width, height);

            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IButton button = _resolver.Container.Resolve<IButton>(idParam);
            button.LabelRenderSize = new SizeF(width, height);
            button.IdleAnimation = idle;
            button.HoverAnimation = hovered;
            button.PushedAnimation = pushed;

            button.Tint = pixelArtButton ? Colors.White : Colors.Transparent;
            button.X = x;
            button.Y = y;
            button.TextConfig = config;
            button.Text = text;
            setParent(button, parent);

            if (button.Skin != null) button.Skin.Apply(button);
            button.IdleAnimation.StartAnimation(button, button, button);

            if (addToUi)
                _gameState.UI.Add(button);

            return button;
        }

        public IButton GetButton(string id, IAnimation idle, IAnimation hovered, IAnimation pushed,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            return GetButton(id, new ButtonAnimation(idle), new ButtonAnimation(hovered), new ButtonAnimation(pushed),
                             x, y, parent, text, config, addToUi, width, height);
        }

        public IButton GetButton(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            IAnimation idle = _graphics.LoadAnimationFromFiles(files: new[] { idleImagePath });
            IAnimation hovered = _graphics.LoadAnimationFromFiles(files: new[] { hoveredImagePath });
            IAnimation pushed = _graphics.LoadAnimationFromFiles(files: new[] { pushedImagePath });

            return GetButton(id, idle, hovered, pushed, x, y, parent, text, config, addToUi, width, height);
        }

        public async Task<IButton> GetButtonAsync(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            IAnimation idle = await _graphics.LoadAnimationFromFilesAsync(files: new[] { idleImagePath });
            IAnimation hovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { hoveredImagePath });
            IAnimation pushed = await _graphics.LoadAnimationFromFilesAsync(files: new[] { pushedImagePath });

            return GetButton(id, idle, hovered, pushed, x, y, parent, text, config, addToUi, width, height);
        }

        public ITextBox GetTextBox(string id, float x, float y, IObject parent = null, string text = "", ITextConfig config = null,
            bool addToUi = true, float width = -1F, float height = -1F)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ITextBox textbox = _resolver.Container.Resolve<ITextBox>(idParam);
            textbox.LabelRenderSize = new SizeF(width, height);
            textbox.X = x;
            textbox.Y = y;
            if (width < 0f && config == null)
            {
                config = new AGSTextConfig(autoFit: AutoFit.TextShouldCrop);
            }
            textbox.TextConfig = config;
            textbox.Text = text;
            setParent(textbox, parent);

            if (addToUi)
                _gameState.UI.Add(textbox);

            return textbox;
        }

        public ICheckBox GetCheckBox(string id, ButtonAnimation notChecked, ButtonAnimation notCheckedHovered, ButtonAnimation @checked, ButtonAnimation checkedHovered,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            bool pixelArtButton = notChecked != null && notChecked.Animation != null && notChecked.Animation.Frames.Count > 0;
            if (width == -1f && pixelArtButton)
            {
                width = notChecked.Animation.Frames[0].Sprite.Width;
            }
            if (height == -1f && pixelArtButton)
            {
                height = notChecked.Animation.Frames[0].Sprite.Height;
            }
            notChecked = validateAnimation(id, notChecked, width, height);
            notCheckedHovered = validateAnimation(id, notCheckedHovered, width, height);
            @checked = validateAnimation(id, @checked, width, height);
            checkedHovered = validateAnimation(id, checkedHovered, width, height);
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ICheckBox checkbox = _resolver.Container.Resolve<ICheckBox>(idParam);
            checkbox.TextConfig = config;
            checkbox.Text = text;
            if (!isCheckButton)
            {
                checkbox.SkinTags.Add(AGSSkin.CheckBoxTag);
            }
            checkbox.LabelRenderSize = new AGS.API.SizeF(width, height);
            if (notChecked != null) checkbox.NotCheckedAnimation = notChecked;
            if (notCheckedHovered != null) checkbox.HoverNotCheckedAnimation = notCheckedHovered;
            if (@checked != null) checkbox.CheckedAnimation = @checked;
            if (checkedHovered != null) checkbox.HoverCheckedAnimation = checkedHovered;

            checkbox.Tint = pixelArtButton ? Colors.White : Colors.Transparent;
            checkbox.X = x;
            checkbox.Y = y;
            setParent(checkbox, parent);

			var skin = checkbox.Skin;
			if (skin != null) skin.Apply(checkbox);
            checkbox.StartAnimation(checkbox.NotCheckedAnimation.Animation);

            if (addToUi)
                _gameState.UI.Add(checkbox);

            return checkbox;
        }

        public ICheckBox GetCheckBox(string id, IAnimation notChecked, IAnimation notCheckedHovered, IAnimation @checked, IAnimation checkedHovered,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            return GetCheckBox(id, new ButtonAnimation(notChecked), new ButtonAnimation(notCheckedHovered),
                               new ButtonAnimation(@checked), new ButtonAnimation(checkedHovered), x, y, parent,
                               text, config, addToUi, width, height, isCheckButton);
        }

        public ICheckBox GetCheckBox(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            IAnimation notChecked = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = _graphics.LoadAnimationFromFiles(files: new[] { checkedPath });
            IAnimation checkedHovered = _graphics.LoadAnimationFromFiles(files: new[] { checkedHoveredPath });

            return GetCheckBox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, parent, text, config, addToUi, width, height, isCheckButton);
        }

        public async Task<ICheckBox> GetCheckBoxAsync(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            IAnimation notChecked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedPath });
            IAnimation checkedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedHoveredPath });

            return GetCheckBox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, parent, text, config, addToUi, width, height, isCheckButton);
        }

        public IComboBox GetComboBox(string id, IButton dropDownButton, ITextBox textBox,
            Func<string, IButton> itemButtonFactory, IObject parent = null, bool addToUi = true)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IComboBox comboBox = _resolver.Container.Resolve<IComboBox>(idParam);
            float defaultHeight = dropDownButton != null ? dropDownButton.Height : textBox != null ? textBox.Height : 20f;
            float itemWidth = textBox != null ? textBox.Width : 100f;
            if (dropDownButton == null) dropDownButton = GetButton(id + "_DropDownButton", (string)null, null, null, 0f, 0f, comboBox, width: 20f, height: defaultHeight);
            else setParent(dropDownButton, comboBox);
            dropDownButton.SkinTags.Add(AGSSkin.DropDownButtonTag);
            if (dropDownButton.Skin != null) dropDownButton.Skin.Apply(dropDownButton);
            if (textBox == null) textBox = GetTextBox(id + "_TextBox", 0f, 0f, comboBox, width: itemWidth, height: defaultHeight);
            else setParent(textBox, comboBox);
            textBox.Enabled = false;
            itemButtonFactory = itemButtonFactory ?? (text => GetButton(id + "_" + text, (string)null, null, null, 0f, 0f, width: itemWidth,
                height: defaultHeight));

            var dropDownPanel = GetPanel(id + "_DropDownPanel", new EmptyImage(1f, 1f), 0f, 0f, comboBox);
            dropDownPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            dropDownPanel.AddComponent<IStackLayoutComponent>();
            var listBox = dropDownPanel.AddComponent<IListboxComponent>();
            listBox.ItemButtonFactory = itemButtonFactory;

            comboBox.DropDownButton = dropDownButton;
            comboBox.TextBox = textBox;
            comboBox.DropDownPanel = dropDownPanel;

            setParent(comboBox, parent);

            if (addToUi)
            {
                _gameState.UI.Add(comboBox);
            }

            return comboBox;
        }

        public ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max,
            IObject parent = null, ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
        {
            var image = imagePath == null ? null : _graphics.LoadImage(imagePath, loadConfig);
            var handleImage = handleImagePath == null ? null : _graphics.LoadImage(handleImagePath, loadConfig);
            return getSlider(id, image, handleImage, value, min, max, parent, config, addToUi);
        }

        public async Task<ISlider> GetSliderAsync(string id, string imagePath, string handleImagePath, float value, float min, float max,
            IObject parent = null, ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
        {
            var image = imagePath == null ? null : await _graphics.LoadImageAsync(imagePath, loadConfig);
            var handleImage = handleImagePath == null ? null : await _graphics.LoadImageAsync(handleImagePath, loadConfig);
            return getSlider(id, image, handleImage, value, min, max, parent, config, addToUi);
        }

		private ISlider getSlider(string id, IImage image, IImage handleImage, float value, float min, float max,
            IObject parent = null, ITextConfig config = null, bool addToUi = true)
        {
            IObject graphics = _object.GetObject(string.Format("{0}(graphics)", id));
            graphics.Image = image ?? new EmptyImage(10f, 100f);
            graphics.IgnoreViewport = true;
            ILabel label = null;
            if (config != null)
            {
                label = GetLabel(string.Format("{0}(label)", id), "", graphics.Width, 30f, 0f, -30f, parent, config, false);
                if (parent != null) label.RenderLayer = parent.RenderLayer;
                label.Anchor = new PointF(0.5f, 0f);
            }

            IObject handle = _object.GetObject(string.Format("{0}(handle)", id));
            handle.Image = handleImage ?? new EmptyImage(20f, 20f);
            handle.IgnoreViewport = true;

            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ISlider slider = _resolver.Container.Resolve<ISlider>(idParam, idParam);
            setParent(slider, parent);
            slider.Label = label;
            slider.MinValue = min;
            slider.MaxValue = max;
            slider.Value = value;
            slider.Graphics = graphics;
            slider.HandleGraphics = handle;
            slider.IgnoreViewport = true;
            if (parent != null)
            {
                slider.RenderLayer = parent.RenderLayer;
                slider.Graphics.RenderLayer = parent.RenderLayer;
                slider.HandleGraphics.RenderLayer = parent.RenderLayer;
            }

            if (addToUi)
                _gameState.UI.Add(slider);
            return slider;
        }

        private ButtonAnimation validateAnimation(string id, ButtonAnimation button, float width, float height)
        {
            button = button ?? new ButtonAnimation(null);
            if (button.Animation != null) return button;
            if (width == -1f || height == -1)
            {
                throw new InvalidOperationException("No animation and no size was supplied for GUI control " + id);
            }
            button.Animation = new AGSSingleFrameAnimation(new EmptyImage(width, height), _graphics);
            return button;
        }

        private void setParent(IObject ui, IObject parent)
        {
            if (parent == null) return;
            ui.TreeNode.SetParent(parent.TreeNode);
        }
    }
}

