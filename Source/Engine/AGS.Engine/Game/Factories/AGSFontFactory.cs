using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSFontFactory : IFontFactory
    {
        private readonly IResourceLoader _resources;
        private readonly ConcurrentDictionary<string, string> _installedFonts;
        private readonly IDevice _device;
        private readonly IGameSettings _settings;

        public AGSFontFactory(IResourceLoader resources, IDevice device, IGameSettings settings)
        {
            _device = device;
            _resources = resources;
            _settings = settings;
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

        [MethodWizard]
        public ITextConfig GetTextConfig(IBrush brush = null, IFont font = null, IBrush outlineBrush = null, float outlineWidth = 0f,
            IBrush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f,
            Alignment alignment = Alignment.TopLeft, AutoFit autoFit = AutoFit.NoFitting,
            float paddingLeft = 2f, float paddingRight = 2f, float paddingTop = 2f, float paddingBottom = 2f, SizeF? labelMinSize = null)
        {
            return new AGSTextConfig
            {
                Brush = brush ?? _device.BrushLoader.LoadSolidBrush(Colors.White),
                Font = font ?? _settings.Defaults.TextFont,
                OutlineBrush = outlineBrush ?? _device.BrushLoader.LoadSolidBrush(Colors.White),
                OutlineWidth = outlineWidth,
                ShadowBrush = shadowBrush,
                ShadowOffsetX = shadowOffsetX,
                ShadowOffsetY = shadowOffsetY,
                Alignment = alignment,
                AutoFit = autoFit,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                LabelMinSize = labelMinSize
            };
        }

        private string resourceToFilePath(string resourcePath)
        {
            return _installedFonts.GetOrAdd(resourcePath, _ => 
            {
                string filePath = _resources.ResolvePath(resourcePath);
                if (filePath != null && _device.FileSystem.FileExists(filePath)) return filePath;
                var resource = _resources.LoadResource(resourcePath);
                if (resource == null) throw new NullReferenceException($"Failed to find font in path: {resourcePath}, current directory: {Directory.GetCurrentDirectory()}");
                filePath = Path.Combine(_device.FileSystem.StorageFolder, Path.GetFileName(resourcePath) ?? throw new NullReferenceException($"file name for {resourcePath} returned null"));
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
