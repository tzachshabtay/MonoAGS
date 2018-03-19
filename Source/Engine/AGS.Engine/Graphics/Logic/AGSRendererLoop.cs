using System;
using AGS.API;
using System.Collections.Generic;
using Autofac;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AGS.Engine
{
	public class AGSRendererLoop : IRendererLoop
	{
		private readonly IGameState _gameState;
        private readonly IGame _game;
		private readonly IImageRenderer _renderer;
		private readonly Resolver _resolver;
		private readonly IAGSRoomTransitions _roomTransitions;
        private readonly DisplayListEventArgs _displayListEventArgs;
        private readonly IMatrixUpdater _matrixUpdater;
        private readonly IGameWindow _gameWindow;
        private readonly IDisplayList _displayList;
        private readonly IInput _input;
        private readonly IGameSettings _noAspectRatioSettings;
        private IGLUtils _glUtils;
        private IShader _lastShaderUsed;
        private IObject _mouseCursorContainer;
		
        private IFrameBuffer _fromTransitionBuffer, _toTransitionBuffer;        

		public AGSRendererLoop (Resolver resolver, IGame game, IImageRenderer renderer,
            IAGSRoomTransitions roomTransitions, IGLUtils glUtils, IGameWindow gameWindow,
            IBlockingEvent<DisplayListEventArgs> onBeforeRenderingDisplayList, IDisplayList displayList, 
            IInput input, IMatrixUpdater matrixUpdater)
		{
            _input = input;
            _displayList = displayList;
            _glUtils = glUtils;
            _gameWindow = gameWindow;
			_resolver = resolver;
            _game = game;
			_gameState = game.State;
            _noAspectRatioSettings = new AGSGameSettings(game.Settings.Title, game.Settings.VirtualResolution, preserveAspectRatio: false);
			_renderer = renderer;
			_roomTransitions = roomTransitions;
            _displayListEventArgs = new DisplayListEventArgs(null);
            _matrixUpdater = matrixUpdater;
            OnBeforeRenderingDisplayList = onBeforeRenderingDisplayList;
			_roomTransitions.Transition = new RoomTransitionInstant ();
		}

		#region IRendererLoop implementation

        public IBlockingEvent<DisplayListEventArgs> OnBeforeRenderingDisplayList { get; private set; }

        public bool Tick()
        {
            _glUtils.RefreshViewport(_game.Settings, _gameWindow, _gameState.Viewport);
            _glUtils.AdjustResolution(_game.Settings.VirtualResolution.Width, _game.Settings.VirtualResolution.Height);

			var transitionState = _roomTransitions.State;
            if (_gameState.Room == null) transitionState = RoomTransitionState.NotInTransition; //If there's no room, then room transition state is meaningless -> we'll interpret as not in transition, which will render the viewports, without a room (so GUIs will still be rendered).
			switch (transitionState)
			{
				case RoomTransitionState.NotInTransition:
					activateShader();
					renderAllViewports();
					break;
				case RoomTransitionState.BeforeLeavingRoom:
                    if (_roomTransitions.Transition == null)
                    {
                        _roomTransitions.State = RoomTransitionState.NotInTransition;
                        return false;
                    }
                    else if (_gameState.Cutscene.IsSkipping)
                    {
                        _roomTransitions.State = RoomTransitionState.PreparingTransition;
                        return false;
                    }
                    else if (!_roomTransitions.Transition.RenderBeforeLeavingRoom(
                        _displayList.GetDisplayList(_gameState.Viewport), 
                        obj => renderObject(_gameState.Viewport, obj)))
					{
						if (_fromTransitionBuffer == null) _fromTransitionBuffer = renderToBuffer();
                        _roomTransitions.State = RoomTransitionState.PreparingTransition;
						return false;
					}
					break;
                case RoomTransitionState.PreparingNewRoomDisplayList:
				case RoomTransitionState.PreparingTransition:
					return false;
				case RoomTransitionState.InTransition:
                    if (_gameState.Cutscene.IsSkipping)
                    { 
                        _fromTransitionBuffer = null;
                        _toTransitionBuffer = null;
                        _roomTransitions.State = RoomTransitionState.AfterEnteringRoom;
                        return false;
                    }
					if (_toTransitionBuffer == null) _toTransitionBuffer = renderToBuffer();
                    _glUtils.RefreshViewport(_noAspectRatioSettings, _gameWindow, _gameState.Viewport);
                    if (!_roomTransitions.Transition.RenderTransition(_fromTransitionBuffer, _toTransitionBuffer))
					{
						_fromTransitionBuffer = null;
						_toTransitionBuffer = null;
						_roomTransitions.State = RoomTransitionState.AfterEnteringRoom;
						return false;
					}
					break;
				case RoomTransitionState.AfterEnteringRoom:
                    if (_gameState.Cutscene.IsSkipping || !_roomTransitions.Transition.RenderAfterEnteringRoom(
                        _displayList.GetDisplayList(_gameState.Viewport), 
                        obj => renderObject(_gameState.Viewport, obj)))
					{
						_roomTransitions.SetOneTimeNextTransition(null);
						_roomTransitions.State = RoomTransitionState.NotInTransition;
						return false;
					}
					break;
				default:
					throw new NotSupportedException (_roomTransitions.State.ToString());
			}
			return true;
		}

		#endregion

        private IFrameBuffer renderToBuffer()
		{
            TypedParameter sizeParam = new TypedParameter(typeof(Size), _game.Settings.WindowSize);
            IFrameBuffer frameBuffer = _resolver.Container.Resolve<IFrameBuffer>(sizeParam);
			frameBuffer.Begin();
			renderAllViewports();
			frameBuffer.End();
			return frameBuffer;
		}

        private void renderAllViewports()
		{
            _matrixUpdater.ClearCache();
            renderViewport(_gameState.Viewport);
            try
            {
                foreach (var viewport in _gameState.SecondaryViewports)
                {
                    if (viewport == null) continue;
                    renderViewport(viewport);
                }
            }
            catch (InvalidOperationException) {} //can be triggered if a viewport was added/removed while enumerating- this should be resolved on next tick
            renderCursor();
		}

        private void renderViewport(IViewport viewport)
        {
            _glUtils.RefreshViewport(_game.Settings, _gameWindow, viewport);
            List<IObject> displayList = _displayList.GetDisplayList(viewport);
			_displayListEventArgs.DisplayList = displayList;
			OnBeforeRenderingDisplayList.Invoke(_displayListEventArgs);
			displayList = _displayListEventArgs.DisplayList;

			foreach (IObject obj in displayList)
			{
				renderObject(viewport, obj);
			}
        }

        private void renderCursor()
        {
			IObject cursor = _input.Cursor;
			if (cursor == null) return;
            cursor.IgnoreViewport = true;
			if (_mouseCursorContainer == null || _mouseCursorContainer.Animation != cursor.Animation)
			{
				_mouseCursorContainer = cursor;
			}
            var viewport = _gameState.Viewport;
            _mouseCursorContainer.X = _input.MousePosition.GetViewportX(viewport);
            _mouseCursorContainer.Y = _input.MousePosition.GetViewportY(viewport);
            _glUtils.RefreshViewport(_game.Settings, _gameWindow, viewport);
            _matrixUpdater.RefreshMatrix(_mouseCursorContainer);
            renderObject(viewport, _mouseCursorContainer);
        }

        private void renderObject(IViewport viewport, IObject obj)
		{
            _matrixUpdater.RefreshMatrix(obj);
            Size resolution = obj.RenderLayer == null || obj.RenderLayer.IndependentResolution == null ? 
                _game.Settings.VirtualResolution :
                obj.RenderLayer.IndependentResolution.Value;
            _glUtils.AdjustResolution(resolution.Width, resolution.Height);

            IImageRenderer imageRenderer = getImageRenderer(obj);

			var shader = applyObjectShader(obj);

			imageRenderer.Render (obj, viewport);

			removeObjectShader(shader);
		}

		private static IShader applyObjectShader(IObject obj)
		{
			var shader = obj.Shader;
			if (shader != null) shader = shader.Compile();
			shader?.Bind();
			return shader;
		}

		private void removeObjectShader(IShader shader)
		{
			if (shader == null) return;

			if (_lastShaderUsed != null) _lastShaderUsed.Bind();
			else shader.Unbind();
		}

		private void activateShader()
		{
			var shader = AGSGame.Shader;
			if (shader != null) shader = shader.Compile();
			if (shader == null)
			{
		        _lastShaderUsed?.Unbind();
				return;
			}
			_lastShaderUsed = shader;
			shader.Bind();
		}

        //todo: duplicate code with AGSDisplayList
		private IImageRenderer getImageRenderer(IObject obj)
		{
			return obj.CustomRenderer ?? getSpriteRenderer(obj) ?? _renderer;
		}

		private IImageRenderer getSpriteRenderer(IObject obj)
		{
			return obj?.CurrentSprite?.CustomRenderer;
		}
	}
}

