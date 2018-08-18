using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSHitTest : IAGSHitTest
    {
        private readonly IGameState _state;
        private readonly IDisplayList _displayList;
        private readonly ICoordinates _coordinates;

        public AGSHitTest(IGameState state, IDisplayList displayList, ICoordinates coordinates)
        {
            _coordinates = coordinates;
            _displayList = displayList;
			_state = state;
        }

        public IObject ObjectAtMousePosition { get; private set; }

        public IObject GetObjectAt(float x, float y, Predicate<IObject> filter = null)
        {
            return getObjectAt(new MousePosition(_coordinates.WorldXToWindowX(x), _coordinates.WorldYToWindowY(y), 
                                                 _coordinates), filter);
        }

        public void Refresh(MousePosition position)
        {
            ObjectAtMousePosition = getObjectAt(position);
        }

        private IObject getObjectAt(MousePosition position, Predicate<IObject> filter = null)
        {
            foreach (var viewport in _state.GetSortedViewports())
            {
                if (!viewport.Interactive) continue;
                var obj = findObject(viewport, position, filter);
                if (obj != null) return obj;
            }
            return null;
        }

		private bool hasFocus(IObject obj)
		{
			var focusedWindow = _state.FocusedUI.FocusedWindow;
			if (focusedWindow == null) return true;
			while (obj != null)
			{
				if (obj == focusedWindow || _state.FocusedUI.CannotLoseFocus.Contains(obj.ID)) return true;
				obj = obj.TreeNode.Parent;
			}
			return false;
		}

        private IObject findObject(IViewport viewport, MousePosition position, Predicate<IObject> filter)
        {
            List<IObject> visibleObjects = _displayList.GetDisplayList(viewport);
            return getObjectAt(visibleObjects, position.GetViewportX(viewport),
                               position.GetViewportY(viewport), viewport, filter);
        }

        private IObject getObjectAt(List<IObject> objects, float x, float y, IViewport viewport, Predicate<IObject> filter)
		{
            for (int i = objects.Count - 1; i >= 0; i--)
			{
                IObject obj = objects[i];
                if (filter == null)
                {
                    if (!obj.Enabled || obj.ClickThrough)
                        continue;
                }
                else if (!filter(obj)) continue;

                if (!obj.CollidesWith(x, y, viewport)) continue;

                if (!hasFocus(obj) && (viewport.Parent == null || !hasFocus(viewport.Parent))) continue;

				return obj;
			}
			return null;
		}
	}
}