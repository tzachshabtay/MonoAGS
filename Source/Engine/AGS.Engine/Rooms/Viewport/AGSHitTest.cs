using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSHitTest : IHitTest, IDisposable
    {
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private readonly IInput _input;
        private readonly IDisplayList _displayList;
        private readonly ICoordinates _coordinates;

        public AGSHitTest(IGameState state, IGameEvents events, IInput input, IDisplayList displayList, ICoordinates coordinates)
        {
            _coordinates = coordinates;
            _displayList = displayList;
			_state = state;
            _events = events;
            _input = input;
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public IObject ObjectAtMousePosition { get; private set; }

        public IObject GetObjectAt(float x, float y, Predicate<IObject> filter = null)
        {
            return getObjectAt(new MousePosition(_coordinates.WorldXToWindowX(x), _coordinates.WorldYToWindowY(y), 
                                                 _coordinates), filter);
        }

        public void Dispose()
        {
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
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

		private void onRepeatedlyExecute()
        {
            ObjectAtMousePosition = getObjectAt(_input.MousePosition);
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
                if (!obj.Enabled || obj.ClickThrough)
					continue;

                if (!obj.CollidesWith(x, y, viewport)) continue;

                if (!hasFocus(obj) && (viewport.Parent == null || !hasFocus(viewport.Parent))) continue;

                if (filter != null && !filter(obj)) continue;

				return obj;
			}
			return null;
		}
	}
}