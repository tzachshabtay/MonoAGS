﻿using Autofac;
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
        private readonly IFontFactory _fontLoader;
        private readonly IDialogSettings _defaults;

        public AGSDialogFactory(Resolver resolver, IGameState gameState, IUIFactory ui, IObjectFactory obj,
                                IBrushLoader brushloader, IFontFactory fontLoader, IDialogSettings defaults)
		{
			_resolver = resolver;
            _brushLoader = brushloader;
            _fontLoader = fontLoader;
			_gameState = gameState;
			_ui = ui;
			_object = obj;
            _defaults = defaults;
		}
			
		public IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			ITextConfig hasBeenChosenConfig = null, bool speakOption = true, bool showOnce = false)
		{
            var game = _resolver.Container.Resolve<IGame>();
            config = config ?? _defaults.Idle;
            hoverConfig = hoverConfig ?? _defaults.Hovered;
            hasBeenChosenConfig = hasBeenChosenConfig ?? _defaults.Chosen;
            ILabel label = _ui.GetLabel($"Dialog option: {text}", text, game.Settings.VirtualResolution.Width, 20f, 0f, 0f, 
                                        config: config, addToUi: false);
            label.RenderLayer = _defaults.RenderLayer;
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
                graphics.RenderLayer = _defaults.RenderLayer;
				graphics.Tint =  Colors.Black;
				graphics.Pivot = new PointF ();
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
