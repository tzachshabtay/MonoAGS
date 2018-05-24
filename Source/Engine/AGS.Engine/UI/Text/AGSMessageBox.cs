using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public static class AGSMessageBox
	{
        public static IRenderLayer RenderLayer = new AGSRenderLayer(AGSLayers.Speech.Z, independentResolution: new Size(1200, 800));
		public static ISayConfig Config = getDefaultConfig();
		public static ITextConfig ButtonConfig = getDefaultButtonConfig();
		public static float ButtonXPadding = 10f;
		public static float ButtonYPadding = 5f;
		public static float ButtonWidth = 60f;
		public static float ButtonHeight = 30f;

		public static async Task<IButton> DisplayAsync(string text, params IButton[] buttons)
		{
			float maxHeight = buttons.Length > 0 ? buttons.Max(b => b.Height) + (ButtonYPadding * 2f) : 0f;
            var sayComponent = getSayComponent(maxHeight);
			sayComponent.SpeechConfig.SkipText = buttons.Length > 0 ? SkipText.External : SkipText.ByMouse;
			IButton selectedButton = null;
			
			sayComponent.OnBeforeSay.Subscribe(args =>
			{
                args.Label.RenderLayer = RenderLayer;
				args.Label.Enabled = true;
                args.Label.AddComponent<IModalWindowComponent>().GrabFocus();
                var textConfig = sayComponent.SpeechConfig.TextConfig;

                float labelWidth = sayComponent.SpeechConfig.LabelSize.Width;

                float buttonsWidth = buttons.Sum(b => b.Width) + ButtonXPadding * (buttons.Length - 1);
				if (buttonsWidth > labelWidth)
				{
					//todo: alter label to have room for all buttons
				}
				float buttonX = labelWidth /2f - buttonsWidth/2f;
				foreach (var button in buttons)
				{
					args.Label.TreeNode.AddChild(button);
					button.X = buttonX;
					button.Y = ButtonYPadding;
					button.RenderLayer = args.Label.RenderLayer;
					buttonX += button.Width + ButtonXPadding;
					button.MouseClicked.Subscribe(_ =>
					{
						selectedButton = button;
						args.Skip();
					});
                    AGSGame.Game.State.UI.Add(button);
				}
			});
			
			await sayComponent.SayAsync(text);
			foreach (var button in buttons)
			{
				AGSGame.Game.State.UI.Remove(button);
			}
			return selectedButton;
		}

		public static async Task<bool> YesNoAsync(string text, string yes = "Yes", string no = "No")
		{
			var factory = AGSGame.Game.Factory;
			var idle = new ButtonAnimation (new EmptyImage (ButtonWidth, ButtonHeight));
			idle.Tint = Colors.Black;
			var hovered = new ButtonAnimation(new EmptyImage (ButtonWidth, ButtonHeight));
			hovered.Tint = Colors.Yellow;
			var pushed = new ButtonAnimation(new EmptyImage (ButtonWidth, ButtonHeight));
			pushed.Tint = Colors.DarkSlateBlue;
            var border = AGSBorders.Gradient(AGSGame.Resolver.Container.Resolve<IGLUtils>(),  AGSGame.Game.Settings, new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
			
			IButton yesButton = factory.UI.GetButton("Dialog Yes Button", idle, hovered, pushed, 0f, 0f, null, yes, ButtonConfig, false);
			IButton noButton = factory.UI.GetButton("Dialog No Button", idle, hovered, pushed, 0f, 0f, null, no, ButtonConfig, false);
			yesButton.Border = border;
			noButton.Border = border;
			return await DisplayAsync(text, yesButton, noButton) == yesButton;
		}

		public static Task<bool> OkCancelAsync(string text)
		{
			return YesNoAsync(text, "OK", "Cancel");
		}

        private static ISayComponent getSayComponent(float buttonHeight)
		{
			TypedParameter outfitParameter = new TypedParameter (typeof(IOutfitComponent), null);
			ISayLocationProvider location = new MessageBoxLocation (AGSGame.Game);
			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocationProvider), location);
			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IFaceDirectionComponent), null);
			TypedParameter configParameter = new TypedParameter (typeof(ISayConfig), AGSSayConfig.FromConfig(Config, buttonHeight));
			return AGSGame.Resolver.Container.Resolve<ISayComponent>(locationParameter, outfitParameter, 
				faceDirectionParameter, configParameter);
		}

		private static ISayConfig getDefaultConfig()
		{
			AGSSayConfig config = new AGSSayConfig ();
			config.Border =  AGSBorders.Gradient(AGSGame.Resolver.Container.Resolve<IGLUtils>(), AGSGame.Game.Settings, new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
            config.TextConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight, alignment: Alignment.TopCenter
                                                   , paddingLeft: 30, paddingTop: 30, paddingBottom: 30, paddingRight: 30);
            var screenWidth = AGSGame.Game.Settings.VirtualResolution.Width;
            var screenHeight = AGSGame.Game.Settings.VirtualResolution.Height;
            if (RenderLayer.IndependentResolution != null)
            {
                screenWidth = RenderLayer.IndependentResolution.Value.Width;
                screenHeight = RenderLayer.IndependentResolution.Value.Height;
            }
            config.LabelSize = new SizeF (screenWidth*3/4f, screenHeight*3/4f);
			config.BackgroundColor = Colors.Black;
			return config;
		}

		private static ITextConfig getDefaultButtonConfig()
		{
			return new AGSTextConfig (autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter);
		}

		private class MessageBoxLocation : ISayLocationProvider
		{
			private readonly IGame _game;

			public MessageBoxLocation(IGame game)
			{
				_game = game;
			}

			#region ISayLocation implementation

            public ISayLocation GetLocation(string text, ISayConfig sayConfig)
			{
                var config = sayConfig.TextConfig;
                float labelHeight = config.GetTextSize(text, sayConfig.LabelSize).Height;
                float height = labelHeight + config.PaddingTop + config.PaddingBottom;
                float width = sayConfig.LabelSize.Width + config.PaddingLeft + config.PaddingRight;
                var screenWidth = AGSGame.Game.Settings.VirtualResolution.Width;
                var screenHeight = AGSGame.Game.Settings.VirtualResolution.Height;
                if (RenderLayer.IndependentResolution != null)
                {
                    screenWidth = RenderLayer.IndependentResolution.Value.Width;
                    screenHeight = RenderLayer.IndependentResolution.Value.Height;
                }
                return new AGSSayLocation(new PointF (screenWidth / 2f - width / 2f, 
                                                      screenHeight / 2f - height / 2f), null);
			}

			#endregion
		}
	}
}

