using AGS.API;

namespace AGS.Engine
{
    public class AGSColoredSkin
    {
        private IGraphicsFactory _factory;

        public AGSColoredSkin(IGraphicsFactory factory)
        {
            _factory = factory;
            ButtonIdleBackColor = Colors.DimGray;
            ButtonHoverBackColor = Colors.LightGray;
            ButtonPushedBackColor = Colors.LightYellow;
            ButtonBorderStyle = AGSBorders.SolidColor(Colors.Black, 1f);        
        }

        public Color ButtonIdleBackColor { get; set; }
        public Color ButtonHoverBackColor { get; set; }
        public Color ButtonPushedBackColor { get; set; }
        public IBorderStyle ButtonBorderStyle { get; set; }
        public ITextConfig TextConfig { get; set; }

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
                textComponent.Text = ">";
            });

            skin.AddRule<ITextComponent>(text =>
            {
                var textConfig = TextConfig;
                if (textConfig == null) return;
                text.TextConfig = textConfig;
            });            

            return skin;
        }
    }
}
