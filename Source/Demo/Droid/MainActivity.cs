using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using AGS.Engine.Android;
using DemoGame;
using Android.Views;
using System;

namespace DemoQuest.Droid
{
	//[Activity(Label = "DemoQuest", MainLauncher = true, Icon = "@mipmap/icon")]
    [Activity(Label = "DemoQuest", MainLauncher = true, Icon = "@mipmap/icon",
              AlwaysRetainTaskState = true,
#if __ANDROID_11__
        HardwareAccelerated = false,
#endif
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.Keyboard, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidGameWindow.Instance.Init(this, DemoStarter.Run);
            AGSEngineAndroid.SetAssembly();

            // Inflate our UI from its XML layout description
            // - should match filename res/layout/main.xml ?
            SetContentView(Resource.Layout.Main);
            debugPath();

            // Load the view
            FindViewById(Resource.Id.AGSGameView);
            /*AGSGameView view = FindViewById(Resource.Id.AGSGameView) as AGSGameView;
            if (view != null)
            {
                view.Load += (sender, e) => DemoStarter.Run();
            }*/

        }

        protected override void OnPause()
        {
            base.OnPause();
            var view = FindViewById<AGSGameView>(Resource.Id.AGSGameView);
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            var view = FindViewById<AGSGameView>(Resource.Id.AGSGameView);
            view.Resume();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            AndroidGameWindow.Instance.OnResize(new EventArgs());
        }


        private void debugPath()
        {
            System.Diagnostics.Debug.WriteLine("FilesDir: " + FilesDir.Path);
            System.Diagnostics.Debug.WriteLine("CurrentDir: " + System.IO.Directory.GetCurrentDirectory());
            listDir(FilesDir.Path, 0);
        }

        private void listDir(string dir, int numTabs)
        {
            try
            {
                indent(numTabs, dir);
                indent(numTabs, "Files:");
                foreach (string path in System.IO.Directory.GetFiles(dir))
                {
                    indent(numTabs + 1, path);
                }
                indent(numTabs, "Folders:");
                foreach (string path in System.IO.Directory.GetDirectories(dir))
                {
                    listDir(path, numTabs + 1);
                }
            }
            catch (Exception e)
            {
                indent(numTabs, e.ToString());
            }
        }

        private void indent(int numTabs, string line)
        {
            string withTabs = "";
            for (int i = 0; i < numTabs; i++) withTabs += "   ";
            System.Diagnostics.Debug.WriteLine(withTabs + line);
        }
    }
}


