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

        public AGSHitTest(IGameState state, IGameEvents events, IInput input, IDisplayList displayList)
        {
            _displayList = displayList;
			_state = state;
            _events = events;
            _input = input;
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public IObject ObjectAtMousePosition { get; private set; }

        public void Dispose()
        {
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
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
            IObject obj = null;
            foreach (var viewport in _state.GetSortedViewports())
            {
                if (!viewport.Interactive) continue;
                obj = findObject(viewport);
                if (obj != null) break;
            }
            ObjectAtMousePosition = obj;
        }

        private IObject findObject(IViewport viewport)
        {
            var room = viewport.RoomProvider.Room;
            List<IObject> visibleObjects = _displayList.GetDisplayList(viewport);
            return getObjectAt(visibleObjects, _input.MousePosition.GetViewportX(viewport),
                               _input.MousePosition.GetViewportY(viewport), viewport);
        }

        private IObject getObjectAt(List<IObject> objects, float x, float y, IViewport viewport)
		{
            for (int i = objects.Count - 1; i >= 0; i--)
			{
                IObject obj = objects[i];
                if (!obj.Enabled || obj.ClickThrough)
					continue;

                if (!obj.CollidesWith(x, y, viewport)) continue;

                if (!hasFocus(obj) && (viewport.Parent == null || !hasFocus(viewport.Parent))) continue;

				return obj;
			}
			return null;
		}


	}
}