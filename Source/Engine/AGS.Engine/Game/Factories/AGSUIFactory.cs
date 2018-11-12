using System.Threading.Tasks;
using AGS.API;
using Autofac;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

namespace AGS.Engine
{
    public class AGSUIFactory : IUIFactory
    {
        private readonly Resolver _resolver;
        private readonly IGameState _gameState;
        private readonly IGraphicsFactory _graphics;
        private readonly IObjectFactory _object;
        private readonly IGameSettings _settings;

        public AGSUIFactory(Resolver resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj, IGameSettings settings)
        {
            _settings = settings;
            _resolver = resolver;
            _gameState = gameState;
            _graphics = graphics;
            _object = obj;
        }

        public IPanel GetPanel(string id, IImage image, float x, float y, IObject parent = null, bool addToUi = true)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IPanel panel = _resolver.Container.Resolve<IPanel>(idParam);
            panel.Image = image;
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

        public IPanel CreateScrollingPanel(IPanel panel, float gutterSize = 15f)
        {
            var contentsPanel = GetPanel($"{panel.ID}_Contents", panel.Width, panel.Height, 0f, 0f, panel);
            contentsPanel.Opacity = 0;

            contentsPanel.RenderLayer = panel.RenderLayer;

            contentsPanel.AddComponent<ICropChildrenComponent>();
            contentsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            IScrollingComponent scroll = contentsPanel.AddComponent<IScrollingComponent>();

            var horizSlider = GetSlider($"{panel.ID}_HorizontalSlider", null, null, 0f, 0f, 0f, panel);
			horizSlider.HandleGraphics.Pivot = new PointF(0f, 0f);
            horizSlider.Direction = SliderDirection.LeftToRight;
			horizSlider.Graphics.Pivot = new PointF(0f, 0f);
			horizSlider.Graphics.Border = AGSBorders.SolidColor(Colors.DarkGray, 0.5f, true);
			horizSlider.HandleGraphics.Border = AGSBorders.SolidColor(Colors.White, 0.5f, true);
            HoverEffect.Add(horizSlider.Graphics, Colors.Gray, Colors.LightGray);
			HoverEffect.Add(horizSlider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

            var verSlider = GetSlider($"{panel.ID}_VerticalSlider", null, null, 0f, 0f, 0f, panel);
			verSlider.HandleGraphics.Pivot = new PointF(0f, 0f);
            verSlider.Direction = SliderDirection.TopToBottom;
			verSlider.Graphics.Pivot = new PointF(0f, 0f);
			verSlider.Graphics.Border = AGSBorders.SolidColor(Colors.DarkGray, 0.5f, true);
			verSlider.HandleGraphics.Border = AGSBorders.SolidColor(Colors.White, 0.5f, true);
            verSlider.MaxHandleOffset = gutterSize;
			HoverEffect.Add(verSlider.Graphics, Colors.Gray, Colors.LightGray);
            HoverEffect.Add(verSlider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

            (var idle, var hovered, var pushed) = getArrowButtonAnimations(ArrowDirection.Up, 1f);
            var upButton = GetButton($"{panel.ID}_ScrollUpButton", idle, hovered, pushed, 0f, panel.Height - gutterSize * 2f, verSlider, width: gutterSize, height: gutterSize);
            upButton.Pivot = new PointF(0f, 1f);
            upButton.MouseClicked.Subscribe(args => verSlider.Value--);

            (idle, hovered, pushed) = getArrowButtonAnimations(ArrowDirection.Down, 1f);
            var downButton = GetButton($"{panel.ID}_ScrollDownButton", idle, hovered, pushed, 0f, 0f, verSlider, width: gutterSize, height: gutterSize);
            downButton.Pivot = new PointF(0f, 1f);
            downButton.MouseClicked.Subscribe(args => verSlider.Value++);

            (idle, hovered, pushed) = getArrowButtonAnimations(ArrowDirection.Left, 1f);
            var leftButton = GetButton($"{panel.ID}_ScrollUpLeft", idle, hovered, pushed, 0f, 0f, horizSlider, width: gutterSize, height: gutterSize);
            leftButton.Pivot = new PointF(1f, 0f);
            leftButton.MouseClicked.Subscribe(args => horizSlider.Value--);

            (idle, hovered, pushed) = getArrowButtonAnimations(ArrowDirection.Right, 1f);
            var rightButton = GetButton($"{panel.ID}_ScrollDownRight", idle, hovered, pushed, panel.Width - gutterSize * 2f, 0f, horizSlider, width: gutterSize, height: gutterSize);
            rightButton.Pivot = new PointF(0f, 0f);
            rightButton.MouseClicked.Subscribe(args => horizSlider.Value++);

            PropertyChangedEventHandler resize = (_, args) =>
            {
                if (args.PropertyName != nameof(IScaleComponent.Width) && args.PropertyName != nameof(IScaleComponent.Height)) return;
                panel.BaseSize = new SizeF(contentsPanel.Width + gutterSize, contentsPanel.Height + gutterSize);
                horizSlider.Graphics.Image = new EmptyImage(panel.Width - gutterSize * 2f, gutterSize);
                horizSlider.HandleGraphics.Image = new EmptyImage(gutterSize, gutterSize);
                verSlider.Graphics.Image = new EmptyImage(gutterSize, panel.Height - gutterSize * 3f);
                verSlider.HandleGraphics.Image = new EmptyImage(gutterSize, gutterSize);
                horizSlider.X = -panel.Width * panel.Pivot.X + gutterSize;
                verSlider.X = panel.Width - gutterSize;
                verSlider.Y = gutterSize * 2f;
                upButton.Y = panel.Height - gutterSize * 2f;
                rightButton.X = panel.Width - gutterSize * 2f;
                contentsPanel.Y = gutterSize;
            };

            resize(this, new PropertyChangedEventArgs(nameof(IScaleComponent.Width)));
            scroll.HorizontalScrollBar = horizSlider;
			scroll.VerticalScrollBar = verSlider;

            contentsPanel.Bind<IScaleComponent>(c => c.PropertyChanged += resize, c => c.PropertyChanged -= resize);

            return contentsPanel;
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
            label.TextConfig = config ?? new AGSTextConfig(font: _settings.Defaults.TextFont);
            setParent(label, parent);
            if (addToUi)
                _gameState.UI.Add(label);
            return label;
        }

        public IButton GetButton(string id, ButtonAnimation idle, ButtonAnimation hovered, ButtonAnimation pushed,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            bool pixelArtButton = idle?.Image != null || (idle?.Animation != null && idle.Animation.Frames.Count > 0);
            if (width == -1f && pixelArtButton)
            {
                width = idle.Image?.Width ?? idle.Animation.Frames[0].Sprite.Width;
            }
            if (height == -1f && pixelArtButton)
            {
                height = idle.Image?.Height ?? idle.Animation.Frames[0].Sprite.Height;
            }
            Func<ButtonAnimation> defaultAnimation = () => new ButtonAnimation((IImage)null);
            idle = validateAnimation(id, idle, defaultAnimation, width, height);
            hovered = validateAnimation(id, hovered, defaultAnimation, width, height);
            pushed = validateAnimation(id, pushed, defaultAnimation, width, height);

            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IButton button = _resolver.Container.Resolve<IButton>(idParam);
            button.LabelRenderSize = new SizeF(width, height);
            button.IdleAnimation = idle;
            button.HoverAnimation = hovered;
            button.PushedAnimation = pushed;

            button.Tint = pixelArtButton ? Colors.White : Colors.Transparent;
            button.X = x;
            button.Y = y;
            button.TextConfig = config ?? new AGSTextConfig(alignment: Alignment.MiddleCenter, font: _settings.Defaults.TextFont);
            button.Text = text;
            setParent(button, parent);

            button.Skin?.Apply(button);
            button.IdleAnimation.StartAnimation(button, button, button, button);

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
            ButtonAnimation idle = new ButtonAnimation(_graphics.LoadImage(idleImagePath));
            ButtonAnimation hovered = new ButtonAnimation(_graphics.LoadImage(hoveredImagePath));
            ButtonAnimation pushed = new ButtonAnimation(_graphics.LoadImage(pushedImagePath));

            return GetButton(id, idle, hovered, pushed, x, y, parent, text, config, addToUi, width, height);
        }

        public async Task<IButton> GetButtonAsync(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true,
            float width = -1f, float height = -1f)
        {
            ButtonAnimation idle = new ButtonAnimation(await _graphics.LoadImageAsync(idleImagePath));
            ButtonAnimation hovered = new ButtonAnimation(await _graphics.LoadImageAsync(hoveredImagePath));
            ButtonAnimation pushed = new ButtonAnimation(await _graphics.LoadImageAsync(pushedImagePath));

            return GetButton(id, idle, hovered, pushed, x, y, parent, text, config, addToUi, width, height);
        }

        public ITextBox GetTextBox(string id, float x, float y, IObject parent = null, string watermark = "", ITextConfig config = null,
            bool addToUi = true, float width = -1F, float height = -1F)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ITextBox textbox = _resolver.Container.Resolve<ITextBox>(idParam);
            textbox.LabelRenderSize = new SizeF(width, height);
            textbox.X = x;
            textbox.Y = y;
            if (config == null)
            {
                config = new AGSTextConfig(autoFit: AutoFit.TextShouldCrop, font: _settings.Defaults.TextFont);
            }
            textbox.TextConfig = config;
            textbox.Text = "";
            setParent(textbox, parent);
            if (!string.IsNullOrEmpty(watermark))
            {
                var watermarkConfig = AGSTextConfig.Clone(config);
                watermarkConfig.Brush = _graphics.Brushes.LoadSolidBrush(Colors.LightGray);
                var watermarkLabel = GetLabel($"{id}_watermark", watermark, width, height, 0f, 0f, textbox, watermarkConfig, addToUi);
                watermarkLabel.Opacity = 50;
                textbox.Watermark = watermarkLabel;
            }

            if (addToUi)
                _gameState.UI.Add(textbox);

            return textbox;
        }

        public ICheckBox GetCheckBox(string id, ButtonAnimation notChecked, ButtonAnimation notCheckedHovered, ButtonAnimation @checked, ButtonAnimation checkedHovered,
            float x, float y, IObject parent = null, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            bool pixelArtButton = notChecked?.Image != null || (notChecked?.Animation != null && notChecked.Animation.Frames.Count > 0);
            if (width == -1f && pixelArtButton)
            {
                width = notChecked.Image?.Width ?? notChecked.Animation.Frames[0].Sprite.Width;
            }
            if (height == -1f && pixelArtButton)
            {
                height = notChecked.Image?.Height ?? notChecked.Animation.Frames[0].Sprite.Height;
            }

            var idleColor = Colors.White;
            var hoverColor = Colors.Yellow;
            const float lineWidth = 1f;
            const float padding = 300f;
            Func<ButtonAnimation> notCheckedDefault = () => new ButtonAnimation(AGSBorders.SolidColor(idleColor, lineWidth), null, Colors.Black);
            Func<ButtonAnimation> checkedDefault = () =>
            {
                var checkIcon = _graphics.Icons.GetXIcon(color: idleColor, padding: padding);
                return new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(idleColor, lineWidth), checkIcon), null, Colors.Black);
            };
            Func<ButtonAnimation> hoverNotCheckedDefault = () => new ButtonAnimation(AGSBorders.SolidColor(hoverColor, lineWidth), null, Colors.Black);
            Func<ButtonAnimation> hoverCheckedDefault = () =>
            {
                var checkHoverIcon = _graphics.Icons.GetXIcon(color: hoverColor, padding: padding);
                return new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(hoverColor, lineWidth), checkHoverIcon), null, Colors.Black);
            }; 

            notChecked = validateAnimation(id, notChecked, notCheckedDefault, width, height);
            notCheckedHovered = validateAnimation(id, notCheckedHovered, hoverNotCheckedDefault, width, height);
            @checked = validateAnimation(id, @checked, checkedDefault, width, height);
            checkedHovered = validateAnimation(id, checkedHovered, hoverCheckedDefault, width, height);
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ICheckBox checkbox = _resolver.Container.Resolve<ICheckBox>(idParam);
            if (!string.IsNullOrEmpty(text))
            {
                checkbox.TextLabel = GetLabel($"{id}_Label", text, 100f, 20f, x + width + 5f, y, parent, config, addToUi);
            }
            if (!isCheckButton)
            {
                checkbox.SkinTags.Add(AGSSkin.CheckBoxTag);
            }
            if (notChecked != null) checkbox.NotCheckedAnimation = notChecked;
            if (notCheckedHovered != null) checkbox.HoverNotCheckedAnimation = notCheckedHovered;
            if (@checked != null) checkbox.CheckedAnimation = @checked;
            if (checkedHovered != null) checkbox.HoverCheckedAnimation = checkedHovered;

            checkbox.Tint = pixelArtButton ? Colors.White : Colors.Transparent;
            checkbox.X = x;
            checkbox.Y = y;
            setParent(checkbox, parent);

            checkbox.Skin?.Apply(checkbox);
            checkbox.NotCheckedAnimation.StartAnimation(checkbox, checkbox.TextLabel, checkbox, checkbox);

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

        public IComboBox GetComboBox(string id, IButton dropDownButton = null, ITextBox textBox = null,
            Func<string, IButton> itemButtonFactory = null, IObject parent = null, bool addToUi = true, 
            float defaultWidth = 500f, float defaultHeight = 40f, string watermark = "")
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IComboBox comboBox = _resolver.Container.Resolve<IComboBox>(idParam);
            if (parent != null) comboBox.RenderLayer = parent.RenderLayer;
            defaultHeight = dropDownButton?.Height ?? textBox?.Height ?? defaultHeight;
            float itemWidth = textBox?.Width ?? defaultWidth;

            if (textBox == null)
            {
                textBox = GetTextBox(id + "_TextBox", 0f, 0f, comboBox, watermark, new AGSTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel, font: _settings.Defaults.TextFont),
                                     false, itemWidth, defaultHeight);
				textBox.Border = AGSBorders.SolidColor(Colors.WhiteSmoke, 3f);
				textBox.Tint = Colors.Transparent;
            }
            else setParent(textBox, comboBox);
			textBox.RenderLayer = comboBox.RenderLayer;
			textBox.Enabled = false;

            if (dropDownButton == null)
            {
                (var idle, var hovered, var pushed) = getArrowButtonAnimations(ArrowDirection.Down, 3f);
				dropDownButton = GetButton(id + "_DropDownButton", idle, hovered, pushed, 0f, 0f, comboBox, "", null, false, 30f, defaultHeight);
                dropDownButton.Border = idle.Border;
			}
			else setParent(dropDownButton, comboBox);
            dropDownButton.RenderLayer = comboBox.RenderLayer;
            dropDownButton.Z = textBox.Z - 1;
			dropDownButton.SkinTags.Add(AGSSkin.DropDownButtonTag);
			dropDownButton.Skin?.Apply(dropDownButton);

            var dropDownPanelLayer = new AGSRenderLayer(comboBox.RenderLayer.Z - 1, comboBox.RenderLayer.ParallaxSpeed, comboBox.RenderLayer.IndependentResolution); //Making sure that the drop-down layer is rendered before the combobox layer, so that it will appear in front of other ui elements that may be below.
			if (itemButtonFactory == null)
            {
                var yellowBrush = _graphics.Brushes.LoadSolidBrush(Colors.Yellow);
				var whiteBrush = _graphics.Brushes.LoadSolidBrush(Colors.White);
				itemButtonFactory = text =>
				{
					var button = GetButton(id + "_" + text,
                                           new ButtonAnimation(null, new AGSTextConfig(whiteBrush, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                           new ButtonAnimation(null, new AGSTextConfig(yellowBrush, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                           new ButtonAnimation(null, new AGSTextConfig(yellowBrush, outlineBrush: whiteBrush, outlineWidth: 0.5f, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                                      0f, 0f, width: itemWidth, height: defaultHeight);
                    button.Pivot = new PointF(0f, 1f);
					button.RenderLayer = dropDownPanelLayer;
					return button;
				};
            }

            var dropDownPanel = GetPanel(id + "_DropDownPanel", new EmptyImage(1f, 1f), 0f, 0f, null, false);
            dropDownPanel.Visible = false;
            _gameState.UI.Add(dropDownPanel);
            dropDownPanel.Border = AGSBorders.SolidColor(Colors.White, 3f);
            dropDownPanel.Tint = Colors.Black;
            dropDownPanel.RenderLayer = dropDownPanelLayer;
            _gameState.FocusedUI.CannotLoseFocus.Add(dropDownPanel.ID);
            var contentsPanel = CreateScrollingPanel(dropDownPanel);
            var listBox = contentsPanel.AddComponent<IListboxComponent>();
            listBox.ItemButtonFactory = itemButtonFactory;
            listBox.MaxHeight = 300f;

            Action placePanel = () =>
            {
                var box = textBox.GetBoundingBoxes(_gameState.Viewport)?.ViewportBox;
                if (box == null) return;
                dropDownPanel.Position = new Position(box.Value.BottomLeft.X, box.Value.BottomLeft.Y);
            };

            textBox.OnBoundingBoxesChanged.Subscribe(placePanel);
            placePanel();

            contentsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            contentsPanel.AddComponent<IStackLayoutComponent>();

            comboBox.DropDownButton = dropDownButton;
            comboBox.TextBox = textBox;
            comboBox.DropDownPanel = contentsPanel;

            setParent(comboBox, parent);

            if (addToUi)
            {
                _gameState.UI.Add(textBox);
                _gameState.UI.Add(dropDownButton);
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
            IObject graphics = _object.GetObject($"{id}(graphics)");
            graphics.Image = image ?? new EmptyImage(10f, 100f);
            graphics.IgnoreViewport = true;
            ILabel label = null;
            if (config != null)
            {
                label = GetLabel($"{id}(label)", "", graphics.Width, 30f, 0f, -30f, parent, config, false);
                if (parent != null) label.RenderLayer = parent.RenderLayer;
                label.Pivot = new PointF(0.5f, 0f);
            }

            IObject handle = _object.GetObject($"{id}(handle)");
            handle.Image = handleImage ?? new EmptyImage(20f, 20f);
            handle.IgnoreViewport = true;

            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ISlider slider = _resolver.Container.Resolve<ISlider>(idParam);
            setParent(slider, parent);
            setParent(handle, slider);
            setParent(graphics, slider);
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

        private (ButtonAnimation idle, ButtonAnimation hovered, ButtonAnimation pushed) getArrowButtonAnimations(ArrowDirection direction, float lineWidth)
        {
            var whiteArrow = AGSBorders.Multiple(AGSBorders.SolidColor(Colors.WhiteSmoke, lineWidth),
                                                 _graphics.Icons.GetArrowIcon(direction, Colors.WhiteSmoke));
            var yellowArrow = AGSBorders.Multiple(AGSBorders.SolidColor(Colors.Yellow, lineWidth),
                                                  _graphics.Icons.GetArrowIcon(direction, Colors.Yellow));

            return (new ButtonAnimation(whiteArrow, null, Colors.Transparent),
                    new ButtonAnimation(yellowArrow, null, Colors.Transparent),
                    new ButtonAnimation(yellowArrow, null, Colors.White.WithAlpha(100)));
        }

        private ButtonAnimation validateAnimation(string id, ButtonAnimation button, Func<ButtonAnimation> defaultAnimation, float width, float height)
        {
            button = button ?? defaultAnimation();
            if (button.Animation != null || button.Image != null) return button;
            if (width == -1f || height == -1)
            {
                throw new InvalidOperationException("No animation and no size was supplied for GUI control " + id);
            }
            button.Image = new EmptyImage(width, height);
            return button;
        }

        private void setParent(IObject ui, IObject parent)
        {
            if (parent == null) return;
            ui.TreeNode.SetParent(parent.TreeNode);
            ui.RenderLayer = parent.RenderLayer;
        }
    }
}
