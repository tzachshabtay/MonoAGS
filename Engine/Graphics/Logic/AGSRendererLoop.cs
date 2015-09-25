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
				IImageRenderer imageRenderer = obj.CustomRenderer ?? 
					getAnimationRenderer(obj) ?? _renderer;
				IPoint areaScaling = getAreaScaling(room, obj);

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

		private IImageRenderer getAnimationRenderer(IObject obj)
		{
			if (obj.Animation == null) return null;
			return obj.Animation.Sprite.CustomRenderer;
		}

		private void addCursor(List<IObject> displayList)
		{
			IAnimationContainer cursor = _input.Cursor;
			if (cursor == null) return;
			if (_mouseCursorContainer == null || _mouseCursorContainer.Animation != cursor.Animation)
			{
				_mouseCursorContainer = new AGSObject (cursor, null) { Anchor = new AGSPoint (0f,1f) };
			}
			_mouseCursorContainer.X = _input.MouseX;
			_mouseCursorContainer.Y = _input.MouseY;
			addToDisplayList(displayList, _mouseCursorContainer);
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
				addToDisplayList(displayList, obj);
			}

			foreach (var area in room.WalkableAreas) addDebugDrawArea(displayList, area);
			foreach (var area in room.WalkBehindAreas)
			{
				addToDisplayList(displayList, _walkBehinds.GetDrawable(area, room.Background.Image.OriginalBitmap));
				addDebugDrawArea(displayList, area);
			}

			foreach (IObject ui in _gameState.UI)
			{
				addToDisplayList(displayList, ui);
			}

			displayList.Sort(_comparer);
			addCursor(displayList);
			return displayList;
		}

		private void addDebugDrawArea(List<IObject> displayList, IArea area)
		{
			if (area.Mask.DebugDraw == null) return;
			addToDisplayList(displayList, area.Mask.DebugDraw);
		}

		private void addToDisplayList(List<IObject> displayList, IObject obj)
		{
			if (!obj.Visible) return;
			displayList.Add(obj);
		}
	}
}

