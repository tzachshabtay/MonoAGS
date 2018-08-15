using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public static class AGSMessageBox
	{
        public static Task<IButton> DisplayAsync(string text, params IButton[] buttons)
        {
            return DisplayAsync(text, AGSGame.Game, null, buttons);
        }

        public static Task<IButton> DisplayAsync(string text, IMessageBoxSettings settings, params IButton[] buttons)
        {
            return DisplayAsync(text, AGSGame.Game, settings, buttons);
        }

        public static async Task<IButton> DisplayAsync(string text, IGame game, IMessageBoxSettings settings = null, params IButton[] buttons)
		{
            settings = settings ?? game.Settings.Defaults.MessageBox;
			float maxHeight = buttons.Length > 0 ? buttons.Max(b => b.Height) + (settings.ButtonYPadding * 2f) : 0f;
            var sayComponent = getSayComponent(maxHeight, game, settings);
			sayComponent.SpeechConfig.SkipText = buttons.Length > 0 ? SkipText.External : SkipText.ByMouse;
			IButton selectedButton = null;
			
			sayComponent.OnBeforeSay.Subscribe(args =>
			{
                args.Label.RenderLayer = settings.RenderLayer;
				args.Label.Enabled = true;
                args.Label.AddComponent<IModalWindowComponent>().GrabFocus();
                var textConfig = sayComponent.SpeechConfig.TextConfig;

                float labelWidth = sayComponent.SpeechConfig.LabelSize.Width;

                float buttonsWidth = buttons.Sum(b => b.Width) + settings.ButtonXPadding * (buttons.Length - 1);
				if (buttonsWidth > labelWidth)
				{
					//todo: alter label to have room for all buttons
				}
				float buttonX = labelWidth /2f - buttonsWidth/2f;
				foreach (var button in buttons)
				{
					args.Label.TreeNode.AddChild(button);
					button.X = buttonX;
                    button.Y = settings.ButtonYPadding;
					button.RenderLayer = args.Label.RenderLayer;
                    buttonX += button.Width + settings.ButtonXPadding;
					button.MouseClicked.Subscribe(_ =>
					{
						selectedButton = button;
						args.Skip();
					});
                    game.State.UI.Add(button);
				}
			});
			
			await sayComponent.SayAsync(text);
			foreach (var button in buttons)
			{
				game.State.UI.Remove(button);
			}
			return selectedButton;
		}

        public static async Task<bool> YesNoAsync(string text, string yes = "Yes", string no = "No", IMessageBoxSettings settings = null, IGame game = null)
		{
            game = game ?? AGSGame.Game;
            var factory = game.Factory;
            settings = settings ?? game.Settings.Defaults.MessageBox;
            var idle = new ButtonAnimation (new EmptyImage (settings.ButtonWidth, settings.ButtonHeight));
			idle.Tint = Colors.Black;
            var hovered = new ButtonAnimation(new EmptyImage (settings.ButtonWidth, settings.ButtonHeight));
			hovered.Tint = Colors.Yellow;
            var pushed = new ButtonAnimation(new EmptyImage (settings.ButtonWidth, settings.ButtonHeight));
			pushed.Tint = Colors.DarkSlateBlue;
            var border = game.Factory.Graphics.Borders.Gradient(new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
			
            IButton yesButton = factory.UI.GetButton("Dialog Yes Button", idle, hovered, pushed, 0f, 0f, null, yes, settings.ButtonText, false);
            IButton noButton = factory.UI.GetButton("Dialog No Button", idle, hovered, pushed, 0f, 0f, null, no, settings.ButtonText, false);
			yesButton.Border = border;
			noButton.Border = border;
			return await DisplayAsync(text, yesButton, noButton) == yesButton;
		}

        public static Task<bool> OkCancelAsync(string text, IMessageBoxSettings settings = null, IGame game = null)
		{
			return YesNoAsync(text, "OK", "Cancel", settings, game);
		}

        private static ISayComponent getSayComponent(float buttonHeight, IGame game, IMessageBoxSettings settings)
		{
			TypedParameter outfitParameter = new TypedParameter(typeof(IOutfitComponent), null);
            ISayLocationProvider location = new MessageBoxLocation(game, settings);
			TypedParameter locationParameter = new TypedParameter(typeof(ISayLocationProvider), location);
			TypedParameter faceDirectionParameter = new TypedParameter(typeof(IFaceDirectionComponent), null);
            TypedParameter configParameter = new TypedParameter(typeof(ISayConfig), AGSSayConfig.FromConfig(settings.DisplayConfig, buttonHeight));
			return AGSGame.Resolver.Container.Resolve<ISayComponent>(locationParameter, outfitParameter, 
				faceDirectionParameter, configParameter);
		}

		private class MessageBoxLocation : ISayLocationProvider
		{
			private readonly IGame _game;
            private readonly IMessageBoxSettings _settings;

			public MessageBoxLocation(IGame game, IMessageBoxSettings settings)
			{
				_game = game;
                _settings = settings;
			}

			#region ISayLocation implementation

            public ISayLocation GetLocation(string text, ISayConfig sayConfig)
			{
                var config = sayConfig.TextConfig;
                float labelHeight = config.GetTextSize(text, sayConfig.LabelSize).Height;
                float height = labelHeight + config.PaddingTop + config.PaddingBottom;
                float width = sayConfig.LabelSize.Width + config.PaddingLeft + config.PaddingRight;
                var screenWidth = _settings.RenderLayer.IndependentResolution?.Width ?? _game.Settings.VirtualResolution.Width;
                var screenHeight = _settings.RenderLayer.IndependentResolution?.Height ?? _game.Settings.VirtualResolution.Height;
                return new AGSSayLocation(new PointF (screenWidth / 2f - width / 2f, 
                                                      screenHeight / 2f - height / 2f), null);
			}

			#endregion
		}
	}
}