using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSColoredSkin
    {
        private IGraphicsFactory _factory;

        public AGSColoredSkin(IGraphicsFactory factory)
        {
            _factory = factory;            
        }

        public Color ButtonIdleBackColor { get; set; }
        public Color ButtonHoverBackColor { get; set; }
        public Color ButtonPushedBackColor { get; set; }
        public IBorderStyle ButtonBorderStyle { get; set; }
        public ITextConfig TextConfig { get; set; }
        public Color TextBoxBackColor { get; set; }
        public IBorderStyle TextBoxBorderStyle { get; set; }
        public Color CheckboxNotCheckedColor { get; set; }
        public Color CheckboxCheckedColor { get; set; }
        public Color CheckboxHoverNotCheckedColor { get; set; }
        public Color CheckboxHoverCheckedColor { get; set; }
        public IBorderStyle CheckboxBorderStyle { get; set; }

        public PointF DefaultItemSize = new PointF(100f, 50f);

        public ISkin CreateSkin()
        {
            AGSSkin skin = new AGSSkin();

            skin.AddRule<IButtonComponent>(button => 
            {               
                button.IdleAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                button.IdleAnimation.Sprite.Tint = ButtonIdleBackColor;

                button.HoverAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                button.HoverAnimation.Sprite.Tint = ButtonHoverBackColor;

                button.PushedAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                button.PushedAnimation.Sprite.Tint = ButtonPushedBackColor;                                                
            });

            skin.AddRule<IButtonComponent>(entity =>
            {
                var animationContainer = entity.GetComponent<IAnimationContainer>();
                if (animationContainer == null) return;
                animationContainer.Border = ButtonBorderStyle;
            });

            skin.AddRule<ICheckboxComponent>(checkBox =>
            {
                checkBox.NotCheckedAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                checkBox.NotCheckedAnimation.Sprite.Tint = CheckboxNotCheckedColor;
                
                checkBox.CheckedAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                checkBox.CheckedAnimation.Sprite.Tint = CheckboxCheckedColor;
                
                checkBox.HoverNotCheckedAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                checkBox.HoverNotCheckedAnimation.Sprite.Tint = CheckboxHoverNotCheckedColor;

                checkBox.HoverCheckedAnimation = new AGSSingleFrameAnimation(new EmptyImage(DefaultItemSize), _factory);
                checkBox.HoverCheckedAnimation.Sprite.Tint = CheckboxHoverCheckedColor;
            });

            skin.AddRule<ICheckboxComponent>(entity =>
            {
                var animationContainer = entity.GetComponent<IAnimationContainer>();
                if (animationContainer == null) return;
                animationContainer.Border = CheckboxBorderStyle;
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
                checkBoxComponent.OnCheckChanged.Subscribe((sender, args) => 
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
                var animationContainer = entity.GetComponent<IAnimationContainer>();
                var image = entity.GetComponent<IImageComponent>();
                if (animationContainer == null) return;
                image.Tint = TextBoxBackColor;
                animationContainer.Border = TextBoxBorderStyle;
            });

            return skin;
        }
    }
}
