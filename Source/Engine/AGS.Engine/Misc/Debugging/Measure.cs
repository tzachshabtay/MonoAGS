using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace AGS.Engine
{
    public static class Measure
    {
        private static ConcurrentDictionary<string, Stopwatch> _measurements = new ConcurrentDictionary<string, Stopwatch>();
        private static int _numIndents;
        private static string _lastKey;

        public static void Start(string key)
        {
            var watch = new Stopwatch();
            _measurements[key] = watch;
            if (_lastKey != null) Debug.WriteLine("");
            StringBuilder sb = new StringBuilder();
            indent(sb);
            sb.Append($"{key}: ");
            Debug.Write(sb.ToString());
            _lastKey = key;
            watch.Restart();
            _numIndents++;
        }

        public static void End(string key)
        {
            if (!_measurements.TryRemove(key, out var watch)) throw new Exception($"Missing watch for {key}");
            var time = watch.Elapsed;
            watch.Stop();
            var lastKey = _lastKey;
            _lastKey = null;
            _numIndents--;
            if (key == lastKey)
            {
                Debug.WriteLine(time);
                return;
            }
            StringBuilder sb = new StringBuilder();
            indent(sb);
            sb.Append($"{time} ({key})");
            Debug.WriteLine(sb.ToString());
        }

        private static void indent(StringBuilder sb)
        {
            for (int i = 0; i < _numIndents; i++) sb.Append(' ');
        }
    }
}
