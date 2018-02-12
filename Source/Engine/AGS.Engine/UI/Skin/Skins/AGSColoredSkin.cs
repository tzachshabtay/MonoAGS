﻿using AGS.API;

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

        private ButtonAnimation getAnimation(IImageComponent container, ButtonAnimation animation)
        {
            if (animation.Border == null || container == null || container.Border == null) return animation;
            ButtonAnimation newAnimation = new ButtonAnimation(AGSBorders.Multiple(container.Border, animation.Border),
                                                               animation.TextConfig, animation.Tint);
            newAnimation.Animation = animation.Animation;
            return newAnimation;
        }

        public ISkin CreateSkin()
        {
            AGSSkin skin = new AGSSkin();

            skin.AddRule(entity => entity.GetComponent<IButtonComponent>() != null, entity => 
            {
                var image = entity.GetComponent<IImageComponent>();
                var button = entity.GetComponent<IButtonComponent>();
                button.IdleAnimation = getAnimation(image, ButtonIdleAnimation);
                button.HoverAnimation = getAnimation(image, ButtonHoverAnimation);
                button.PushedAnimation = getAnimation(image, ButtonPushedAnimation);
            });

            skin.AddRule(entity => entity.GetComponent<ICheckboxComponent>() != null, entity => 
            {
                var image = entity.GetComponent<IImageComponent>();
                var button = entity.GetComponent<ICheckboxComponent>();
                button.NotCheckedAnimation = getAnimation(image, CheckboxNotCheckedAnimation);
                button.CheckedAnimation = getAnimation(image, CheckboxCheckedAnimation);
                button.HoverCheckedAnimation = getAnimation(image, CheckboxHoverCheckedAnimation);
                button.HoverNotCheckedAnimation = getAnimation(image, CheckboxHoverNotCheckedAnimation);
            });

            skin.AddRule(entity =>
            {
                var skinComponent = entity.GetComponent<ISkinComponent>();
                return skinComponent != null && skinComponent.SkinTags.Contains(AGSSkin.DropDownButtonTag);
            }, entity => 
            {
                var textComponent = entity.GetComponent<ITextComponent>();
                textComponent.Text = "\u25BE";//Unicode for down arrow. Another option is "\u25BC";
            });

            skin.AddRule(entity =>
            {
                var skinComponent = entity.GetComponent<ISkinComponent>();
                return skinComponent != null && skinComponent.SkinTags.Contains(AGSSkin.DialogBoxTag);
            }, entity =>
            {
                var imageComponent = entity.GetComponent<IImageComponent>();
                if (imageComponent != null)
                {
                    imageComponent.Tint = DialogBoxColor;
                    imageComponent.Border = DialogBoxBorder;
                }
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
                if (image == null) return;
                image.Tint = TextBoxBackColor;
                image.Border = TextBoxBorderStyle;
            });

            return skin;
        }
    }
}
