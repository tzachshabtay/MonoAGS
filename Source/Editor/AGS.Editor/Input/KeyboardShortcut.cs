using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Editor
{
    public class KeyboardShortcut : IEquatable<KeyboardShortcut>
    {
        private readonly List<Key> _keys;

        public KeyboardShortcut(params Key[] keys)
        {
            _keys = keys.ToList();
        }

        public bool IsPressed(List<Key> pressedKeys)
        {
            return _keys.Count == pressedKeys.Count && _keys.All(k => pressedKeys.Contains(k));
        }

        public bool Equals(KeyboardShortcut other)
        {
            return IsPressed(other._keys);
        }

        public override bool Equals(object obj)
        {
            return obj is KeyboardShortcut other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (_keys.Count == 0) return 0;
            return _keys[0].GetHashCode();
        }
	}
}