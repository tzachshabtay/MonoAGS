using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public static class AGSMessageBox
	{
		public static ISayConfig Config = getDefaultConfig();

		public static void Display(string text)//, params IButton[] buttons)
		{
			var sayBehavior = getSayBehavior();
			sayBehavior.Say(text);
		}

		private static ISayBehavior getSayBehavior()
		{
			TypedParameter outfitParameter = new TypedParameter (typeof(IHasOutfit), null);
			ISayLocation location = new MessageBoxLocation (AGSGame.Game);
			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocation), location);
			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IFaceDirectionBehavior), null);
			TypedParameter configParameter = new TypedParameter (typeof(ISayConfig), Config);
			return AGSGame.Resolver.Container.Resolve<ISayBehavior>(locationParameter, outfitParameter, 
				faceDirectionParameter, configParameter);
		}

		private static ISayConfig getDefaultConfig()
		{
			AGSSayConfig config = new AGSSayConfig ();
			config.Border =  AGSBorders.Gradient(new FourCorners<Color>(Colors.DarkOliveGreen,
				Colors.LightGreen, Colors.LightGreen, Colors.DarkOliveGreen), 3f, true);
			config.SkipText = SkipText.ByMouse;
			config.TextConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight
				, paddingLeft: 8, paddingTop: 8, paddingBottom: 8, paddingRight: 8);
			config.LabelSize = new SizeF (AGSGame.Game.VirtualResolution.Width*3/4f, 
				AGSGame.Game.VirtualResolution.Height*3/4f);
			config.BackgroundColor = Colors.Black;
			return config;
		}

		private class MessageBoxLocation : ISayLocation
		{
			private readonly IGame _game;

			public MessageBoxLocation(IGame game)
			{
				_game = game;
			}

			#region ISayLocation implementation

			public PointF GetLocation(string text, SizeF labelSize, ITextConfig config)
			{
				SizeF size = config.GetTextSize(text, labelSize);
				return new PointF (_game.VirtualResolution.Width / 2f - size.Width / 2f, 
					_game.VirtualResolution.Height / 2f - size.Height / 2f);
			}

			#endregion
		}
	}
}

