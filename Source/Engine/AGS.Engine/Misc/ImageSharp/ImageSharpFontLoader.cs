using System;
using System.Diagnostics;
using System.Linq;
using AGS.API;
using SixLabors.Fonts;

namespace AGS.Engine
{
    public class ImageSharpFontLoader : IFontLoader
    {
        private readonly FontCollection _collection;
        private IResourceLoader _resources => AGSGame.Game.Factory.Resources; //todo: get resource factory as a parameter
        private readonly FontFamily _defaultFamily;

        public ImageSharpFontLoader()
        {
            _collection = new FontCollection();

            _defaultFamily = SystemFonts.Collection.Families.First();
            Debug.WriteLine($"Default system font is {_defaultFamily.Name}.");
        }

        public void InstallFonts(params string[] paths) {}

        public IFont LoadFont(string fontFamily, float sizeInPoints, API.FontStyle style = API.FontStyle.Regular)
        {
            FontFamily family = null;
            if (fontFamily == null)
            {
                family = _defaultFamily;
            }
            else
            {
                if (!_collection.TryFind(fontFamily, out family) && !SystemFonts.TryFind(fontFamily, out family))
                {
                    family = SystemFonts.Collection.Families.First();
                    Debug.WriteLine($"Did not find font family {fontFamily}, using {family.Name} instead.");
                }
            }
            return new ImageSharpFont(family, sizeInPoints, style);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, API.FontStyle style = API.FontStyle.Regular)
        {
            var resource = _resources.LoadResource(path);
            var family = _collection.Install(resource.Stream);
            resource.Stream.Close();
            if (family == null)
            {
                family = _defaultFamily;
                Debug.WriteLine($"Failed to install font family from {path}, using {family.Name} instead.");
            }
            else
            {
                Debug.WriteLine($"Installed font family {family.Name} from {path}.");
            }
            return new ImageSharpFont(family, sizeInPoints, style);
        }
    }
}