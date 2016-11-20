using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using AGS.Engine.Android;
using DemoGame;
using Android.Views;

namespace DemoQuest.Droid
{
	//[Activity(Label = "DemoQuest", MainLauncher = true, Icon = "@mipmap/icon")]
    [Activity(Label = "DemoQuest", MainLauncher = true, Icon = "@mipmap/icon",
#if __ANDROID_11__
        HardwareAccelerated = false,
#endif
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidDebugLogger.Init();

            //AGSEngineAndroid.Init();

            // Inflate our UI from its XML layout description
            // - should match filename res/layout/main.xml ?
            SetContentView(Resource.Layout.Main);

            // Load the view
            FindViewById(Resource.Id.AGSGameView);
            AGSEngineAndroid.Init();
            DemoStarter.Run();
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
    }
}


