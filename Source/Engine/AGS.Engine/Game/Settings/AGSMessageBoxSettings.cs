﻿using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSMessageBoxSettings : IMessageBoxSettings
    {
        public AGSMessageBoxSettings(IGame game)
        {
            DisplayConfig = getDefaultConfig(game);
            ButtonText = game.Factory.Fonts.GetTextConfig(autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter);
        }

        public IRenderLayer RenderLayer { get; set; } = new AGSRenderLayer(AGSLayers.Speech.Z, independentResolution: new Size(1200, 800));
        public ISayConfig DisplayConfig { get; set; }
        public ITextConfig ButtonText { get; set; }
        public float ButtonXPadding { get; set; } = 10f;
        public float ButtonYPadding { get; set; } = 5f;
        public float ButtonWidth { get; set; } = 60f;
        public float ButtonHeight { get; set; } = 30f;

        public override string ToString() => "Message Box Settings";

        private ISayConfig getDefaultConfig(IGame game)
        {
            AGSSayConfig config = new AGSSayConfig(game);
            config.Border = game.Factory.Graphics.Borders.Gradient(new FourCorners<Color>(Colors.DarkOliveGreen,
                Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
            config.TextConfig = game.Factory.Fonts.GetTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight, alignment: Alignment.TopCenter
                                                   , paddingLeft: 30, paddingTop: 30, paddingBottom: 30, paddingRight: 30);
            var screenWidth = game.Settings.VirtualResolution.Width;
            var screenHeight = game.Settings.VirtualResolution.Height;
            if (RenderLayer.IndependentResolution != null)
            {
                screenWidth = RenderLayer.IndependentResolution.Value.Width;
                screenHeight = RenderLayer.IndependentResolution.Value.Height;
            }
            config.LabelSize = new SizeF(screenWidth * 3 / 4f, screenHeight * 3 / 4f);
            config.BackgroundColor = Colors.Black;
            return config;
        }
    }
}