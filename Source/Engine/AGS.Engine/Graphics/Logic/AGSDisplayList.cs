using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayList : IDisplayList
    {
        private readonly IGameState _gameState;
        private readonly IInput _input;
        private readonly IImageRenderer _renderer;
        private readonly AGSWalkBehindsMap _walkBehinds;
        private readonly IComparer<IObject> _comparer;
        private IObject _mouseCursorContainer;

        public AGSDisplayList(IGameState gameState, IInput input, AGSWalkBehindsMap walkBehinds, IImageRenderer renderer)
        {
            _gameState = gameState;
            _input = input;
            _renderer = renderer;
            _walkBehinds = walkBehinds;
            _comparer = new RenderOrderSelector();
        }

        public List<IObject> GetDisplayList(IRoom room)
        {
            return getDisplayList(room);
        }

		private List<IObject> getDisplayList(IRoom room)
		{
			int count = 1 + room.Objects.Count + _gameState.UI.Count;

			List<IObject> displayList = new List<IObject>(count);

			if (room.Background != null)
				addToDisplayList(displayList, room.Background);

			foreach (IObject obj in room.Objects)
			{
				if (!room.ShowPlayer && obj == _gameState.Player)
					continue;
				addToDisplayList(displayList, obj, room);
			}

			foreach (var area in room.Areas) addDebugDrawArea(displayList, area, room);
			foreach (var area in room.Areas)
			{
				if (!area.Enabled || room.Background == null || room.Background.Image == null) continue;
				IObject drawable = _walkBehinds.GetDrawable(area, room.Background.Image.OriginalBitmap);
				if (drawable == null) continue;
				addToDisplayList(displayList, drawable, room);
			}

			foreach (IObject ui in _gameState.UI)
			{
				addToDisplayList(displayList, ui, room);
			}

			displayList.Sort(_comparer);
			addCursor(displayList, room);
			return displayList;
		}

		private void addCursor(List<IObject> displayList, IRoom room)
		{
			IObject cursor = _input.Cursor;
			if (cursor == null) return;
			if (_mouseCursorContainer == null || _mouseCursorContainer.Animation != cursor.Animation)
			{
				_mouseCursorContainer = cursor;
			}
			_mouseCursorContainer.X = (_input.MouseX - room.Viewport.X) * room.Viewport.ScaleX;
			_mouseCursorContainer.Y = (_input.MouseY - room.Viewport.Y) * room.Viewport.ScaleY;
			addToDisplayList(displayList, _mouseCursorContainer, room);
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

				imageRenderer.Prepare(obj, obj, room.Viewport);
				return;
			}

			addToDisplayList(displayList, obj);
		}

		private void addToDisplayList(List<IObject> displayList, IObject obj)
		{
			displayList.Add(obj);
		}

		//todo: duplicate code with AGSRendererLoop
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
