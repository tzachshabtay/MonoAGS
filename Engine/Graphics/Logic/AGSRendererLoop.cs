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

		public AGSRendererLoop (IGameState gameState, IImageRenderer renderer, AGSWalkBehindsMap walkBehinds)
		{
			this._walkBehinds = walkBehinds;
			this._gameState = gameState;
			this._renderer = renderer;
			this._comparer = new RenderOrderSelector ();
		}

		#region IRendererLoop implementation

		public void
		Tick ()
		{
			if (_gameState.Player.Character == null) return;
			IRoom room = _gameState.Player.Character.Room;
			List<IObject> displayList = getDisplayList(room);
			foreach (IObject obj in displayList) 
			{
				IImageRenderer imageRenderer = obj.CustomRenderer ?? 
					getAnimationRenderer(obj) ?? _renderer;
				imageRenderer.Render (obj, room.Viewport);
			}
		}

		#endregion

		private IImageRenderer getAnimationRenderer(IObject obj)
		{
			if (obj.Animation == null) return null;
			return obj.Animation.Sprite.CustomRenderer;
		}

		private List<IObject> getDisplayList(IRoom room)
		{
			int count = 1 + room.Objects.Count + _gameState.UI.Count;

			List<IObject> displayList = new List<IObject> (count);

			if (room.Background != null)
				displayList.Add (room.Background);

			foreach (IObject obj in room.Objects) 
			{
				if (!obj.Visible) continue;
				if (!room.ShowPlayer && obj == _gameState.Player.Character) 
					continue;
				displayList.Add (obj);
			}

			foreach (var area in room.WalkableAreas) addDebugDrawArea(displayList, area);
			foreach (var area in room.WalkBehindAreas)
			{
				displayList.Add(_walkBehinds.GetDrawable(area, room.Background.Image.OriginalBitmap));
				addDebugDrawArea(displayList, area);
			}

			foreach (IObject ui in _gameState.UI)
			{
				displayList.Add(ui);
			}

			displayList.Sort(_comparer);
			return displayList;
		}

		private void addDebugDrawArea(List<IObject> displayList, IArea area)
		{
			if (area.Mask.DebugDraw == null) return;
		    displayList.Add(area.Mask.DebugDraw);
		}
	}
}

