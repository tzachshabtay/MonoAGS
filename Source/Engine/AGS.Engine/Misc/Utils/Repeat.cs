using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public static class Repeat
	{
		private static ConcurrentDictionary<string, Counter> _times = new ConcurrentDictionary<string, Counter>();

        /// <summary>
        /// This function gives you an easy way of making some code run only the first time that the player encounters it. 
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <returns><c>true</c>, for the first time that it is called with this key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
		public static bool OnceOnly(string key) => Do(key) == 1;

        /// <summary>
        /// This function allows you to make some code run exactly X times.
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <returns><c>true</c> if this was called "times" times with this key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="times">Times.</param>
		public static bool Exactly(string key, int times) => Do(key) == times;

        /// <summary>
        /// This function allows you to make some code run more than X times.
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <returns><c>true</c> if this was called more than "times" times with this key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="times">Times.</param>
		public static bool MoreThan(string key, int times) => Do(key) > times;

        /// <summary>
        /// This function allows you to make some code run less than X times.
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <returns><c>true</c> if this was called less than "times" times with this key, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="times">Times.</param>
		public static bool LessThan(string key, int times) => Do(key) < times;

        /// <summary>
        /// This counts how many times this function was called with the given key.
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <returns>The number of times this was called with this key.</returns>
        /// <param name="key">Key.</param>
		public static int Do(string key) => getCounter(key).Increment();

        /// <summary>
        /// Rotates around a list of actions to perform (i.e on each call it performs the next action on the list, 
        /// and eventually goes back to the beginning).
        /// 
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <example>
        /// This can be useful when wanting to have multiple responses for an interaction which repeat themselves:
        /// <code>
        /// private void someDefaultInteraction()
        /// {
        ///    Repeat.Rotate("default interaction",
        ///        () => cHero.Say("I'm not going to do that."),
        ///        () => cHero.Say("Nope, I don't think so."),
        ///        () => cHero.Say("I really don't want to, sorry."));
        /// }
        /// </code>
        /// </example>
        /// <param name="key">Key.</param>
        /// <param name="actions">Actions.</param>
        public static void Rotate(string key, params Action[] actions)
        {
            int times = Do(key);
            actions[times % actions.Length]();
        }

        /// <summary>
        /// Rotates asynchonously around a list of actions to perform (i.e on each call it performs the next action on the list, 
        /// and eventually goes back to the beginning).
        /// 
        /// The key parameter is an arbitrary string. You can pass whatever you like in for this, but IT MUST BE UNIQUE.
        /// It is this string that allows the engine to determine whether this section of code has been run before, 
        /// therefore you should make sure that you do not use the same key string in two different places in your game.
        /// </summary>
        /// <example>
        /// This can be useful when wanting to have multiple responses for an interaction which repeat themselves:
        /// <code>
        /// private async Task someDefaultInteraction()
        /// {
        ///    await Repeat.RotateAsync("default interaction",
        ///        () => cHero.SayAsync("I'm not going to do that."),
        ///        () => cHero.SayAsync("Nope, I don't think so."),
        ///        () => cHero.SayAsync("I really don't want to, sorry."));
        /// }
        /// </code>
        /// </example>
        /// <param name="key">Key.</param>
        /// <param name="actions">Actions.</param>
        public static Task RotateAsync(string key, params Func<Task>[] actions)
        {
            int times = Do(key);
            return actions[times % actions.Length]();
        }

        public static int Current(string key) => getCounter(key).Value;

        public static void Reset(string key)
        {
            getCounter(key).Reset();
        }

        public static Dictionary<string, int> ToDictionary() => _times.ToDictionary(k => k.Key, v => v.Value.Value);

        public static void FromDictionary(Dictionary<string, int> counters)
        {
			_times = new ConcurrentDictionary<string, Counter> ();
			foreach (var counter in counters)
			{
				_times.TryAdd(counter.Key, new Counter (counter.Value));
			}
        }

		private static Counter getCounter(string key)
		{
			return _times.GetOrAdd(key, _ => new Counter ());
		}
			
		private class Counter
		{
            public Counter(int value = 0)
            {
                _value = value;
            }
            
			private int _value;

            public int Value => _value;

            public int Increment()
			{
				return Interlocked.Increment(ref _value);
			}

			public void Reset()
			{
				_value = 0;
			}
		}
	}
}

