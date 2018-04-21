extern alias IOS;

using System.Collections.Concurrent;
using System.Diagnostics;
using AGS.API;
using IOS::UIKit;
using IOS::Foundation;
using IOS::CoreGraphics;
using IOS::CoreText;

namespace AGS.Engine.IOS
{
    public class IOSFontLoader : IFontLoader
    {
        private readonly ConcurrentDictionary<string, CGFont> _installedFonts = 
            new ConcurrentDictionary<string, CGFont>();

        private readonly ConcurrentDictionary<string, string> _postScriptNames =
            new ConcurrentDictionary<string, string>();

        public void InstallFonts(params string[] paths)
        {
        }

        public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            //todo: the postscript names dictionary helps preventing duplicate warnings from ios core text:
            //"For best performance, only use PostScript names when calling this API."
            //However, we'll still get one warning for each font family not written as postscript.
            //Need to see if there's a way to get the postscript name in advance.
            string postScriptName = null;
            _postScriptNames.TryGetValue(fontFamily ?? "", out postScriptName);
            if (postScriptName == null) postScriptName = fontFamily;
            CTFont font = new CTFont(postScriptName, sizeInPoints);
            _postScriptNames[fontFamily ?? ""] = font.PostScriptName;
            font = setFontStyle(font, sizeInPoints, style);
            return new IOSFont(font, style, this);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            CGFont cgFont = _installedFonts.GetOrAdd(path, _ =>
            {
                CGFont createdFont = CGFont.CreateFromProvider(new CGDataProvider(path));
                NSError error;
                if (!CTFontManager.RegisterGraphicsFont(createdFont, out error))
                {
                    Debug.WriteLine("Failed to load font from {0} (loading default font instead), error: {1}", path, error.ToString());
                    return null;
                }
                return createdFont;
            });
            if (cgFont == null) return LoadFont("Helvetica", sizeInPoints, style);

            CTFont font = new CTFont(cgFont, sizeInPoints, CGAffineTransform.MakeIdentity());
            font = setFontStyle(font, sizeInPoints, style);
            return new IOSFont(font, style, this);
        }

        private CTFont setFontStyle(CTFont font, float sizeInPoints, FontStyle style)
        {
            if (style == FontStyle.Regular) return font;

            var traits = getTraits(style);
            if (traits != CTFontSymbolicTraits.None)
                font = font.WithSymbolicTraits(sizeInPoints, traits, traits);

            var desc = getDescriptor(style);
            if (desc != null)
                font = font.WithAttributes(sizeInPoints, desc);

            return font;
        }

        private CTFontSymbolicTraits getTraits(FontStyle style)
        {
            CTFontSymbolicTraits traits = CTFontSymbolicTraits.None;
            if (style.HasFlag(FontStyle.Bold))
            {
                traits |= CTFontSymbolicTraits.Bold;
            }
            if (style.HasFlag(FontStyle.Italic))
            {
                traits |= CTFontSymbolicTraits.Italic;
            }
            return traits;
        }

        private CTFontDescriptor getDescriptor(FontStyle style)
        {
            if (!style.HasFlag(FontStyle.Underline) && !style.HasFlag(FontStyle.Strikeout)) return null;

            NSMutableDictionary dict = new NSMutableDictionary();
            if (style.HasFlag(FontStyle.Underline))
            {
                NSString underline = UIStringAttributeKey.UnderlineStyle;
                dict[underline] = NSNumber.FromInt32((int)NSUnderlineStyle.Single);
            }
            if (style.HasFlag(FontStyle.Strikeout))
            {
                NSString strike = UIStringAttributeKey.StrikethroughStyle;
                dict[strike] = NSNumber.FromInt32((int)NSUnderlineStyle.Single);
            }
            CTFontDescriptorAttributes attrs = new CTFontDescriptorAttributes(dict);
            CTFontDescriptor desc = new CTFontDescriptor(attrs);
            return desc;
        }
    }
}
