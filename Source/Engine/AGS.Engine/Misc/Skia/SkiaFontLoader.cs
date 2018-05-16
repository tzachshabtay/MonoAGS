using System;
using AGS.API;

namespace AGS.Engine
{
    public class SkiaFontLoader : IFontLoader
    {
        public void InstallFonts(params string[] paths) {}

        public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return SkiaFont.Create(fontFamily, style, sizeInPoints);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return SkiaFont.CreateFromPath(path, style, sizeInPoints);
        }
    }
}