using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class KeyboardBindings
    {
        private readonly ConcurrentDictionary<KeyboardShortcut, string> _bindings;
        private readonly List<Key> _pressedKeys;

        public KeyboardBindings(IInput input)
        {
            OnKeyboardShortcutPressed = new AGSEvent<string>();
            _bindings = new ConcurrentDictionary<KeyboardShortcut, string>();
            _pressedKeys = new List<Key>();
            applyDefaults();
            input.KeyDown.Subscribe(onKeyDown);
            input.KeyUp.Subscribe(onKeyUp);
        }

        public const string Undo = "Undo";
        public const string Redo = "Redo";
        public const string GameView = "Game View";

        public IBlockingEvent<string> OnKeyboardShortcutPressed { get; private set; }

        public void Bind(KeyboardShortcut keyboardShortcut, string action)
        {
            removeBinding(keyboardShortcut);
            if (action == null) return;

            _bindings[keyboardShortcut] = action;
        }

        private void applyDefaults()
        {
            Bind(new KeyboardShortcut(Key.AltLeft, Key.G), GameView);
            Bind(new KeyboardShortcut(Key.AltRight, Key.G), GameView);

            Bind(new KeyboardShortcut(Key.ControlLeft, Key.Z), Undo);
            Bind(new KeyboardShortcut(Key.ControlRight, Key.Z), Undo);

            Bind(new KeyboardShortcut(Key.ControlLeft, Key.Y), Redo);
            Bind(new KeyboardShortcut(Key.ControlRight, Key.Y), Redo);
        }

        private void removeBinding(KeyboardShortcut keyboardShortcut)
        {
            if (!_bindings.TryRemove(keyboardShortcut, out string currentAction)) return;
        }

        private Key convert(Key key)
        {
            //working around this bug: https://github.com/opentk/opentk/issues/747
            switch (key)
            {
                case Key.AltRight: return Key.AltLeft;
                case Key.ControlRight: return Key.ControlLeft;
                case Key.ShiftRight: return Key.ShiftLeft;
                default: return key;
            }
        }

        private void onKeyUp(KeyboardEventArgs args)
        {
            var key = convert(args.Key);
            _pressedKeys.Remove(key);
        }

        private void onKeyDown(KeyboardEventArgs args)
        {
            var key = convert(args.Key);
            if (_pressedKeys.Contains(key)) return;
            _pressedKeys.Add(key);
            foreach (var pair in _bindings)
            {
                if (pair.Key.IsPressed(_pressedKeys))
                {
                    OnKeyboardShortcutPressed.Invoke(pair.Value);
                }
            }
        }
    }
}