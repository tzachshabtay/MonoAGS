using Autofac;
using AGS.API;

namespace AGS.Engine
{
	public class AGSDialogFactory : IDialogFactory
	{
        private readonly Resolver _resolver;
		private readonly IGameState _gameState;
		private readonly IUIFactory _ui;
		private readonly IObjectFactory _object;
        private readonly IBrushLoader _brushLoader;
        private readonly IFontLoader _fontLoader;

        public AGSDialogFactory(Resolver resolver, IGameState gameState, IUIFactory ui, IObjectFactory obj,
                                IBrushLoader brushloader, IFontLoader fontLoader)
		{
			_resolver = resolver;
            _brushLoader = brushloader;
            _fontLoader = fontLoader;
			_gameState = gameState;
			_ui = ui;
			_object = obj;
		}
			
		public IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			ITextConfig hasBeenChosenConfig = null, bool speakOption = true, bool showOnce = false)
		{
            var game = _resolver.Container.Resolve<IGame>();
			if (config == null) config = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
                brush: _brushLoader.LoadSolidBrush(Colors.White), font: _fontLoader.LoadFont(null,10f));
			if (hoverConfig == null) hoverConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
				brush: _brushLoader.LoadSolidBrush(Colors.Yellow), font: _fontLoader.LoadFont(null, 10f));
            if (hasBeenChosenConfig == null) hasBeenChosenConfig = new AGSTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
                brush: _brushLoader.LoadSolidBrush(Colors.Gray), font: _fontLoader.LoadFont(null, 10f));
            ILabel label = _ui.GetLabel(string.Format("Dialog option: {0}", text), text, game.Settings.VirtualResolution.Width, 20f, 0f, 0f, 
                                        config: config, addToUi: false);
			label.Enabled = true;
			TypedParameter labelParam = new TypedParameter (typeof(ILabel), label);
			NamedParameter speakParam = new NamedParameter ("speakOption", speakOption);
			NamedParameter showOnceParam = new NamedParameter ("showOnce", showOnce);
            NamedParameter hoverParam = new NamedParameter ("hoverConfig", hoverConfig);
            NamedParameter wasChosenParam = new NamedParameter("hasBeenChosenConfig", hasBeenChosenConfig);
            TypedParameter playerParam = new TypedParameter(typeof(ICharacter), _gameState.Player);
            IDialogActions dialogActions = _resolver.Container.Resolve<IDialogActions>(playerParam);
            TypedParameter dialogActionsParam = new TypedParameter(typeof(IDialogActions), dialogActions);
            IDialogOption option = _resolver.Container.Resolve<IDialogOption>(labelParam, speakParam, showOnceParam, hoverParam, 
                                                                    wasChosenParam, playerParam, dialogActionsParam);
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
            TypedParameter playerParam = new TypedParameter(typeof(ICharacter), _gameState.Player);
            IDialogActions dialogActions = _resolver.Container.Resolve<IDialogActions>(playerParam);
            TypedParameter dialogActionsParam = new TypedParameter(typeof(IDialogActions), dialogActions);
            IDialog dialog = _resolver.Container.Resolve<IDialog>(showParam, graphicsParam, dialogActionsParam);
			foreach (IDialogOption option in options)
			{
				dialog.Options.Add(option);
			}
			return dialog;
		}


	}
}

