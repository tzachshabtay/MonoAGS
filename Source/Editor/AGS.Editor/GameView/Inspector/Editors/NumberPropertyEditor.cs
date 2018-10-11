using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;
using System.Reflection;
using System.ComponentModel;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class NumberPropertyEditor : IEditorSupportsNulls
    {
        private readonly IGameFactory _factory;
        private readonly bool _wholeNumbers;
        private readonly List<InternalNumberEditor> _internalEditors;
        private readonly IGameState _state;
        private readonly ActionManager _actions;
        private readonly StateModel _model; 
        private List<(IObject control, INumberEditorComponent editor)> _panels;
        private IProperty _property;
        private const float SLIDER_HEIGHT = 5f;
        private const float ROW_HEIGHT = 20f;

        public class InternalNumberEditor
        {
            public InternalNumberEditor(string text, Func<IProperty, string> getValueString,
                                        Action<IProperty, float, bool> setValue,
                                        Action<InternalNumberEditor, INumberEditorComponent> configureNumberEditor)
            {
                Text = text;
                GetValueString = getValueString;
                SetValue = setValue;
                ConfigureNumberEditor = (prop, editor) =>
                {
                    var slider = prop.GetAttribute<NumberEditorSliderAttribute>();
                    if (slider != null)
                    {
                        editor.SuggestedMinValue = slider.SliderMin;
                        editor.SuggestedMaxValue = slider.SliderMax;
                        editor.Step = slider.Step;
                    }
                    else configureNumberEditor?.Invoke(this, editor);
                };
            }

            public Func<IProperty, string> GetValueString { get; private set; }
            public Action<IProperty, float, bool> SetValue { get; private set; }
            public Action<IProperty, INumberEditorComponent> ConfigureNumberEditor { get; private set; }
            public string Text { get; private set; }
        }

        public NumberPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model,
                                    bool wholeNumbers, List<InternalNumberEditor> internalEditors = null)
        {
            _actions = actions;
            _state = state;
            _factory = factory;
            _wholeNumbers = wholeNumbers;
            _model = model;
            _internalEditors = internalEditors ?? new List<InternalNumberEditor>
            {
                new InternalNumberEditor(null, prop => prop.ValueString, (prop, value, userInitiated) =>
                {
                    if (_actions.ActionIsExecuting) return;
                    if (wholeNumbers) setValue(prop, (int)Math.Round(value), userInitiated);
                    else setValue(prop, value, userInitiated);
                }, null)
            };
            _panels = new List<(IObject, INumberEditorComponent)>(_internalEditors.Count);
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            var panels = new List<(IObject control, INumberEditorComponent editor)>(_internalEditors.Count);
            for (int i = 0; i < _internalEditors.Count; i++)
            {
                var editor = _internalEditors[i];
                var panel = addEditor(id + i, view, property, editor);
                panels.Add(panel);
            }
            _panels = panels;
        }

        public void RefreshUI()
        {
            if (_property == null) return;
            OnNullChanged(false);
        }

        public void OnNullChanged(bool isNull)
        {
            for (int i = 0; i < _panels.Count; i++)
            {
                var panel = _panels[i];
                var editor = _internalEditors[i];
                if (!isNull)
                {
                    var text = editor.GetValueString(_property);
                    panel.editor.Value = float.Parse(text);
                    panel.control.Visible = true;
                }
                else panel.control.Visible = false;
            }
        }

        private void setValue(IProperty property, float value, bool userInitiated)
        {
            object val = value;
            if (_wholeNumbers)
            {
                int valInt = (int)value;
                val = valInt;
            }
            if (userInitiated) _actions.RecordAction(new PropertyAction(property, val, _model));
            else property.Value = new ValueModel(val);
        }

        private (IObject control, INumberEditorComponent editor) addEditor(string id, ITreeNodeView view, IProperty property, InternalNumberEditor editor)
        {
            var label = view.TreeItem;
            var panel = _factory.UI.GetPanel(id + "EditPanel", 100f, ROW_HEIGHT, label.X, label.Y, label.TreeNode.Parent);
            panel.RenderLayer = label.RenderLayer;
            panel.Tint = Colors.Transparent;
            panel.Z = label.Z;
            float x = 0f;
            if (editor.Text != null)
            {
                var propLabel = _factory.UI.GetLabel(id + "_PropLabel", editor.Text, 1f, 1f, 0f, 0f, panel,
                             _factory.Fonts.GetTextConfig(paddingTop: 0f, paddingBottom: 0f, autoFit: AutoFit.LabelShouldFitText));
                propLabel.Tint = Colors.Transparent;
                propLabel.TextBackgroundVisible = false;
                propLabel.RenderLayer = label.RenderLayer;
                propLabel.PrepareTextBoundingBoxes();
                x += propLabel.TextWidth + 3f;
            }
            var textPanel = _factory.UI.GetPanel(id + "_TextPanel", 100f, ROW_HEIGHT, x, 0f, panel);
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
            Action<NumberValueChangedArgs> onValueChanged = (args =>
            {
                editor.SetValue(property, numberEditor.Value, args.UserInitiated);
            });
            numberEditor.OnValueChanged.Subscribe(onValueChanged);
            panel.OnDisposed(() => numberEditor.OnValueChanged.Unsubscribe(onValueChanged));
            x += textbox.Width + 5f;
            addArrowButtons(id, panel, numberEditor, x);
            editor.ConfigureNumberEditor(property, numberEditor);
            return (panel, numberEditor);
        }

        private ITextBox addTextBox(string id, IUIControl panel, string text)
        {
            var config = GameViewColors.TextboxTextConfig;
            var textbox = _factory.UI.GetTextBox(id + "_Textbox",
                                              0f, SLIDER_HEIGHT + 1f, panel,
                                              "", config, width: 100f, height: ROW_HEIGHT);
            textbox.Text = text;
            textbox.RenderLayer = panel.RenderLayer;
            textbox.Z = panel.Z;
            textbox.TextBackgroundVisible = false;
            GameViewColors.AddHoverEffect(textbox);
            return textbox;
        }

        private void addArrowButtons(string id, IObject panel,
                                     INumberEditorComponent numberEditor, float x)
        {
            var textConfig = FontIcons.TinyButtonConfig;
            var hoveredConfig = AGSTextConfig.ChangeColor(textConfig, Colors.Black, Colors.White, 0f);

            var idle = new ButtonAnimation(null, textConfig, Colors.Purple);
            var hover = new ButtonAnimation(null, hoveredConfig, Colors.Yellow);
            var pushed = new ButtonAnimation(null, textConfig, Colors.Blue);

            var buttonsPanel = _factory.UI.GetPanel(id + "_ButtonsPanel", 1f, 1f, 0f, 0f, panel);
            buttonsPanel.RenderLayer = panel.RenderLayer;
            buttonsPanel.Tint = Colors.Transparent;
            float halfRowHeight = ROW_HEIGHT / 2f;
            float buttonBottomPadding = 1f;
            float betweenButtonsPadding = 2f;
            float buttonHeight = halfRowHeight + 6f - betweenButtonsPadding * 2;

            var upButton = _factory.UI.GetButton(id + "_UpButton", idle, hover, pushed, x, buttonBottomPadding + buttonHeight + betweenButtonsPadding, 
                                                 buttonsPanel, FontIcons.CaretUp, width: 15f, height: buttonHeight);
            upButton.RenderLayer = panel.RenderLayer;
            upButton.Z = panel.Z;

            var downButton = _factory.UI.GetButton(id + "_DownButton", idle, hover, pushed, x, buttonBottomPadding, 
                                                   buttonsPanel, FontIcons.CaretDown, width: 15f, height: buttonHeight);
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
        public MultipleNumbersPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model,
                                             bool wholeNumbers, Action<InternalNumberEditor, INumberEditorComponent> configureNumberEditor,
                                             params (string text, Func<float, T, T> getValue)[] creators) :
        base(actions, state, factory, model, wholeNumbers, creators.Select((creator, index) =>
            new InternalNumberEditor(creator.text, prop => prop.ValueString == InspectorProperty.NullValue ?
                                     InspectorProperty.NullValue : prop.ValueString.Replace("(", "").Replace(")", "").Split(',')[index],
                                     (prop, value, userInitiated) =>
            {
                if (actions.ActionIsExecuting) return;
                object objVal = prop.Value.Value;
                T val = objVal == null ? default : (T)objVal;
                if (userInitiated) actions.RecordAction(new PropertyAction(prop, creator.getValue(value, val), model));
                else prop.Value = new ValueModel(creator.getValue(value, val));
            }, configureNumberEditor)
        ).ToList()){}
    }

    public class SizeFPropertyEditor : MultipleNumbersPropertyEditor<SizeF>
    {
        public SizeFPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model)
            : base(actions, state, factory, model, false, null,
                           ("Width", (width, size) => new SizeF(width, size.Height)),
                           ("Height", (height, size) => new SizeF(size.Width, height))){}
    }

    public class SizePropertyEditor : MultipleNumbersPropertyEditor<Size>
    {
        public SizePropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, true, null,
                   ("Width", (width, size) => new Size((int)Math.Round(width), size.Height)),
                   ("Height", (height, size) => new Size(size.Width, (int)Math.Round(height)))){}
    }

    public class PointFPropertyEditor : MultipleNumbersPropertyEditor<PointF>
    {
        public PointFPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, false, null,
                           ("X", (x, point) => new PointF(x, point.Y)),
                           ("Y", (y, point) => new PointF(point.X, y)))
        { }
    }

    public class PointPropertyEditor : MultipleNumbersPropertyEditor<Point>
    {
        public PointPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, true, null,
                   ("X", (x, point) => new Point((int)Math.Round(x), point.Y)),
                   ("Y", (y, point) => new Point(point.X, (int)Math.Round(y))))
        { }
    }

    public class Vector2PropertyEditor : MultipleNumbersPropertyEditor<Vector2>
    {
        public Vector2PropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, false, null,
                           ("X", (x, vector) => new Vector2(x, vector.Y)),
                           ("Y", (y, vector) => new Vector2(vector.X, y)))
        { }
    }

    public class Vector3PropertyEditor : MultipleNumbersPropertyEditor<Vector3>
    {
        public Vector3PropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, false, null,
                           ("X", (x, vector) => new Vector3(x, vector.Y, vector.Z)),
                           ("Y", (y, vector) => new Vector3(vector.X, y, vector.Z)),
                           ("Z", (z, vector) => new Vector3(vector.X, vector.Y, z)))
        { }
    }

    public class Vector4PropertyEditor : MultipleNumbersPropertyEditor<Vector4>
    {
        public Vector4PropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, false, null,
                           ("X", (x, vector) => new Vector4(x, vector.Y, vector.Z, vector.W)),
                           ("Y", (y, vector) => new Vector4(vector.X, y, vector.Z, vector.W)),
                           ("Z", (z, vector) => new Vector4(vector.X, vector.Y, z, vector.W)),
                           ("W", (w, vector) => new Vector4(vector.X, vector.Y, vector.Z, w)))
        { }
    }

    public class LocationPropertyEditor : MultipleNumbersPropertyEditor<Position>
    {
        public LocationPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model, IGameSettings settings, IDrawableInfoComponent drawable) 
            : base(actions, state, factory, model, false,
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
                           ("X", (x, vector) => new Position(x, vector.Y, vector.Z)),
                           ("Y", (y, vector) => new Position(vector.X, y, vector.Z)),
                           ("Z", (z, vector) => new Position(vector.X, vector.Y, z)))
        { }
    }

    public class RectangleFPropertyEditor : MultipleNumbersPropertyEditor<RectangleF>
    {
        public RectangleFPropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, false, null,
                           ("X", (x, rect) => new RectangleF(x, rect.Y, rect.Width, rect.Height)),
                           ("Y", (y, rect) => new RectangleF(rect.X, y, rect.Width, rect.Height)),
                           ("Width", (w, rect) => new RectangleF(rect.X, rect.Y, w, rect.Height)),
                           ("Height", (h, rect) => new RectangleF(rect.X, rect.Y, rect.Width, h)))
        { }
    }

    public class RectanglePropertyEditor : MultipleNumbersPropertyEditor<Rectangle>
    {
        public RectanglePropertyEditor(ActionManager actions, IGameState state, IGameFactory factory, StateModel model) 
            : base(actions, state, factory, model, true, null,
                           ("X", (x, rect) => new Rectangle((int)Math.Round(x), rect.Y, rect.Width, rect.Height)),
                           ("Y", (y, rect) => new Rectangle(rect.X, (int)Math.Round(y), rect.Width, rect.Height)),
                           ("Width", (w, rect) => new Rectangle(rect.X, rect.Y, (int)Math.Round(w), rect.Height)),
                           ("Height", (h, rect) => new Rectangle(rect.X, rect.Y, rect.Width, (int)Math.Round(h))))
        { }
    }
}