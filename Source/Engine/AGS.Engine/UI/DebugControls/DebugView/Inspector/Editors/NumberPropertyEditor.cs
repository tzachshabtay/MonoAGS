using System;
using AGS.API;

namespace AGS.Engine
{
    public class NumberPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private readonly bool _wholeNumbers;
        private const float SLIDER_HEIGHT = 5f;
        private const float ROW_HEIGHT = 20f;

        public NumberPropertyEditor(IGameFactory factory, bool wholeNumbers)
        {
            _factory = factory;
            _wholeNumbers = wholeNumbers;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            var label = view.TreeItem;

            var textPanel = _factory.UI.GetPanel(id + "_TextPanel", 100f, ROW_HEIGHT, label.X, label.Y, label.TreeNode.Parent);
            textPanel.RenderLayer = label.RenderLayer;
            textPanel.Z = label.Z;
            textPanel.Tint = Colors.Transparent;
            var textbox = addTextBox(id, textPanel, property.Value);
            var numberEditor = textbox.AddComponent<INumberEditorComponent>();
            addSlider(id, textPanel, numberEditor);
            numberEditor.EditWholeNumbersOnly = _wholeNumbers;
            numberEditor.Value = float.Parse(property.Value);
            numberEditor.OnValueChanged.Subscribe(() =>
            {
                if (_wholeNumbers)
                {
                    property.Prop.SetValue(property.Object, (int)Math.Round(numberEditor.Value));
                }
                else property.Prop.SetValue(property.Object, numberEditor.Value);
            });
            addArrowButtons(id, label, numberEditor);
        }

        private ITextBox addTextBox(string id, IUIControl panel, string text)
        {
            AGSTextConfig config = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText);
            var textbox = _factory.UI.GetTextBox(id + "_Textbox",
                                              0f, SLIDER_HEIGHT + 1f, panel,
                                              text, config, width: 100f, height: ROW_HEIGHT);
            textbox.RenderLayer = panel.RenderLayer;
            textbox.Z = panel.Z;
            HoverEffect.Add(textbox, Colors.Transparent, Colors.DarkSlateBlue);
            return textbox;
        }

        private void addArrowButtons(string id, IUIControl label, 
                                     INumberEditorComponent numberEditor)
        {
            var icons = _factory.Graphics.Icons;
            var arrowUpIdle = icons.GetArrowIcon(ArrowDirection.Up, Colors.White);
            var arrowDownIdle = icons.GetArrowIcon(ArrowDirection.Down, Colors.White);
            var arrowUpHovered = icons.GetArrowIcon(ArrowDirection.Up, Colors.Black);
            var arrowDownHovered = icons.GetArrowIcon(ArrowDirection.Down, Colors.Black);
            var buttonsPanel = _factory.UI.GetPanel(id + "_ButtonsPanel", 1f, 1f, 0f, label.Y, label.TreeNode.Parent);
            buttonsPanel.RenderLayer = label.RenderLayer;
            buttonsPanel.Tint = Colors.Transparent;
            float halfRowHeight = ROW_HEIGHT / 2f;
            float buttonBottomPadding = 4.5f;
            float betweenButtonsPadding = 1f;
            float buttonHeight = halfRowHeight - betweenButtonsPadding * 2;
            var upButton = _factory.UI.GetButton(id + "_UpButton", new ButtonAnimation(arrowUpIdle, null, Colors.Purple),
                                              new ButtonAnimation(arrowUpHovered, null, Colors.Yellow), new ButtonAnimation(arrowUpIdle, null, Colors.Blue),
                                              0f, buttonBottomPadding + buttonHeight + betweenButtonsPadding, buttonsPanel, width: 20f, height: buttonHeight);
            upButton.RenderLayer = label.RenderLayer;
            upButton.Z = label.Z;

            var downButton = _factory.UI.GetButton(id + "_DownButton", new ButtonAnimation(arrowDownIdle, null, Colors.Purple),
                                                new ButtonAnimation(arrowDownHovered, null, Colors.Yellow), new ButtonAnimation(arrowDownIdle, null, Colors.Blue),
                                                0f, buttonBottomPadding, buttonsPanel, width: 20f, height: buttonHeight);
            downButton.RenderLayer = label.RenderLayer;
            downButton.Z = label.Z;
            numberEditor.UpButton = upButton;
            numberEditor.DownButton = downButton;
        }

        private void addSlider(string id, IUIControl panel, INumberEditorComponent numberEditor)
        {
            var slider = _factory.UI.GetSlider(id + "_Slider", null, null, 0f, 0f, 0f, panel);
            slider.Y = 0f;
            slider.Z = panel.Z - 1f;
            slider.HandleGraphics.Anchor = new PointF(0f, 0.5f);
            slider.Direction = SliderDirection.LeftToRight;
            slider.Graphics.Anchor = new PointF(0f, 0.5f);
            slider.Graphics.Image = new EmptyImage(20f, SLIDER_HEIGHT);
            slider.Graphics.ResetBaseSize(panel.Width, SLIDER_HEIGHT);
            slider.HandleGraphics.Image = new EmptyImage(2f, SLIDER_HEIGHT);
            slider.RenderLayer = slider.Graphics.RenderLayer = slider.HandleGraphics.RenderLayer = panel.RenderLayer;
            HoverEffect.Add(slider.Graphics, Colors.Gray, Colors.LightGray);
            HoverEffect.Add(slider.HandleGraphics, Colors.DarkGray, Colors.WhiteSmoke);

            numberEditor.Slider = slider;

            var sliderColorImage = _factory.UI.GetPanel(id + "_SliderColorImage", slider.HandleGraphics.X, SLIDER_HEIGHT, 0f, 0f, slider);
            sliderColorImage.RenderLayer = slider.RenderLayer;
            sliderColorImage.ClickThrough = true;
            sliderColorImage.Z = slider.Graphics.Z - 1f;
            sliderColorImage.Tint = Colors.Purple;
            sliderColorImage.Anchor = slider.Graphics.Anchor;
            slider.HandleGraphics.OnLocationChanged.Subscribe(() => 
            {
                sliderColorImage.Image = new EmptyImage(slider.HandleGraphics.X, SLIDER_HEIGHT);
            });
            var uiEvents = slider.Graphics.GetComponent<IUIEvents>();
            uiEvents.MouseEnter.Subscribe(_ => sliderColorImage.Tint = Colors.MediumPurple);
            uiEvents.MouseLeave.Subscribe(_ => sliderColorImage.Tint = Colors.Purple);
        }
    }
}
