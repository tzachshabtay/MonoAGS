using System;
using Android.Graphics;
using AGS.API;
using Android.Views;

namespace AGS.Engine.Android
{
	public static class Extensions
	{
		public static global::Android.Graphics.Color Convert(this AGS.API.Color color)
		{
			return new global::Android.Graphics.Color (color.R, color.G, color.B, color.A);
		}

		public static TypefaceStyle Convert(this FontStyle style)
		{
			if (style.HasFlag(FontStyle.Bold))
			{
				if (!style.HasFlag(FontStyle.Italic)) return TypefaceStyle.Bold;
				return TypefaceStyle.BoldItalic;
			}
			if (style.HasFlag(FontStyle.Italic))
			{
				return TypefaceStyle.Italic;
			}
			return TypefaceStyle.Normal;
		}

		public static FontStyle Convert(this TypefaceStyle style)
		{
			switch (style)
			{
				case TypefaceStyle.Bold:
					return FontStyle.Bold;
				case TypefaceStyle.BoldItalic:
					return FontStyle.Bold | FontStyle.Italic;
				case TypefaceStyle.Italic:
					return FontStyle.Italic;
				default:
					return FontStyle.Regular;
			}
		}

        public static Key? Convert(this Keycode keycode)
        {
            if (keycode >= Keycode.A && keycode <= Keycode.Z)
            {
                return keycode - Keycode.A + Key.A;
            }
            if (keycode >= Keycode.Num0 && keycode <= Keycode.Num9)
            {
                return keycode - Keycode.Num0 + Key.Number0;
            }
            if (keycode >= Keycode.Numpad0 && keycode <= Keycode.Numpad9)
            {
                return keycode - Keycode.Numpad0 + Key.Keypad0;
            }
            if (keycode >= Keycode.F1 && keycode <= Keycode.F12)
            {
                return keycode - Keycode.F1 + Key.F1;
            }
            switch (keycode)
            {
                case Keycode.Slash: return Key.Slash;
                case Keycode.Backslash: return Key.BackSlash;
                case Keycode.Space: return Key.Space;
                case Keycode.Back: return Key.BackSpace;
                case Keycode.Apostrophe: return Key.Quote;
                case Keycode.AltLeft: return Key.AltLeft;
                case Keycode.AltRight: return Key.AltRight;
                case Keycode.Comma: return Key.Comma;
                case Keycode.CtrlLeft: return Key.ControlLeft;
                case Keycode.CtrlRight: return Key.ControlRight;
                case Keycode.Del: return Key.BackSpace;
                case Keycode.Enter: return Key.Enter;
                case Keycode.Escape: return Key.Escape;
                case Keycode.Equals: return Key.Plus;
                case Keycode.Grave: return Key.Grave;
                case Keycode.Home: return Key.Home;
                case Keycode.Insert: return Key.Insert;
                case Keycode.LeftBracket: return Key.BracketLeft;
                case Keycode.RightBracket: return Key.BracketRight;
                case Keycode.Minus: return Key.Minus;
                case Keycode.MoveEnd: return Key.End;
                case Keycode.MoveHome: return Key.Home;
                case Keycode.NumLock: return Key.NumLock;
                case Keycode.NumpadAdd: return Key.KeypadAdd;
                case Keycode.NumpadDot: return Key.KeypadPeriod;
                case Keycode.NumpadComma: return Key.Comma;
                case Keycode.NumpadEnter: return Key.KeypadEnter;
                case Keycode.NumpadDivide: return Key.KeypadDivide;
                case Keycode.NumpadMultiply: return Key.KeypadMultiply;
                case Keycode.NumpadSubtract: return Key.KeypadSubtract;
                case Keycode.Period: return Key.Period;
                case Keycode.Plus: return Key.Plus;
                case Keycode.ScrollLock: return Key.ScrollLock;
                case Keycode.Semicolon: return Key.Semicolon;
                case Keycode.Tab: return Key.Tab;
                default: return null;
            }
        }
	}
}

