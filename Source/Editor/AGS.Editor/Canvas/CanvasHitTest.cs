using AGS.API;

namespace AGS.Editor
{
    public class CanvasHitTest
    {
        private readonly AGSEditor _editor;

        public CanvasHitTest(AGSEditor editor)
        {
            _editor = editor;
        }

        public void Refresh()
        {
            var pos = _editor.Game.Input.MousePosition;
            ObjectAtMousePosition = _editor.Game.HitTest.GetObjectAt(pos.XMainViewport, pos.YMainViewport, filterObject);
        }

        public IObject ObjectAtMousePosition { get; private set; }

        private bool filterObject(IObject obj)
        {
            if (obj.Room != null && obj.Room.Background == obj) return false;
            return true;
        }
    }
}