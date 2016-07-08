using System;
using Autofac;
using AGS.API;


namespace AGS.Engine
{
	public class AGSDialogFactory : IDialogFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;
		private IUIFactory _ui;
		private IObjectFactory _object;

		public AGSDialogFactory(IContainer resolver, IGameState gameState, IUIFactory ui, IObjectFactory obj)
		{
			_resolver = resolver;
			_gameState = gameState;
			_ui = ui;
			_object = obj;
		}
			
		public IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			bool speakOption = true, bool showOnce = false)
		{
			IGame game = _resolver.Resolve<IGame>();
			if (config == null) config = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapWithoutHeightFitting,
				brush: Hooks.BrushLoader.LoadSolidBrush(Colors.White), font: Hooks.FontLoader.LoadFont(null,10f));
			if (hoverConfig == null) hoverConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapWithoutHeightFitting,
				brush: Hooks.BrushLoader.LoadSolidBrush(Colors.Yellow), font: Hooks.FontLoader.LoadFont(null, 10f));
			ILabel label = _ui.GetLabel(string.Format("Dialog option: {0}", text), text, game.VirtualResolution.Width, 20f, 0f, 0f, config);
			label.Enabled = true;
			TypedParameter labelParam = new TypedParameter (typeof(ILabel), label);
			NamedParameter speakParam = new NamedParameter ("speakOption", speakOption);
			NamedParameter showOnceParam = new NamedParameter ("showOnce", showOnce);
			TypedParameter hoverParam = new TypedParameter (typeof(ITextConfig), hoverConfig);
			IDialogOption option = _resolver.Resolve<IDialogOption>(labelParam, speakParam, showOnceParam, hoverParam);
			return option;
		}

		public IDialog GetDialog(string id, float x = 0f, float y = 0f, IObject graphics = null, bool showWhileOptionsAreRunning = false, 
			params IDialogOption[] options)
		{
			TypedParameter showParam = new TypedParameter (typeof(bool), showWhileOptionsAreRunning);
			if (graphics == null)
			{
				graphics = _object.GetObject(id);
				graphics.Tint =  Colors.Black;
				graphics.Anchor = new PointF ();
                graphics.IgnoreViewport = true;
                graphics.IgnoreScalingArea = true;
				_gameState.UI.Add(graphics);
			}
			TypedParameter graphicsParam = new TypedParameter (typeof(IObject), graphics);
			IDialog dialog = _resolver.Resolve<IDialog>(showParam, graphicsParam);
			foreach (IDialogOption option in options)
			{
				dialog.Options.Add(option);
			}
			return dialog;
		}


	}
}

