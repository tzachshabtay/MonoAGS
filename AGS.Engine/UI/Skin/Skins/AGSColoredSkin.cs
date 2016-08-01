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

        public Color ButtonIdleBackColor { get; set; }
        public Color ButtonHoverBackColor { get; set; }
        public Color ButtonPushedBackColor { get; set; }
        public IBorderStyle ButtonBorderStyle { get; set; }
        public ITextConfig TextConfig { get; set; }
        public Color TextBoxBackColor { get; set; }
        public IBorderStyle TextBoxBorderStyle { get; set; }

        public ISkin CreateSkin()
        {
            AGSSkin skin = new AGSSkin();

            skin.AddRule<IButtonComponent>(button => 
            {               
                button.IdleAnimation = new AGSSingleFrameAnimation(new EmptyImage(100f, 50f), _factory);
                button.IdleAnimation.Sprite.Tint = ButtonIdleBackColor;

                button.HoverAnimation = new AGSSingleFrameAnimation(new EmptyImage(100f, 50f), _factory);
                button.HoverAnimation.Sprite.Tint = ButtonHoverBackColor;

                button.PushedAnimation = new AGSSingleFrameAnimation(new EmptyImage(100f, 50f), _factory);
                button.PushedAnimation.Sprite.Tint = ButtonPushedBackColor;                                                
            });

            skin.AddRule<IButtonComponent>(entity =>
            {
                var animationContainer = entity.GetComponent<IAnimationContainer>();
                if (animationContainer == null) return;
                animationContainer.Border = ButtonBorderStyle;
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

            skin.AddRule<ITextComponent>(text =>
            {
                var textConfig = TextConfig;
                if (textConfig == null) return;
                text.TextConfig = textConfig;
            });

            skin.AddRule<ITextBoxComponent>(entity =>
            {
                var animationContainer = entity.GetComponent<IAnimationContainer>();
                if (animationContainer == null) return;
                animationContainer.Tint = TextBoxBackColor;
                animationContainer.Border = TextBoxBorderStyle;
            });

            return skin;
        }
    }
}
