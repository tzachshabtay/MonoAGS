using System;
using Android.Views;

namespace AGS.Engine.Android
{
    public class AndroidSimpleGestures : GestureDetector.SimpleOnGestureListener
    {
        public event EventHandler<MotionEvent> OnUserSingleTap;
        public event EventHandler<MotionEvent> OnUserDoubleTap;
        public event EventHandler<MotionEvent> OnUserLongPress;
        public event EventHandler<MotionEvent> OnUserDrag;

        public override bool OnDown(MotionEvent e)
        {
            return true;
        }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            fireEvent(OnUserSingleTap, e);
            return base.OnSingleTapConfirmed(e);
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            fireEvent(OnUserDoubleTap, e);
            return base.OnDoubleTap(e);
        }

        public override void OnLongPress(MotionEvent e)
        {
            fireEvent(OnUserLongPress, e);
            base.OnLongPress(e);
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            fireEvent(OnUserDrag, e2);
            return base.OnScroll(e1, e2, distanceX, distanceY);
        }

        private void fireEvent(EventHandler<MotionEvent> ev, MotionEvent e)
        {
            ev?.Invoke(this, e);
        }
    }
}
