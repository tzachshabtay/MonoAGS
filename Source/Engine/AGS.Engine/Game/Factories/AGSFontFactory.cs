using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSFontFactory : IFontLoader
    {
        private IResourceLoader _resources;
        private ConcurrentDictionary<string, string> _installedFonts;

        public AGSFontFactory(IResourceLoader resources)
        {
            _resources = resources;
            _installedFonts = new ConcurrentDictionary<string, string>();
        }

        public void InstallFonts(params string[] paths)
        {
            string[] newPaths = paths.Select(p => resourceToFilePath(p)).ToArray();
            Hooks.FontLoader.InstallFonts(newPaths);
        }

        public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return Hooks.FontLoader.LoadFont(fontFamily, sizeInPoints, style);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            path = resourceToFilePath(path);
            return Hooks.FontLoader.LoadFontFromPath(path, sizeInPoints, style);
        }

        private string resourceToFilePath(string resourcePath)
        {
            return _installedFonts.GetOrAdd(resourcePath, _ => 
            {
                string filePath = _resources.FindFilePath(resourcePath);
                if (filePath != null && Hooks.FileSystem.FileExists(filePath)) return filePath;
                var resource = _resources.LoadResource(resourcePath);
                if (resource == null) throw new NullReferenceException(string.Format("Failed to find font in path: {0}", resourcePath));
                filePath = Path.Combine(Hooks.FileSystem.StorageFolder, Path.GetFileName(resourcePath));
                if (Hooks.FileSystem.FileExists(filePath))
                {
                    Hooks.FileSystem.Delete(filePath);
                }

                using (Stream fileStream = Hooks.FileSystem.Create(filePath))
                using (resource.Stream)
                {
                    resource.Stream.CopyTo(fileStream);
                }

                return filePath;
            });

        }
    }
}
