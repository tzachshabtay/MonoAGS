using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AGS.Engine;
using AGS.Engine.IOS;
using DemoGame;
using Foundation;
using UIKit;

namespace DemoQuest.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
            Debug.WriteLine("Main started");
            ResourceLoader.CustomAssemblyName = "DemoQuest.iOS";
            IOSGameWindow.Instance.StartGame = DemoStarter.Run;
            AGSEngineIOS.SetAssembly();
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
            Debug.WriteLine("Running UIApplication");
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
