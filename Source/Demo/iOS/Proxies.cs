using System;
using System.Diagnostics;
using AGS.Engine.IOS;
using Foundation;
using ObjCRuntime;
using OpenTK.Platform.iPhoneOS;
using UIKit;

namespace DemoQuest.iOS
{
    [Register("GLControllerProxy")]
    public class GLControllerProxy : OpenGLViewController
    {
        public GLControllerProxy(IntPtr handle) : base(handle) 
        { 
            Debug.WriteLine("GLControllerProxy"); 
        }
    }

    [Register("GameViewProxy")]
    public class GameViewProxy : IOSGameView
    {
        [Export("initWithCoder:")]
        public GameViewProxy(NSCoder coder) : base(coder) 
        { 
            Debug.WriteLine("GameViewProxy");
        }

        [Export("layerClass")]
        public static new Class GetLayerClass()
        {
            return iPhoneOSGameView.GetLayerClass();
        }

        [Export("drawFrame")]
        void DrawFrame()
        {
            OnRenderFrame(new OpenTK.FrameEventArgs());
        }
    }
}
