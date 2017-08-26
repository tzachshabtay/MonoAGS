﻿using System;
using AGS.API;
using System.Collections.Generic;
using Autofac;

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
        private readonly Stack<IObject> _parentStack;
        private readonly IGameWindow _gameWindow;
        private readonly IDisplayList _displayList;
        private IGLUtils _glUtils;
        private IShader _lastShaderUsed;
		
        private IFrameBuffer _fromTransitionBuffer, _toTransitionBuffer;        

		public AGSRendererLoop (Resolver resolver, IGame game, IImageRenderer renderer,
            IAGSRoomTransitions roomTransitions, IGLUtils glUtils, IGameWindow gameWindow,
            IEvent<DisplayListEventArgs> onBeforeRenderingDisplayList, IDisplayList displayList)
		{
            _displayList = displayList;
            _glUtils = glUtils;
            _gameWindow = gameWindow;
			_resolver = resolver;
            _game = game;
			_gameState = game.State;
			_renderer = renderer;
			_roomTransitions = roomTransitions;
            _displayListEventArgs = new DisplayListEventArgs(null);
            _parentStack = new Stack<IObject>();
            OnBeforeRenderingDisplayList = onBeforeRenderingDisplayList;
			_roomTransitions.Transition = new RoomTransitionInstant ();
		}

		#region IRendererLoop implementation

        public IEvent<DisplayListEventArgs> OnBeforeRenderingDisplayList { get; private set; }

		public bool Tick ()
		{
            if (_gameState.Room == null) return false;
			IRoom room = _gameState.Room;
            _glUtils.RefreshViewport(_game.Settings, _gameWindow, _gameState.Viewport.ProjectionBox);

			switch (_roomTransitions.State)
			{
				case RoomTransitionState.NotInTransition:
					activateShader();
					renderRoom();
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
			renderRoom();
			frameBuffer.End();
			return frameBuffer;
		}

		private void renderRoom()
		{
            renderViewport(_gameState.Viewport);
            try
            {
                foreach (var viewport in _gameState.SecondaryViewports)
                {
                    renderViewport(viewport);
                }
            }
            catch (InvalidOperationException) {} //can be triggered if a viewport was added/removed while enumerating- this should be resolved on next tick
		}

        private void renderViewport(IViewport viewport)
        {
            _glUtils.RefreshViewport(_game.Settings, _gameWindow, viewport.ProjectionBox);
            List<IObject> displayList = _displayList.GetDisplayList(viewport);
			_displayListEventArgs.DisplayList = displayList;
			OnBeforeRenderingDisplayList.Invoke(_displayListEventArgs);
			displayList = _displayListEventArgs.DisplayList;

			foreach (IObject obj in displayList)
			{
				renderObject(viewport, obj);
			}
        }

        private void renderObject(IViewport viewport, IObject obj)
		{
            refreshParentMatrices(obj);
            Size resolution = obj.RenderLayer == null || obj.RenderLayer.IndependentResolution == null ? 
                _game.Settings.VirtualResolution :
                obj.RenderLayer.IndependentResolution.Value;
            _glUtils.AdjustResolution(resolution.Width, resolution.Height);

            IImageRenderer imageRenderer = getImageRenderer(obj);

			imageRenderer.Prepare(obj, obj, viewport);

			var shader = applyObjectShader(obj);

			imageRenderer.Render (obj, viewport);

			removeObjectShader(shader);
		}

        private void refreshParentMatrices(IObject obj)
        {
            //Making sure all of the parents have their matrix refreshed before rendering the object,
            //as if they need a new matrix the object will need to recalculate its matrix as well.
            //todo: find a more performant solution, to only visit each object once.
			var parent = obj.TreeNode.Parent;
			while (parent != null)
			{
                _parentStack.Push(parent);
				parent = parent.TreeNode.Parent;
			}
			while (_parentStack.Count > 0) _parentStack.Pop().GetModelMatrices();
        }

		private static IShader applyObjectShader(IObject obj)
		{
			var shader = obj.Shader;
			if (shader != null) shader = shader.Compile();
			if (shader != null) shader.Bind();
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
				if (_lastShaderUsed != null) _lastShaderUsed.Unbind();
				return;
			}
			_lastShaderUsed = shader;
			shader.Bind();
		}

        //todo: duplicate code with AGSDisplayList
		private IImageRenderer getImageRenderer(IObject obj)
		{
			return obj.CustomRenderer ?? getAnimationRenderer(obj) ?? _renderer;
		}

		private IImageRenderer getAnimationRenderer(IObject obj)
		{
			if (obj.Animation == null) return null;
			return obj.Animation.Sprite.CustomRenderer;
		}

		

		
	}
}

