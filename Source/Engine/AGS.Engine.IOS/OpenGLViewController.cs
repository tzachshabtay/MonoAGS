extern alias IOS;

using System;
using IOS::CoreGraphics;
using IOS::Foundation;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class OpenGLViewController : UIViewController
    {
        public OpenGLViewController(IntPtr handle) : base(handle)
        {
        }

        new IOSGameView View => (IOSGameView)base.View;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Code to start the Xamarin Test Cloud Agent
            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start ();
            #endif

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, a =>
            {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating();
            }, this);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, a =>
            {
                if (IsViewLoaded && View.Window != null)
                    View.StartAnimating();
            }, this);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillTerminateNotification, a =>
            {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating();
            }, this);
            //todo: this might be needed on older IOS versions, as ViewWillTransitionToSize might not be supported
            //UIDevice.Notifications.ObserveOrientationDidChange((sender, e) => IOSGameWindow.Instance.OnResize(e));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            View.StartAnimating();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            View.StopAnimating();
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);
            coordinator.AnimateAlongsideTransition((obj) => { }, (obj) => 
            {
                IOSGameWindow.Instance.OnResize(toSize);
            });
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }
}
