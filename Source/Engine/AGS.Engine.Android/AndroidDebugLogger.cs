using System.Diagnostics;
using Android.Util;

namespace AGS.Engine.Android
{
    public class AndroidDebugLogger : TraceListener
    {
        public static void Init()
        {
            Debug.Listeners.Add(new AndroidDebugLogger());
        }

        public override void Write(string message)
        {
            Log.Verbose("AGS", message);
        }

        public override void WriteLine(string message)
        {
            Log.Verbose("AGS", message);
        }
    }
}
