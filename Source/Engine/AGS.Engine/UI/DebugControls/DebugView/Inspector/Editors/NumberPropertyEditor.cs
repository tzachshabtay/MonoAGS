using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;
using System.Reflection;
using System.ComponentModel;

namespace AGS.Engine
{
    public class NumberPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private readonly bool _wholeNumbers, _nullable;
        private readonly List<InternalNumberEditor> _internalEditors;
        private List<(IUIControl control, INumberEditorComponent editor)> _panels;
        private ICheckboxComponent _nullBox;
        private InspectorProperty _property;
        private const float SLIDER_HEIGHT = 5f;
        private const float ROW_HEIGHT = 20f;

        public class InternalNumberEditor
        {
            public InternalNumberEditor(string text, Func<InspectorProperty, string> getValueString,
                                        Action<InspectorProperty, float> setValue, Action<InternalNumberEditor, INumberEditorComponent> configureNumberEditor)
            {
                Text = text;
                GetValueString = getValueString;
                SetValue = setValue;
                ConfigureNumberEditor = (prop, editor) =>
                {
                    var slider = prop.Prop.GetCustomAttribute<NumberEditorSliderAttribute>();
                    if (slider != null)
                    {
                        editor.SuggestedMinValue = slider.SliderMin;
                        editor.SuggestedMaxValue = slider.SliderMax;
                    }
                    else configureNumberEditor?.Invoke(this, editor);
                };
            }

            public Func<InspectorProperty, string> GetValueString { get; private set; }
            public Action<InspectorProperty, float> SetValue { get; private set; }
            public Action<InspectorProperty, INumberEditorComponent> ConfigureNumberEditor { get; private set; }
            public string Text { get; private set; }
        }

        public NumberPropertyEditor(IGameFactory factory, bool wholeNumbers, bool nullable, List<InternalNumberEditor> internalEditors = null)
        {
            _factory = factory;
            _wholeNumbers = wholeNumbers;
            _nullable = nullable;
            _internalEditors = internalEditors ?? new List<InternalNumberEditor>
            {
                new InternalNumberEditor(null, prop => prop.Value, (prop, value) =>
                {
                    if (_wholeNumbers)
                    {
                        prop.Prop.SetValue(prop.Object, (int)Math.Round(value));
                    }
                    else prop.Prop.SetValue(prop.Object, value);
                }, null)
            };
            _panels = new List<(IUIControl, INumberEditorComponent)>(_internalEditors.Count);
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
            ICheckBox nullBox = null;
            if (_nullable)
            {
                nullBox = BoolPropertyEditor.CreateCheckbox(view.TreeItem, _factory, id + "_NullBox");
                _nullBox = nullBox;
                nullBox.Checked = (property.Value != InspectorProperty.NullValue);
            }
            var panels = new List<(IUIControl control, INumberEditorComponent editor)>(_internalEditors.Count);
            for (int i = 0; i < _internalEditors.Count; i++)
            {
                var editor = _internalEditors[i];
                var panel = addEditor(id + i, view, property, editor);
                panel.control.Visible = nullBox == null ? true : nullBox.Checked;
                panels.Add(panel);
            }
            _panels = panels;

            nullBox?.OnCheckChanged.Subscribe(args =>
            {
                if (!args.Checked)
                {
                    property.Prop.SetValue(property.Object, null);
                }
                foreach (var panel in panels)
                {
                    panel.control.Visible = nullBox.Checked;
                    if (args.Checked)
                    {
                        float val = default;
                        panel.editor.Value = val;
                    }
                }
            });
        }

        public void RefreshUI()
        {
            if (_property == null) return;
            if (_nullBox != null)
            {
                _nullBox.Checked = (_property.Value != InspectorProperty.NullValue);
            }

            for (int i = 0; i < _panels.Count(); i++)
            {
                var panel = _panels[i];
                var editor = _internalEditors[i];
                var text = editor.GetValueString(_property);
                if (text != InspectorProperty.NullValue)
                {
                    panel.editor.Value = float.Parse(text);
                    panel.control.Visible = true;
                }
                else panel.control.Visible = false;
            }
        }

        private (IUIControl control, INumberEditorComponent editor) addEditor(string id, ITreeNodeView view, InspectorProperty property, InternalNumberEditor editor)
        {
            var label = view.TreeItem;
            var panel = _factory.UI.GetPanel(id + "EditPanel", 100f, ROW_HEIGHT, label.X, label.Y, label.TreeNode.Parent);
            panel.RenderLayer = label.RenderLayer;
            panel.Tint = Colors.Transparent;
            panel.Z = label.Z;
            var layout = panel.AddComponent<IStackLayoutComponent>();
            layout.Direction = LayoutDirection.Horizontal;
            layout.RelativeSpacing = 1f;
            layout.StartLayout();
            if (editor.Text != null)
            {
                var propLabel = _factory.UI.GetLabel(id + "_PropLabel", editor.Text, 1f, 1f, 0f, 0f, panel, 
                                     new AGSTextConfig(paddingTop: 0f, paddingBottom: 0f, autoFit: AutoFit.LabelShouldFitText));
                propLabel.Tint = Colors.Transparent;
                propLabel.RenderLayer = label.RenderLayer;
            }
            var textPanel = _factory.UI.GetPanel(id + "_TextPanel", 100f, ROW_HEIGHT, 0f, 0f, panel);
            textPanel.RenderLayer = label.RenderLayer;
            textPanel.Z = label.Z;
            textPanel.Tint = Colors.Transparent;
            var text = editor.GetValueString(property);
            var textbox = addTextBox(id, textPanel, text);
            var numberEditor = textbox.AddComponent<INumberEditorComponent>();
            addSlider(id, textPanel, numberEditor);
            numberEditor.EditWholeNumbersOnly = _wholeNumbers;
            if (text != InspectorProperty.NullValue)
            {
                numberEditor.Value = float.Parse(text);
            }
            numberEditor.OnValueChanged.Subscribe(() =>
            {
                editor.SetValue(property, numberEditor.Value);
            });
            addArrowButtons(id, panel, numberEditor);
            editor.ConfigureNumberEditor(property, numberEditor);
            return (panel, numberEditor);
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

        private void addArrowButtons(string id, IUIControl panel,
                                     INumberEditorComponent numberEditor)
        {
            var icons = _factory.Graphics.Icons;
            var arrowUpIdle = icons.GetArrowIcon(ArrowDirection.Up, Colors.White);
            var arrowDownIdle = icons.GetArrowIcon(ArrowDirection.Down, Colors.White);
            var arrowUpHovered = icons.GetArrowIcon(ArrowDirection.Up, Colors.Black);
            var arrowDownHovered = icons.GetArrowIcon(ArrowDirection.Down, Colors.Black);
            var buttonsPanel = _factory.UI.GetPanel(id + "_ButtonsPanel", 1f, 1f, 0f, 0f, panel);
            buttonsPanel.RenderLayer = panel.RenderLayer;
            buttonsPanel.Tint = Colors.Transparent;
            float halfRowHeight = ROW_HEIGHT / 2f;
            float buttonBottomPadding = 12f;
            float betweenButtonsPadding = 1f;
            float buttonHeight = halfRowHeight - betweenButtonsPadding * 2;
            var upButton = _factory.UI.GetButton(id + "_UpButton", new ButtonAnimation(arrowUpIdle, null, Colors.Purple),
                                              new ButtonAnimation(arrowUpHovered, null, Colors.Yellow), new ButtonAnimation(arrowUpIdle, null, Colors.Blue),
                                              0f, buttonBottomPadding + buttonHeight + betweenButtonsPadding, buttonsPanel, width: 20f, height: buttonHeight);
            upButton.RenderLayer = panel.RenderLayer;
            upButton.Z = panel.Z;

            var downButton = _factory.UI.GetButton(id + "_DownButton", new ButtonAnimation(arrowDownIdle, null, Colors.Purple),
                                                new ButtonAnimation(arrowDownHovered, null, Colors.Yellow), new ButtonAnimation(arrowDownIdle, null, Colors.Blue),
                                                0f, buttonBottomPadding, buttonsPanel, width: 20f, height: buttonHeight);
            downButton.RenderLayer = panel.RenderLayer;
            downButton.Z = panel.Z;
            numberEditor.UpButton = upButton;
            numberEditor.DownButton = downButton;
        }

        private void addSlider(string id, IUIControl panel, INumberEditorComponent numberEditor)
        {
            var slider = _factory.UI.GetSlider(id + "_Slider", null, null, 0f, 0f, 0f, panel);
            slider.Y = 0f;
            slider.Z = panel.Z - 1f;
            slider.HandleGraphics.Pivot = new PointF(0f, 0.5f);
            slider.Direction = SliderDirection.LeftToRight;
            slider.Graphics.Pivot = new PointF(0f, 0.5f);
            slider.Graphics.Image = new EmptyImage(20f, SLIDER_HEIGHT);
            slider.Graphics.BaseSize = new SizeF(panel.Width, SLIDER_HEIGHT);
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
            sliderColorImage.Pivot = slider.Graphics.Pivot;
            PropertyChangedEventHandler onHandleLocationChanged = (_, args) =>
            {
                if (args.PropertyName != nameof(ITranslateComponent.X)) return;
                sliderColorImage.Image = new EmptyImage(slider.HandleGraphics.X, SLIDER_HEIGHT);
            };
            slider.HandleGraphics.Bind<ITranslateComponent>(c => c.PropertyChanged += onHandleLocationChanged,
                                                            c => c.PropertyChanged -= onHandleLocationChanged);
            var uiEvents = slider.Graphics.GetComponent<IUIEvents>();
            uiEvents.MouseEnter.Subscribe(_ => sliderColorImage.Tint = Colors.MediumPurple);
            uiEvents.MouseLeave.Subscribe(_ => sliderColorImage.Tint = Colors.Purple);
        }
    }

    public class MultipleNumbersPropertyEditor<T> : NumberPropertyEditor
    {
        public MultipleNumbersPropertyEditor(IGameFactory factory, bool wholeNumbers, bool nullable,
                                             Action<InternalNumberEditor, INumberEditorComponent> configureNumberEditor, 
                                             params (string text, Func<float, T, T> getValue)[] creators) :
        base(factory, wholeNumbers, nullable, creators.Select((creator, index) => 
            new InternalNumberEditor(creator.text, prop => prop.Value == InspectorProperty.NullValue ?
                                     InspectorProperty.NullValue : prop.Value.Split(',')[index],
                                     (prop, value) =>
            {
                object objVal = prop.Prop.GetValue(prop.Object);
                T val = objVal == null ? default : (T)objVal;
                prop.Prop.SetValue(prop.Object, creator.getValue(value, val));
            }, configureNumberEditor)
        ).ToList()){} 
    }

    public class SizeFPropertyEditor : MultipleNumbersPropertyEditor<SizeF>
    {
        public SizeFPropertyEditor(IGameFactory factory, bool nullable): base(factory, false, nullable, null,
                           ("Width", (width, size) => new SizeF(width, size.Height)),
                           ("Height", (height, size) => new SizeF(size.Width, height))){}
    }

    public class SizePropertyEditor : MultipleNumbersPropertyEditor<Size>
    {
        public SizePropertyEditor(IGameFactory factory, bool nullable) : base(factory, true, nullable, null,
                   ("Width", (width, size) => new Size((int)Math.Round(width), size.Height)),
                   ("Height", (height, size) => new Size(size.Width, (int)Math.Round(height)))){}
    }

    public class PointFPropertyEditor : MultipleNumbersPropertyEditor<PointF>
    {
        public PointFPropertyEditor(IGameFactory factory, bool nullable) : base(factory, false, nullable, null,
                           ("X", (x, point) => new PointF(x, point.Y)),
                           ("Y", (y, point) => new PointF(point.X, y)))
        { }
    }

    public class PointPropertyEditor : MultipleNumbersPropertyEditor<Point>
    {
        public PointPropertyEditor(IGameFactory factory, bool nullable) : base(factory, true, nullable, null,
                   ("X", (x, point) => new Point((int)Math.Round(x), point.Y)),
                   ("Y", (y, point) => new Point(point.X, (int)Math.Round(y))))
        { }
    }

    public class Vector2PropertyEditor : MultipleNumbersPropertyEditor<Vector2>
    {
        public Vector2PropertyEditor(IGameFactory factory, bool nullable) : base(factory, false, nullable, null,
                           ("X", (x, vector) => new Vector2(x, vector.Y)),
                           ("Y", (y, vector) => new Vector2(vector.X, y)))
        { }
    }

    public class LocationPropertyEditor : MultipleNumbersPropertyEditor<ILocation>
    {
        public LocationPropertyEditor(IGameFactory factory, bool nullable, IGameSettings settings, IDrawableInfoComponent drawable) : base(factory, false, nullable, 
                            (internalEditor, editor) => 
                            {
                                if (internalEditor.Text == "X")
                                {
                                    editor.SuggestedMinValue = 0f;
                                    editor.SuggestedMaxValue = drawable == null || drawable.RenderLayer == null || drawable.RenderLayer.IndependentResolution == null ? settings.VirtualResolution.Width : drawable.RenderLayer.IndependentResolution.Value.Width;
                                }
                                else if (internalEditor.Text == "Y")
                                {
                                    editor.SuggestedMinValue = 0f;
                                    editor.SuggestedMaxValue = drawable == null || drawable.RenderLayer == null || drawable.RenderLayer.IndependentResolution == null ? settings.VirtualResolution.Height : drawable.RenderLayer.IndependentResolution.Value.Height;
                                }
                            },
                           ("X", (x, vector) => new AGSLocation(x, vector.Y, vector.Z)),
                           ("Y", (y, vector) => new AGSLocation(vector.X, y, vector.Z)),
                           ("Z", (z, vector) => new AGSLocation(vector.X, vector.Y, z)))
        { }
    }

    public class RectangleFPropertyEditor : MultipleNumbersPropertyEditor<RectangleF>
    {
        public RectangleFPropertyEditor(IGameFactory factory, bool nullable) : base(factory, false, nullable, null,
                           ("X", (x, rect) => new RectangleF(x, rect.Y, rect.Width, rect.Height)),
                           ("Y", (y, rect) => new RectangleF(rect.X, y, rect.Width, rect.Height)),
                           ("Width", (w, rect) => new RectangleF(rect.X, rect.Y, w, rect.Height)),
                           ("Height", (h, rect) => new RectangleF(rect.X, rect.Y, rect.Width, h)))
        { }
    }

    public class RectanglePropertyEditor : MultipleNumbersPropertyEditor<Rectangle>
    {
        public RectanglePropertyEditor(IGameFactory factory, bool nullable) : base(factory, true, nullable, null,
                           ("X", (x, rect) => new Rectangle((int)Math.Round(x), rect.Y, rect.Width, rect.Height)),
                           ("Y", (y, rect) => new Rectangle(rect.X, (int)Math.Round(y), rect.Width, rect.Height)),
                           ("Width", (w, rect) => new Rectangle(rect.X, rect.Y, (int)Math.Round(w), rect.Height)),
                           ("Height", (h, rect) => new Rectangle(rect.X, rect.Y, rect.Width, (int)Math.Round(h))))
        { }
    }
}
