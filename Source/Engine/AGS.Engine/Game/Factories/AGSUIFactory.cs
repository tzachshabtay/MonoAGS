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
        private readonly IBorderFactory _borders;
        private readonly IObjectFactory _object;
        private readonly IGameSettings _settings;
        private readonly IFocusedUI _focus;

        public AGSUIFactory(Resolver resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj, 
                            IGameSettings settings, IFocusedUI focusedUI)
        {
            _settings = settings;
            _resolver = resolver;
            _gameState = gameState;
            _graphics = graphics;
            _borders = graphics.Borders;
            _object = obj;
            _focus = focusedUI;
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

        [MethodWizard]
        public IPanel GetPanel(string id, 
                               [MethodParam(Default = 100f)]float width, [MethodParam(Default = 50f)]float height, 
                               float x, float y, 
                               [MethodParam(Browsable = false)]IObject parent = null, 
                               [MethodParam(Browsable = false, Default = false)]bool addToUi = true)
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

        public IScrollbar GetScrollbar(string idPrefix, SliderDirection direction, 
                                       IObject parent = null, float width = 15f, float height = 15f, float step = 10f, float buttonBorderWidth = 1f)
        {
            var slider = GetSlider($"{idPrefix}_{direction}Slider", null, null, 0f, 0f, 0f, parent);
            slider.ShouldClampValuesWhenChangingMinMax = false;
            slider.Direction = direction;
            slider.HandleGraphics.Pivot = slider.IsHorizontal() ? new PointF(1f, 0f) : new PointF(0f, 1f);
            slider.Graphics.Pivot = new PointF(0f, 0f);
            slider.Graphics.Border = _borders.SolidColor(Colors.DarkGray, 0.5f, true);
            slider.HandleGraphics.Border = _borders.SolidColor(Colors.White, 0.5f, true);
            float gutterSize = slider.IsHorizontal() ? height : width;
            HoverEffect.Add(slider.Graphics, Colors.Gray, Colors.LightGray);
            HoverEffect.Add(slider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

            (var up, var down) = slider.IsHorizontal() ? (ArrowDirection.Left, ArrowDirection.Right) : (ArrowDirection.Up, ArrowDirection.Down);
            PointF pivot = slider.IsHorizontal() ? (1f, 0f) : (0f, 1f);
            (var upX, var upY) = slider.IsHorizontal() ? (0f, buttonBorderWidth / 2f) : (buttonBorderWidth / 2f, height - gutterSize * 2f);
            (var idle, var hovered, var pushed) = getArrowButtonAnimations(up, buttonBorderWidth);
            var upButton = GetButton($"{idPrefix}_Scroll{up}Button", idle, hovered, pushed, upX, upY, slider, width: gutterSize - buttonBorderWidth, height: gutterSize - buttonBorderWidth);
            upButton.Pivot = pivot;
            upButton.MouseClicked.Subscribe(args => 
            {
                if (direction == SliderDirection.TopToBottom || direction == SliderDirection.LeftToRight)
                    slider.Decrease(step);
                else slider.Increase(step);
                _focus.HasKeyboardFocus = slider;
            });

            (var downX, var downY) = slider.IsHorizontal() ? (width - gutterSize * 2f, buttonBorderWidth / 2f) : (buttonBorderWidth / 2f, 0f);
            (idle, hovered, pushed) = getArrowButtonAnimations(down, buttonBorderWidth);
            var downButton = GetButton($"{idPrefix}_Scroll{down}Button", idle, hovered, pushed, downX, downY, slider, width: gutterSize - buttonBorderWidth, height: gutterSize - buttonBorderWidth);
            downButton.Pivot = pivot;
            downButton.MouseClicked.Subscribe(args => 
            {
                if (direction == SliderDirection.TopToBottom || direction == SliderDirection.LeftToRight)   
                    slider.Increase(step);
                else slider.Decrease(step);
                _focus.HasKeyboardFocus = slider;
            });

            var scrollbar = new AGSScrollbar(upButton, downButton, slider);
            scrollbar.Step = step;
            return scrollbar;
        }

        public IPanel CreateScrollingPanel(IPanel panel, float gutterSize = 15f, float stepHorizontal = 10f, float stepVertical = 10f)
        {
            var contentsPanel = GetPanel($"{panel.ID}_Contents", panel.Width, panel.Height, 0f, 0f, panel);
            contentsPanel.Opacity = 0;

            contentsPanel.RenderLayer = panel.RenderLayer;

            contentsPanel.AddComponent<ICropChildrenComponent>();
            var box = contentsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            box.IncludeSelf = false;
            IScrollingComponent scroll = contentsPanel.AddComponent<IScrollingComponent>();

            const float buttonBorderWidth = 1f;

            var horizScrollbar = GetScrollbar(panel.ID, SliderDirection.LeftToRight, panel, panel.Width, gutterSize, buttonBorderWidth: buttonBorderWidth);
            var verScrollbar = GetScrollbar(panel.ID, SliderDirection.TopToBottom, panel, gutterSize, panel.Height, buttonBorderWidth: buttonBorderWidth);
            horizScrollbar.Slider.Y = -buttonBorderWidth;

            PropertyChangedEventHandler resize = (_, args) =>
            {
                if (args.PropertyName != nameof(IScaleComponent.Width) && args.PropertyName != nameof(IScaleComponent.Height)) return;
                panel.BaseSize = new SizeF(contentsPanel.Width + gutterSize, contentsPanel.Height + gutterSize);
                horizScrollbar.Slider.Graphics.Image = new EmptyImage(panel.Width - gutterSize * 3f, gutterSize);
                horizScrollbar.Slider.HandleGraphics.Image = new EmptyImage(horizScrollbar.Slider.HandleGraphics.Image.Width, gutterSize);
                verScrollbar.Slider.Graphics.Image = new EmptyImage(gutterSize, panel.Height - gutterSize * 3f);
                verScrollbar.Slider.HandleGraphics.Image = new EmptyImage(gutterSize, verScrollbar.Slider.HandleGraphics.Image.Height);
                horizScrollbar.Slider.X = -panel.Width * panel.Pivot.X + gutterSize;
                verScrollbar.Slider.X = panel.Width - gutterSize + buttonBorderWidth;
                verScrollbar.Slider.Y = gutterSize * 2f;
                verScrollbar.UpButton.Y = panel.Height - gutterSize * 2f;
                horizScrollbar.DownButton.X = panel.Width - gutterSize * 2f;
                contentsPanel.Y = gutterSize;
            };

            resize(this, new PropertyChangedEventArgs(nameof(IScaleComponent.Width)));
            scroll.HorizontalScrollBar = horizScrollbar.Slider;
            scroll.VerticalScrollBar = verScrollbar.Slider;

            contentsPanel.Bind<IScaleComponent>(c => c.PropertyChanged += resize, c => c.PropertyChanged -= resize);

            return contentsPanel;
        }

        [MethodWizard]
        public ILabel GetLabel(string id, [MethodParam(Default = "")]string text, 
                               [MethodParam(Default = 80f)]float width, [MethodParam(Default = 30f)]float height, 
                               float x, float y, 
                               [MethodParam(Browsable = false)]IObject parent = null, ITextConfig config = null, 
                               [MethodParam(Browsable = false, Default = false)]bool addToUi = true)
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

        public static ButtonAnimation GetDefaultIdleAnimation(Resolver resolver) 
            => new ButtonAnimation(getDefaultBorder(resolver), 
                               new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, 
                                                 brush: AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.WhiteSmoke)), 
                               Color.FromRgba(44, 51, 61, 255));

        public static ButtonAnimation GetDefaultHoverAnimation(Resolver resolver)
            => new ButtonAnimation(null, null, Colors.Gray);

        public static ButtonAnimation GetDefaultPushedAnimation(Resolver resolver)
            => new ButtonAnimation(null, null, Colors.DarkGray);

        private static IBorderStyle getDefaultBorder(Resolver resolver)
        {
            var borders = resolver.Container.Resolve<IBorderFactory>();
            return borders.SolidColor(Colors.Black, 1f);
        }

        public static List<object> ConvertListboxToEntities(IListbox listbox)
        {
            List<object> entities = new List<object>();
            entities.Add(listbox.ContentsPanel);
            if (listbox.ScrollingPanel != null) entities.Add(listbox.ScrollingPanel);
            return entities;
        }

        public static List<object> ConvertCheckboxToEntities(ICheckBox checkbox)
        {
            List<object> entities = new List<object>();
            entities.Add(checkbox);
            if (checkbox.TextLabel != null) entities.Add(checkbox.TextLabel);
            return entities;
        }

        [MethodWizard]
        public IButton GetButton(string id, 
             [MethodParam(DefaultProvider = nameof(GetDefaultIdleAnimation))] ButtonAnimation idle, 
             [MethodParam(DefaultProvider = nameof(GetDefaultHoverAnimation))] ButtonAnimation hovered, 
             [MethodParam(DefaultProvider = nameof(GetDefaultPushedAnimation))] ButtonAnimation pushed,
             float x, float y, [MethodParam(Browsable = false)] IObject parent = null, string text = "", 
             [MethodParam(Browsable = false)] ITextConfig config = null, 
             [MethodParam(Browsable = false, Default = false)] bool addToUi = true,
             [MethodParam(Browsable = false, Default = 25f)] float width = -1f,
             [MethodParam(Browsable = false, Default = 25f)] float height = -1f)
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

        [MethodWizard]
        public ITextBox GetTextBox(string id, float x, float y, 
            [MethodParam(Browsable = false)]IObject parent = null, string watermark = "", ITextConfig config = null,
            [MethodParam(Browsable = false, Default = false)] bool addToUi = true, 
            [MethodParam(Browsable = false, Default = 80f)]float width = -1F, 
            [MethodParam(Browsable = false, Default = 25f)]float height = -1F)
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

        [MethodWizard(EntitiesProvider = nameof(ConvertCheckboxToEntities))]
        public ICheckBox GetCheckBox(string id, 
            ButtonAnimation notChecked, ButtonAnimation notCheckedHovered, ButtonAnimation @checked, ButtonAnimation checkedHovered,
            float x, float y, [MethodParam(Browsable = false)]IObject parent = null, string text = "", ITextConfig config = null, 
            [MethodParam(Browsable = false, Default = false)]bool addToUi = true, 
            [MethodParam(Browsable = false, Default = 25f)] float width = -1f,
            [MethodParam(Browsable = false, Default = 25f)] float height = -1f, bool isCheckButton = false)
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
            config = config ?? new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont);

            var idleColor = Colors.White;
            var hoverColor = Colors.Yellow;
            const float lineWidth = 1f;
            const float padding = 300f;
            Func<ButtonAnimation> notCheckedDefault = () => new ButtonAnimation(_borders.SolidColor(idleColor, lineWidth), null, Colors.Black);
            Func<ButtonAnimation> checkedDefault = () =>
            {
                var checkIcon = _graphics.Icons.GetXIcon(color: idleColor, padding: padding);
                return new ButtonAnimation(_borders.Multiple(_borders.SolidColor(idleColor, lineWidth), checkIcon), null, Colors.Black);
            };
            Func<ButtonAnimation> hoverNotCheckedDefault = () => new ButtonAnimation(_borders.SolidColor(hoverColor, lineWidth), null, Colors.Black);
            Func<ButtonAnimation> hoverCheckedDefault = () =>
            {
                var checkHoverIcon = _graphics.Icons.GetXIcon(color: hoverColor, padding: padding);
                return new ButtonAnimation(_borders.Multiple(_borders.SolidColor(hoverColor, lineWidth), checkHoverIcon), null, Colors.Black);
            }; 

            notChecked = validateAnimation(id, notChecked, notCheckedDefault, width, height);
            notCheckedHovered = validateAnimation(id, notCheckedHovered, hoverNotCheckedDefault, width, height);
            @checked = validateAnimation(id, @checked, checkedDefault, width, height);
            checkedHovered = validateAnimation(id, checkedHovered, hoverCheckedDefault, width, height);
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            ICheckBox checkbox = _resolver.Container.Resolve<ICheckBox>(idParam);
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

            if (!string.IsNullOrEmpty(text))
            {
                checkbox.TextLabel = GetLabel($"{id}_Label", text, 100f, 20f, width + 5f, height / 2f, checkbox, config, addToUi);
                checkbox.TextLabel.Pivot = (0f, 0.5f);
            }

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

        [MethodWizard(EntitiesProvider = nameof(ConvertListboxToEntities))]
        public IListbox GetListBox(string id, [MethodParam(Browsable = false)]IRenderLayer layer, 
           [MethodParam(Browsable = false)]Func<string, IUIControl> listItemFactory = null,
           float defaultWidth = 500f, float defaultHeight = 40f, 
           [MethodParam(Browsable = false, Default = false)]bool addToUi = true, 
           bool isVisible = true, bool withScrollBars = true)
        {
            layer = layer ?? AGSLayers.UI;
            if (listItemFactory == null)
            {
                var yellowBrush = _graphics.Brushes.LoadSolidBrush(Colors.Yellow);
                var whiteBrush = _graphics.Brushes.LoadSolidBrush(Colors.White);
                listItemFactory = text =>
                {
                    var button = GetButton(id + "_" + text,
                                           new ButtonAnimation(null, new AGSTextConfig(whiteBrush, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                           new ButtonAnimation(null, new AGSTextConfig(yellowBrush, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                           new ButtonAnimation(null, new AGSTextConfig(yellowBrush, outlineBrush: whiteBrush, outlineWidth: 0.5f, autoFit: AutoFit.LabelShouldFitText, font: _settings.Defaults.TextFont), null),
                                           0f, 0f, width: defaultWidth, height: defaultHeight);
                    button.Pivot = new PointF(0f, 1f);
                    button.RenderLayer = layer;
                    button.Text = text;
                    return button;
                };
            }

            var scrollingPanel = GetPanel(id + "_DropDownPanel", new EmptyImage(1f, 1f), 0f, 0f, null, false);
            scrollingPanel.Visible = isVisible;
            scrollingPanel.Border = _borders.SolidColor(Colors.White, 3f);
            scrollingPanel.Tint = Colors.Black;
            scrollingPanel.RenderLayer = layer;
            var contentsPanel = withScrollBars ? CreateScrollingPanel(scrollingPanel) : scrollingPanel;
            var listBox = contentsPanel.AddComponent<IListboxComponent>();
            listBox.ListItemFactory = listItemFactory;
            listBox.MaxHeight = 300f;

            contentsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            contentsPanel.AddComponent<IStackLayoutComponent>();

            if (addToUi)
            {
                _gameState.UI.Add(scrollingPanel);
            }

            return new AGSListbox(withScrollBars ? scrollingPanel : null, contentsPanel, listBox);
        }

        [MethodWizard]
        public IComboBox GetComboBox(string id, [MethodParam(Browsable = false)]IButton dropDownButton = null, 
            [MethodParam(Browsable = false)]ITextBox textBox = null,
            [MethodParam(Browsable = false)]Func<string, IUIControl> listItemFactory = null, 
            [MethodParam(Browsable = false)]IObject parent = null, 
            [MethodParam(Browsable = false, Default = false)]bool addToUi = true, 
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
                textBox.Border = _borders.SolidColor(Colors.WhiteSmoke, 3f);
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
            var listbox = GetListBox(id, dropDownPanelLayer, listItemFactory, itemWidth, defaultHeight, isVisible: false);
            _gameState.FocusedUI.CannotLoseFocus.Add(listbox.ScrollingPanel.ID);

            Action placePanel = () =>
            {
                var box = textBox.GetBoundingBoxes(_gameState.Viewport)?.ViewportBox;
                if (box == null) return;
                listbox.ScrollingPanel.Position = new Position(box.Value.BottomLeft.X, box.Value.BottomLeft.Y);
            };

            textBox.OnBoundingBoxesChanged.Subscribe(placePanel);
            placePanel();

            comboBox.DropDownButton = dropDownButton;
            comboBox.TextBox = textBox;
            comboBox.DropDownPanel = listbox;

            setParent(comboBox, parent);

            if (addToUi)
            {
                _gameState.UI.Add(textBox);
                _gameState.UI.Add(dropDownButton);
                _gameState.UI.Add(comboBox);
            }

            return comboBox;
        }

        [MethodWizard]
        public ISlider GetSlider(string id, 
            [MethodParam(Browsable = false)]string imagePath, 
            [MethodParam(Browsable = false)]string handleImagePath, float value, float min, float max,
            [MethodParam(Browsable = false)]IObject parent = null, ITextConfig config = null, 
            [MethodParam(Browsable = false)]ILoadImageConfig loadConfig = null, 
            [MethodParam(Browsable = false, Default = false)]bool addToUi = true)
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
            var whiteArrow = _borders.Multiple(_borders.SolidColor(Colors.WhiteSmoke, lineWidth),
                                                 _graphics.Icons.GetArrowIcon(direction, Colors.WhiteSmoke));
            var yellowArrow = _borders.Multiple(_borders.SolidColor(Colors.Yellow, lineWidth),
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
                throw new InvalidOperationException("No animation/image and no size was supplied for GUI control " + id);
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