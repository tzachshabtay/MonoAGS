using AGS.API;

namespace AGS.Engine
{
    public class AGSColoredSkin
    {
        private IGraphicsFactory _factory;

        public AGSColoredSkin(IGraphicsFactory factory)
        {
            _factory = factory;            
        }

        public ButtonAnimation ButtonIdleAnimation { get; set; }
        public ButtonAnimation ButtonHoverAnimation { get; set; }
        public ButtonAnimation ButtonPushedAnimation { get; set; }
        public ITextConfig TextConfig { get; set; }
        public Color TextBoxBackColor { get; set; }
        public IBorderStyle TextBoxBorderStyle { get; set; }
        public ButtonAnimation CheckboxNotCheckedAnimation { get; set; }
        public ButtonAnimation CheckboxCheckedAnimation { get; set; }
        public ButtonAnimation CheckboxHoverNotCheckedAnimation { get; set; }
        public ButtonAnimation CheckboxHoverCheckedAnimation { get; set; }
        public Color DialogBoxColor { get; set; }
        public IBorderStyle DialogBoxBorder { get; set; }

        public PointF DefaultItemSize = new PointF(100f, 50f);

        private ButtonAnimation getAnimation(IBorderComponent container, ButtonAnimation animation)
        {
            if (animation.Border == null || container == null || container.Border == null) return animation;
            ButtonAnimation newAnimation = new ButtonAnimation(animation.Animation, animation.Image, _factory.Borders.Multiple(container.Border, animation.Border),
                                                               animation.TextConfig, animation.Tint);
            return newAnimation;
        }

        public ISkin CreateSkin()
        {
            AGSSkin skin = new AGSSkin();

            skin.AddRule(entity => entity.GetComponent<IButtonComponent>() != null, entity => 
            {
                var border = entity.GetComponent<IBorderComponent>();
                var button = entity.GetComponent<IButtonComponent>();
                button.IdleAnimation = getAnimation(border, ButtonIdleAnimation);
                button.HoverAnimation = getAnimation(border, ButtonHoverAnimation);
                button.PushedAnimation = getAnimation(border, ButtonPushedAnimation);
            });

            skin.AddRule(entity => entity.GetComponent<ICheckboxComponent>() != null, entity => 
            {
                var border = entity.GetComponent<IBorderComponent>();
                var button = entity.GetComponent<ICheckboxComponent>();
                button.NotCheckedAnimation = getAnimation(border, CheckboxNotCheckedAnimation);
                button.CheckedAnimation = getAnimation(border, CheckboxCheckedAnimation);
                button.HoverCheckedAnimation = getAnimation(border, CheckboxHoverCheckedAnimation);
                button.HoverNotCheckedAnimation = getAnimation(border, CheckboxHoverNotCheckedAnimation);
            });

            skin.AddRule(entity =>
            {
                var skinComponent = entity.GetComponent<ISkinComponent>();
                return skinComponent != null && skinComponent.SkinTags.Contains(AGSSkin.DropDownButtonTag);
            }, entity => 
            {
                var textComponent = entity.GetComponent<ITextComponent>();
                textComponent.Text = "\u25BE"; //Unicode for down arrow. Another option is "\u25BC";
            });

            skin.AddRule(entity =>
            {
                var skinComponent = entity.GetComponent<ISkinComponent>();
                return skinComponent != null && skinComponent.SkinTags.Contains(AGSSkin.DialogBoxTag);
            }, entity =>
            {
                var imageComponent = entity.GetComponent<IImageComponent>();
                var borderComponent = entity.GetComponent<IBorderComponent>();
                if (imageComponent != null) imageComponent.Tint = DialogBoxColor;
                if (borderComponent != null) borderComponent.Border = DialogBoxBorder;
            });

            skin.AddRule(entity =>
            {
                var skinComponent = entity.GetComponent<ISkinComponent>();
                return skinComponent != null && skinComponent.SkinTags.Contains(AGSSkin.CheckBoxTag);
            }, entity =>
            {
                var checkBoxComponent = entity.GetComponent<ICheckboxComponent>();
                if (checkBoxComponent == null) return;
                var textComponent = entity.GetComponent<ITextComponent>();
                const string checkedStr = "\u2611";
                const string notCheckedStr = "\u2610";
                if (!textComponent.Text.StartsWith(checkedStr) && !textComponent.Text.StartsWith(notCheckedStr))
                {
                    textComponent.Text = (checkBoxComponent.Checked ? checkedStr : notCheckedStr) + textComponent.Text;
                }
                checkBoxComponent.OnCheckChanged.Subscribe(_ => 
                {
                    textComponent.Text = (checkBoxComponent.Checked ? checkedStr : notCheckedStr) + textComponent.Text.Substring(1);
                });                
            });

            skin.AddRule<ITextComponent>(text =>
            {
                var textConfig = TextConfig;
                if (textConfig == null) return;
                text.TextConfig = textConfig;
            });

            skin.AddRule<ITextBoxComponent>(entity =>
            {
                var image = entity.GetComponent<IImageComponent>();
                var border = entity.GetComponent<IBorderComponent>();
                if (image != null) image.Tint = TextBoxBackColor;
                if (border != null) border.Border = TextBoxBorderStyle;
            });

            return skin;
        }
    }
}
