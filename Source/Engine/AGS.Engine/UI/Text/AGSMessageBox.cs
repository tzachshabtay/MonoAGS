using System;
using System.Linq;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public static class AGSMessageBox
	{
		public static ISayConfig Config = getDefaultConfig();
		public static ITextConfig ButtonConfig = getDefaultButtonConfig();
		public static float ButtonXPadding = 10f;
		public static float ButtonYPadding = 5f;
		public static float ButtonWidth = 60f;
		public static float ButtonHeight = 30f;

		public static IButton Display(string text, params IButton[] buttons)
		{
			float maxHeight = buttons.Length > 0 ? buttons.Max(b => b.Height) + (ButtonYPadding * 2f) : 0f;
			var sayBehavior = getSayBehavior(maxHeight);
			sayBehavior.SpeechConfig.SkipText = buttons.Length > 0 ? SkipText.External : SkipText.ByMouse;
			IButton selectedButton = null;
			if (buttons.Length > 0)
			{
				sayBehavior.OnBeforeSay.Subscribe((sender, args) =>
				{
					args.Label.Enabled = true;
                    var textConfig = sayBehavior.SpeechConfig.TextConfig;
                    SizeF textSize = textConfig.GetTextSize(text, sayBehavior.SpeechConfig.LabelSize);
                    float labelWidth = textSize.Width + textConfig.PaddingLeft + textConfig.PaddingRight;

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
						button.MouseClicked.Subscribe((s, e) =>
						{
							selectedButton = button;
							args.Skip();
						});
					}
				});
			}
			sayBehavior.Say(text);
			foreach (var button in buttons)
			{
				AGSGame.Game.State.UI.Remove(button);
			}
			return selectedButton;
		}

		public static bool YesNo(string text, string yes = "Yes", string no = "No")
		{
			var factory = AGSGame.Game.Factory;
			IAnimation idle = new AGSSingleFrameAnimation (new EmptyImage (ButtonWidth, ButtonHeight), factory.Graphics);
			idle.Sprite.Tint = Colors.Black;
			IAnimation hovered = new AGSSingleFrameAnimation (new EmptyImage (ButtonWidth, ButtonHeight), factory.Graphics);
			hovered.Sprite.Tint = Colors.Yellow;
			IAnimation pushed = new AGSSingleFrameAnimation (new EmptyImage (ButtonWidth, ButtonHeight), factory.Graphics);
			pushed.Sprite.Tint = Colors.DarkSlateBlue;
			var border = AGSBorders.Gradient(new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
			
			IButton yesButton = factory.UI.GetButton("Dialog Yes Button", idle, hovered, pushed, 0f, 0f, yes, ButtonConfig);
			IButton noButton = factory.UI.GetButton("Dialog No Button", idle, hovered, pushed, 0f, 0f, no, ButtonConfig);
			yesButton.Border = border;
			noButton.Border = border;
			return Display(text, yesButton, noButton) == yesButton;
		}

		public static bool OkCancel(string text)
		{
			return YesNo(text, "OK", "Cancel");
		}

		private static ISayBehavior getSayBehavior(float buttonHeight)
		{
			TypedParameter outfitParameter = new TypedParameter (typeof(IHasOutfit), null);
			ISayLocationProvider location = new MessageBoxLocation (AGSGame.Game);
			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocationProvider), location);
			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IFaceDirectionBehavior), null);
			TypedParameter configParameter = new TypedParameter (typeof(ISayConfig), AGSSayConfig.FromConfig(Config, buttonHeight));
			return AGSGame.Resolver.Container.Resolve<ISayBehavior>(locationParameter, outfitParameter, 
				faceDirectionParameter, configParameter);
		}

		private static ISayConfig getDefaultConfig()
		{
			AGSSayConfig config = new AGSSayConfig ();
			config.Border =  AGSBorders.Gradient(new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
			config.TextConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight
				, paddingLeft: 8, paddingTop: 8, paddingBottom: 8, paddingRight: 8);
			config.LabelSize = new SizeF (AGSGame.Game.Settings.VirtualResolution.Width*3/4f, 
				AGSGame.Game.Settings.VirtualResolution.Height*3/4f);
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
                SizeF size = config.GetTextSize(text, sayConfig.LabelSize);
				float height = size.Height + config.PaddingTop + config.PaddingBottom;
				float width = size.Width + config.PaddingLeft + config.PaddingRight;
                return new AGSSayLocation(new PointF (_game.Settings.VirtualResolution.Width / 2f - width / 2f, 
                              _game.Settings.VirtualResolution.Height / 2f - height / 2f), null);
			}

			#endregion
		}
	}
}

