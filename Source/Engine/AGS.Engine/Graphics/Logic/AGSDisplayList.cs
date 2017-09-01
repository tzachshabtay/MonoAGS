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

        public AGSDisplayList(IGameState gameState, IInput input, AGSWalkBehindsMap walkBehinds, 
                              IImageRenderer renderer)
        {
            _gameState = gameState;
            _input = input;
            _renderer = renderer;
            _walkBehinds = walkBehinds;
            _comparer = new RenderOrderSelector();
        }

        public List<IObject> GetDisplayList(IViewport viewport)
        {
            var settings = viewport.DisplayListSettings;
            var room = viewport.RoomProvider.Room;
			int count = 1 + room.Objects.Count + _gameState.UI.Count;

			List<IObject> displayList = new List<IObject>(count);

            if (settings.DisplayRoom)
            {
                if (room.Background != null)
                    addToDisplayList(displayList, room.Background, viewport);

                foreach (IObject obj in room.Objects)
                {
                    if (!room.ShowPlayer && obj == _gameState.Player)
                        continue;
                    addToDisplayList(displayList, obj, viewport);
                }

                foreach (var area in room.Areas) addDebugDrawArea(displayList, area, viewport);
                foreach (var area in room.Areas)
                {
                    if (!area.Enabled || room.Background == null || room.Background.Image == null) continue;
                    IObject drawable = _walkBehinds.GetDrawable(area, room.Background.Image.OriginalBitmap);
                    if (drawable == null) continue;
                    addToDisplayList(displayList, drawable, viewport);
                }
            }

            if (settings.DisplayGUIs)
            {
                foreach (IObject ui in _gameState.UI)
                {
                    addToDisplayList(displayList, ui, viewport);
                }
            }

			displayList.Sort(_comparer);
			return displayList;
		}

        private void addDebugDrawArea(List<IObject> displayList, IArea area, IViewport viewport)
		{
			if (area.Mask.DebugDraw == null) return;
			addToDisplayList(displayList, area.Mask.DebugDraw, viewport);
		}

        private void addToDisplayList(List<IObject> displayList, IObject obj, IViewport viewport)
		{
            if (!obj.Visible || viewport.DisplayListSettings.RestrictionList.IsRestricted(obj.ID))
			{
				IImageRenderer imageRenderer = getImageRenderer(obj);

				imageRenderer.Prepare(obj, obj, viewport);
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
