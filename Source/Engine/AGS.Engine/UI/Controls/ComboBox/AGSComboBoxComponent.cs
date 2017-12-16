using AGS.API;

namespace AGS.Engine
{
    public class AGSComboBoxComponent : AGSComponent, IComboBoxComponent
    {
        private IButton _dropDownButton;
        private IListboxComponent _dropDownPanelList;
        private IVisibleComponent _dropDownPanelVisible;
        private IDrawableInfoComponent _dropDownPanelDrawable;
        private IEntity _dropDownPanel;

        public AGSComboBoxComponent(IGameEvents gameEvents)
        {
            gameEvents.OnRepeatedlyExecute.Subscribe(refreshDropDownLayout);
        }

        public IButton DropDownButton
        {
            get => _dropDownButton;
            set
            {
                if (_dropDownButton == value) return;
                _dropDownButton?.MouseClicked.Unsubscribe(onDropDownClicked);
                _dropDownButton = value;
                _dropDownButton?.MouseClicked.Subscribe(onDropDownClicked);
            }
        }

        public ITextBox TextBox { get; set; }

        public IListboxComponent DropDownPanelList => _dropDownPanelList;

        public IEntity DropDownPanel 
        {
            get => _dropDownPanel;
            set 
            {
                _dropDownPanel = value;
                var visibleComponent = value.GetComponent<IVisibleComponent>();
                var drawableComponent = value.GetComponent<IDrawableInfoComponent>();
                var listBoxComponent = value.GetComponent<IListboxComponent>();
                var imageComponent = value.GetComponent<IImageComponent>();
                var translateComponent = value.GetComponent<ITranslateComponent>();
                _dropDownPanelVisible = visibleComponent;
                _dropDownPanelDrawable = drawableComponent;

                _dropDownPanelList?.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
                _dropDownPanelList = listBoxComponent;
                listBoxComponent?.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
                if (imageComponent != null) imageComponent.Pivot = new PointF(0f, 1f);
                if (translateComponent != null) translateComponent.Y = -1f;
                if (visibleComponent != null) visibleComponent.Visible = false;
            }
        }

        public void SetDropDownPanel(IListboxComponent listBoxComponent, IVisibleComponent visibleComponent,
                                     IDrawableInfoComponent drawableComponent, IImageComponent imageComponent,
                                     ITranslateComponent translateComponent)
        {
            _dropDownPanelVisible = visibleComponent;
            _dropDownPanelDrawable = drawableComponent;

            _dropDownPanelList?.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
            _dropDownPanelList = listBoxComponent;
            listBoxComponent?.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
            if (imageComponent != null) imageComponent.Pivot = new PointF(0f, 1f);
            if (translateComponent != null) translateComponent.Y = -1f;
            if (visibleComponent != null) visibleComponent.Visible = false;
        }

        private void onSelectedItemChanged(ListboxItemArgs args)
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
                float dropBorderLeft = dropDownButton.Border?.WidthLeft ?? 0f;
                dropDownButton.X = textbox.X + (textbox.Width < 0 ? textbox.TextWidth : textbox.Width) + dropBorderLeft;
            }
        }

        private void onDropDownClicked(MouseButtonEventArgs args)
        {
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = !_dropDownPanelVisible.Visible;
        }
    }
}
