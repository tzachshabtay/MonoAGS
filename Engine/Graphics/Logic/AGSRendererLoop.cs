using System;
using AGS.API;
using System.Linq;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSRendererLoop : IRendererLoop
	{
		private readonly IGameState _gameState;
		private readonly IImageRenderer _renderer;
		private readonly IComparer<IObject> _comparer;
		private readonly AGSWalkBehindsMap _walkBehinds;
		private readonly IInput _input;

		private IObject _mouseCursorContainer;


		public AGSRendererLoop (IGameState gameState, IImageRenderer renderer, IInput input, AGSWalkBehindsMap walkBehinds)
		{
			this._walkBehinds = walkBehinds;
			this._gameState = gameState;
			this._renderer = renderer;
			this._input = input;
			this._comparer = new RenderOrderSelector ();
		}

		#region IRendererLoop implementation

		public void Tick ()
		{
			if (_gameState.Player.Character == null) return;
			IRoom room = _gameState.Player.Character.Room;
			List<IObject> displayList = getDisplayList(room);
			foreach (IObject obj in displayList) 
			{
				IImageRenderer imageRenderer = getImageRenderer(obj);
				IPoint areaScaling = getAreaScaling(room, obj);

				imageRenderer.Prepare(obj, room.Viewport, areaScaling);

				imageRenderer.Render (obj, room.Viewport, areaScaling);
			}
		}

		#endregion

		private IPoint getAreaScaling(IRoom room, IObject obj)
		{
			if (obj.IgnoreScalingArea) return GLMatrixBuilder.NoScaling;
			foreach (IScalingArea area in room.ScalingAreas)
			{
				if (!area.Enabled || !area.IsInArea(obj.Location)) continue;
				float scale = MathUtils.Lerp(area.Mask.MaxY, area.MinScaling, area.Mask.MinY, area.MaxScaling, obj.Y);
				return new AGSPoint (scale, scale);
			}
			return GLMatrixBuilder.NoScaling;
		}

		private IImageRenderer getImageRenderer(IObject obj)
		{
			return obj.CustomRenderer ?? getAnimationRenderer(obj) ?? _renderer;
		}

		private IImageRenderer getAnimationRenderer(IObject obj)
		{
			if (obj.Animation == null) return null;
			return obj.Animation.Sprite.CustomRenderer;
		}

		private void addCursor(List<IObject> displayList, IRoom room)
		{
			IAnimationContainer cursor = _input.Cursor;
			if (cursor == null) return;
			if (_mouseCursorContainer == null || _mouseCursorContainer.Animation != cursor.Animation)
			{
				_mouseCursorContainer = new AGSObject (cursor, null, null) { Anchor = new AGSPoint (0f,1f) };
			}
			_mouseCursorContainer.X = _input.MouseX;
			_mouseCursorContainer.Y = _input.MouseY;
			addToDisplayList(displayList, _mouseCursorContainer, room);
		}

		private List<IObject> getDisplayList(IRoom room)
		{
			int count = 1 + room.Objects.Count + _gameState.UI.Count;

			List<IObject> displayList = new List<IObject> (count);

			if (room.Background != null)
				displayList.Add (room.Background);

			foreach (IObject obj in room.Objects) 
			{
				if (!room.ShowPlayer && obj == _gameState.Player.Character) 
					continue;
				addToDisplayList(displayList, obj, room);
			}

			foreach (var area in room.WalkableAreas) addDebugDrawArea(displayList, area, room);
			foreach (var area in room.WalkBehindAreas)
			{
				addToDisplayList(displayList, _walkBehinds.GetDrawable(area, room.Background.Image.OriginalBitmap), room);
				addDebugDrawArea(displayList, area, room);
			}

			foreach (IObject ui in _gameState.UI)
			{
				addToDisplayList(displayList, ui, room);
			}


			displayList.Sort(_comparer);
			addCursor(displayList, room);
			return displayList;
		}

		private void addDebugDrawArea(List<IObject> displayList, IArea area, IRoom room)
		{
			if (area.Mask.DebugDraw == null) return;
			addToDisplayList(displayList, area.Mask.DebugDraw, room);
		}

		private void addToDisplayList(List<IObject> displayList, IObject obj, IRoom room)
		{
			if (!obj.Visible)
			{
				IImageRenderer imageRenderer = getImageRenderer(obj);
				IPoint areaScaling = getAreaScaling(room, obj);

				imageRenderer.Prepare(obj, room.Viewport, areaScaling);
				return;
			}

			displayList.Add(obj);
		}
	}
}

