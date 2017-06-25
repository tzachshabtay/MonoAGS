using AGS.API;

namespace AGS.Engine
{
    public class AGSComboBoxComponent : AGSComponent, IComboBoxComponent
    {
        private ITextBox _textBox;
        private IButton _dropDownButton;
        private IListboxComponent _dropDownPanelList;
        private IVisibleComponent _dropDownPanelVisible;
        private IDrawableInfo _dropDownPanelDrawable;
        private IEntity _dropDownPanel;

        public AGSComboBoxComponent(IGameEvents gameEvents)
        {
            gameEvents.OnRepeatedlyExecute.Subscribe((_, __) => refreshDropDownLayout());
        }

        public IButton DropDownButton
        {
            get { return _dropDownButton; }
            set
            {
                if (_dropDownButton == value) return;
                var oldDropDownButton = _dropDownButton;
                var newDropDownButton = value;
                if (oldDropDownButton != null) oldDropDownButton.MouseClicked.Unsubscribe(onDropDownClicked);
                _dropDownButton = newDropDownButton;
                if (newDropDownButton != null)
                {
                    if (_dropDownPanelDrawable != null)
                    {
                        _dropDownPanelDrawable.RenderLayer = value.RenderLayer;
                    }
                    newDropDownButton.MouseClicked.Subscribe(onDropDownClicked);
                }
            }
        }

        public ITextBox TextBox
        {
            get { return _textBox; }
            set { _textBox = value; }
        }

        public IListboxComponent DropDownPanelList { get { return _dropDownPanelList; } }

        public IEntity DropDownPanel 
        { 
            get { return _dropDownPanel; }
            set 
            {
                _dropDownPanel = value;
                var visibleComponent = value.GetComponent<IVisibleComponent>();
                var drawableComponent = value.GetComponent<IDrawableInfo>();
                var listBoxComponent = value.GetComponent<IListboxComponent>();
                var imageComponent = value.GetComponent<IImageComponent>();
                var translateComponent = value.GetComponent<ITranslateComponent>();
                _dropDownPanelVisible = visibleComponent;
                _dropDownPanelDrawable = drawableComponent;

                var oldPanel = _dropDownPanelList;
                if (oldPanel != null) oldPanel.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
                _dropDownPanelList = listBoxComponent;
                if (listBoxComponent != null) listBoxComponent.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
                if (imageComponent != null) imageComponent.Anchor = new PointF(0f, 1f);
                if (translateComponent != null) translateComponent.Y = -1f;
                if (visibleComponent != null) visibleComponent.Visible = false;
                if (drawableComponent != null && DropDownButton != null) 
                    drawableComponent.RenderLayer = DropDownButton.RenderLayer;
            }
        }

        public void SetDropDownPanel(IListboxComponent listBoxComponent, IVisibleComponent visibleComponent,
                                     IDrawableInfo drawableComponent, IImageComponent imageComponent,
                                     ITranslateComponent translateComponent)
        {
            _dropDownPanelVisible = visibleComponent;
            _dropDownPanelDrawable = drawableComponent;

            var oldPanel = _dropDownPanelList;
            if (oldPanel != null) oldPanel.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
            _dropDownPanelList = listBoxComponent;
            if (listBoxComponent != null) listBoxComponent.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
            if (imageComponent != null) imageComponent.Anchor = new PointF(0f, 1f);
            if (translateComponent != null) translateComponent.Y = -1f;
            if (visibleComponent != null) visibleComponent.Visible = false;
            if (drawableComponent != null && DropDownButton != null) 
                drawableComponent.RenderLayer = DropDownButton.RenderLayer;
        }

        private void onSelectedItemChanged(object sender, ListboxItemArgs args)
        {
            var textBox = TextBox;
            if (textBox != null) textBox.Text = args.Item == null ? "" : args.Item.Text;
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = false;
        }

        private void refreshDropDownLayout()
        {
            var textbox = TextBox;
            var dropDownButton = DropDownButton;
            if (dropDownButton == null) return;
            if (textbox == null)
            {
                dropDownButton.X = 0;
            }
            else 
            {
                float dropBorderLeft = 0f;
                if (dropDownButton.Border != null) dropBorderLeft = dropDownButton.Border.WidthLeft;
                dropDownButton.X = textbox.X + (textbox.Width < 0 ? textbox.TextWidth : textbox.Width) + dropBorderLeft;
            }
        }

        private void onDropDownClicked(object sender, MouseButtonEventArgs args)
        {
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = !_dropDownPanelVisible.Visible;
        }
    }
}
