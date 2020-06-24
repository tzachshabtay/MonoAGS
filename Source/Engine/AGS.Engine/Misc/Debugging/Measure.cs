﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AGS.Engine
{
    public static class Measure
    {
        private static ConcurrentDictionary<string, Stopwatch> _measurements = new ConcurrentDictionary<string, Stopwatch>();
        private static int _numIndents;
        private static string _lastKey;
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, List<Stopwatch>>> _tags = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<Stopwatch>>>();

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

        public static void StartTag(string key)
        {
            _tags[key] = new ConcurrentDictionary<string, List<Stopwatch>>();
            Start(key);
        }

        public static void EndTag(string key)
        {
            End(key);
            if (!_tags.TryRemove(key, out var stopwatchesMap))
            {
                throw new Exception($"Missing key for tag measurement: {key}");
            }
            foreach (var stopwatches in stopwatchesMap)
            {
                var sum = stopwatches.Value.Sum(w => w.ElapsedTicks);
                var average = (float)sum / stopwatches.Value.Count;
                var totalTime = TimeSpan.FromTicks(sum);
                var averageTime = TimeSpan.FromTicks((long)average);
                var category = (stopwatches.Key == "" ? "" : $"Category: {stopwatches.Key}, "); 
                Debug.WriteLine($"\tTag: {key}, {category}Instances: {stopwatches.Value.Count}, Total: {totalTime}, Average: {averageTime}");
            }
        }

        public static Action StartTagInstance(string key, string subCategory = null)
        {
            if (!_tags.TryGetValue(key, out var values))
                return () => {};
            subCategory = subCategory ?? "";
            var watch = new Stopwatch();
            values.GetOrAdd(subCategory, _ => new List<Stopwatch>()).Add(watch);
            watch.Restart();
            return () => watch.Stop();
        }

        private static void indent(StringBuilder sb)
        {
            for (int i = 0; i < _numIndents; i++) sb.Append(' ');
        }
    }
}
