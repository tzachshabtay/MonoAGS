using System;
using API;
using System.Linq;
using System.Collections.Generic;

namespace Engine
{
	public class AGSRendererLoop : IRendererLoop
	{
		private readonly IGameState gameState;
		private readonly IImageRenderer renderer;
		private readonly IComparer<IObject> comparer;

		public AGSRendererLoop (IGameState gameState, IImageRenderer renderer)
		{
			this.gameState = gameState;
			this.renderer = renderer;
			this.comparer = new RenderOrderSelector ();
		}

		#region IRendererLoop implementation

		public void Tick ()
		{
			if (gameState.Player.Character == null) return;
			IRoom room = gameState.Player.Character.Room;
			List<IObject> displayList = getDisplayList(room);
			foreach (IObject obj in displayList) 
			{
				IImageRenderer imageRenderer = obj.CustomRenderer ?? 
					getAnimationRenderer(obj) ?? renderer;
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
			int count = 1 + room.Objects.Count + gameState.UI.Count;

			List<IObject> displayList = new List<IObject> (count);

			if (room.Background != null)
				displayList.Add (room.Background);

			foreach (IObject obj in room.Objects) 
			{
				if (!obj.Visible) continue;
				if (!room.ShowPlayer && obj == gameState.Player.Character) 
					continue;
				displayList.Add (obj);
			}

			foreach (var area in room.WalkableAreas) addDebugDrawArea(displayList, area);
			foreach (var area in room.WalkBehindAreas) addDebugDrawArea(displayList, area);

			foreach (IObject ui in gameState.UI)
			{
				displayList.Add(ui);
			}

			displayList.Sort(comparer);
			return displayList;
		}

		private void addDebugDrawArea(List<IObject> displayList, IArea area)
		{
			if (area.Mask.DebugDraw == null) return;
		    displayList.Add(area.Mask.DebugDraw);
		}
	}
}

