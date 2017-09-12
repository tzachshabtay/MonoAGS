using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSHitTest : IHitTest, IDisposable
    {
        private readonly IGameState _state;
        private readonly IGameEvents _events;
		private readonly RenderOrderSelector _sorter;
        private readonly IInput _input;

        public AGSHitTest(IGameState state, IGameEvents events, IInput input)
        {
			_sorter = new RenderOrderSelector { Backwards = true };
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
            for (int i = _state.SecondaryViewports.Count - 1; i >= 0; i--)
            {
                IViewport viewport = _state.SecondaryViewports[i];
                if (!viewport.Interactive) continue;
                obj = findObject(viewport);
                if (obj != null) break;
            }
            if (obj == null && _state.Viewport.Interactive) 
            {
                obj = findObject(_state.Viewport);
            }
            ObjectAtMousePosition = obj;
        }

        private IObject findObject(IViewport viewport)
        {
            var room = viewport.RoomProvider.Room;
            List<IObject> visibleObjects = room != null && viewport.DisplayListSettings.DisplayRoom ?
                                                           room.Objects.Where(o => viewport.IsObjectVisible(o) && (room.ShowPlayer || o != _state.Player)).ToList() 
                                                                   : new List<IObject>(_state.UI.Count);
            if (viewport.DisplayListSettings.DisplayGUIs) visibleObjects.AddRange(_state.UI.Where(o => viewport.IsObjectVisible(o)));
            visibleObjects.Sort(_sorter);
            return getObjectAt(visibleObjects, _input.MousePosition.GetViewportX(viewport),
                               _input.MousePosition.GetViewportY(viewport), viewport);
            
        }

        private IObject getObjectAt(List<IObject> objects, float x, float y, IViewport viewport)
		{
			foreach (IObject obj in objects)
			{
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
