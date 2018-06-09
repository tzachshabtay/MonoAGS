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
        private IDevice _device;

        public AGSFontFactory(IResourceLoader resources, IDevice device)
        {
            _device = device;
            _resources = resources;
            _installedFonts = new ConcurrentDictionary<string, string>();
        }

        public void InstallFonts(params string[] paths)
        {
            string[] newPaths = paths.Select(p => resourceToFilePath(p)).ToArray();
            _device.FontLoader.InstallFonts(newPaths);
        }

        public IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return _device.FontLoader.LoadFont(fontFamily, sizeInPoints, style);
        }

        public IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            path = resourceToFilePath(path);
            return _device.FontLoader.LoadFontFromPath(path, sizeInPoints, style);
        }

        private string resourceToFilePath(string resourcePath)
        {
            return _installedFonts.GetOrAdd(resourcePath, _ => 
            {
                string filePath = _resources.ResolvePath(resourcePath);
                if (filePath != null && _device.FileSystem.FileExists(filePath)) return filePath;
                var resource = _resources.LoadResource(resourcePath);
                if (resource == null) throw new NullReferenceException($"Failed to find font in path: {resourcePath}, current directory: {Directory.GetCurrentDirectory()}");
                filePath = Path.Combine(_device.FileSystem.StorageFolder, Path.GetFileName(resourcePath));
                try
                {
                    if (_device.FileSystem.FileExists(filePath))
                    {
                        _device.FileSystem.Delete(filePath);
                    }

                    using (Stream fileStream = _device.FileSystem.Create(filePath))
                    using (resource.Stream)
                    {
                        resource.Stream.CopyTo(fileStream);
                    }
                }
                catch (UnauthorizedAccessException) { }

                return filePath;
            });

        }
    }
}
