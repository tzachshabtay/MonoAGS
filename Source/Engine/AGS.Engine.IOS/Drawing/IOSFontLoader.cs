using System.Collections.Concurrent;
using System.Diagnostics;
using AGS.API;
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;

namespace AGS.Engine.IOS
{
    public class IOSFontLoader : IFontLoader
    {
        private readonly ConcurrentDictionary<string, IFont> _installedFonts = 
            new ConcurrentDictionary<string, IFont>();

        public void InstallFonts(params string[] paths)
        {
        }

        public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            CTFont font = new CTFont(fontFamily, sizeInPoints);
            font = setFontStyle(font, sizeInPoints, style);
            return new IOSFont(font, style);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return _installedFonts.GetOrAdd(path, _ =>
            {
                CGFont cgFont = CGFont.CreateFromProvider(new CGDataProvider(path));
                NSError error;
                if (!CTFontManager.RegisterGraphicsFont(cgFont, out error))
                {
                    Debug.WriteLine("Failed to load font from {0} (loading default font instead), error: {1}", path, error.ToString());
                    return LoadFont("Helvetica", sizeInPoints, style);
                }
                CTFont font = new CTFont(cgFont, sizeInPoints, CGAffineTransform.MakeIdentity());
                font = setFontStyle(font, sizeInPoints, style);
                return new IOSFont(font, style);
            });
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
