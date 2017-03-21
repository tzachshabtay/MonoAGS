extern alias IOS;

using System;
using AGS.API;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class IOSGestures
    {
        private readonly IOSGameView _view;

        public IOSGestures(IOSGameView view)
        {
            _view = view;

            UIPanGestureRecognizer dragGesture = new UIPanGestureRecognizer(onDrag);

            UITapGestureRecognizer singleTapGesture = new UITapGestureRecognizer(onSingleTap);
            singleTapGesture.NumberOfTapsRequired = 1;

            UITapGestureRecognizer doubleTapGesture = new UITapGestureRecognizer(onDoubleTap);
            doubleTapGesture.NumberOfTapsRequired = 2;

            UILongPressGestureRecognizer longPressGesture = new UILongPressGestureRecognizer(onLongPress);

            addGesture(dragGesture);
            addGesture(singleTapGesture);
            addGesture(doubleTapGesture);
            addGesture(longPressGesture);

            singleTapGesture.RequireGestureRecognizerToFail(doubleTapGesture);
        }

        public event EventHandler<MousePositionEventArgs> OnUserSingleTap;
        public event EventHandler<MousePositionEventArgs> OnUserDoubleTap;
        public event EventHandler<MousePositionEventArgs> OnUserLongPress;
        public event EventHandler<MousePositionEventArgs> OnUserDrag;

        private void addGesture(UIGestureRecognizer gesture)
        { 
            gesture.ShouldRecognizeSimultaneously += (_, __) => true;
            _view.AddGestureRecognizer(gesture);
        }

        private void onDrag(UIPanGestureRecognizer gesture)
        {
            if (shouldSkipGesture(gesture)) return;
            fireEvent(OnUserDrag, gesture);
        }

        private void onSingleTap(UITapGestureRecognizer gesture)
        { 
            if (shouldSkipGesture(gesture)) return;
            fireEvent(OnUserSingleTap, gesture);
        }

        private void onDoubleTap(UITapGestureRecognizer gesture)
        {
            if (shouldSkipGesture(gesture)) return;
            fireEvent(OnUserDoubleTap, gesture);
        }

        private void onLongPress(UILongPressGestureRecognizer gesture)
        { 
            if (shouldSkipGesture(gesture)) return;
            fireEvent(OnUserLongPress, gesture);
        }

        private bool shouldSkipGesture(UIGestureRecognizer gesture)
        {
            return (gesture.State == UIGestureRecognizerState.Cancelled
                    || gesture.State == UIGestureRecognizerState.Failed
                    || gesture.State == UIGestureRecognizerState.Possible);
        }

        private void fireEvent(EventHandler<MousePositionEventArgs> ev, UIGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(_view);
            if (ev != null) ev(this, new MousePositionEventArgs((float)point.X, (float)point.Y));
        }
    }
}
