using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    public class EmbeddedResourcesPack : IResourcePack
    {
        private Assembly _assembly;
        private string _assemblyName;
        private Dictionary<string, List<string>> _resourceFolders;

        public EmbeddedResourcesPack(Assembly assembly, string customAssemblyName = null, int priority = 1)
        {
            Priority = priority;
            _assembly = assembly;
            _assemblyName = customAssemblyName ?? assembly?.GetName().Name;
        }

        public int Priority { get; private set; }

        public static string AssetsFolder = "Assets";

        public string ResolvePath(string path)
        {
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path).ToUpper();
            if (filename.Contains(".")) return path;

            //We're assuming we received a file/resource name without the extension: let's try to find a matching resource/file.
            string folderResourceName = getResourceName(folder);
            if (_resourceFolders.TryGetValue(folderResourceName, out var files))
            {
                var matchingResource = files.FirstOrDefault(f => f.ToUpperInvariant().Contains(filename));
                if (matchingResource != null) return matchingResource;
            }
            return null;
        }

        public IResource LoadResource(string path)
        {
            string resourcePath = getResourceName(path);

            return loadResource(resourcePath);
        }

        public List<IResource> LoadResources(string folder)
        {
            loadResourceFolders();

            string folderResource = getResourceName(folder);
            List<IResource> resources = new List<IResource>();
            if (_resourceFolders.TryGetValue(folderResource, out var embeddedResources))
            {
                foreach (string resourceName in embeddedResources)
                {
                    IResource resource = loadResource(resourceName);
                    if (resource == null) continue;
                    resources.Add(resource);
                }
            }
            return resources;
        }

        private IResource loadResource(string resourceName)
        {
            if (resourceName == null) return null;
            Stream stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return null;
            return new AGSResource(resourceName, stream);
        }

        private string getResourceName(string path)
        {
            try
            {
                int assetsIndex = path.IndexOf(AssetsFolder, StringComparison.Ordinal);
                if (assetsIndex < 0) return null;
                string resourcePath = path.Substring(assetsIndex);
                resourcePath = resourcePath.Replace('/', '.').Replace('\\', '.');
                resourcePath = $"{_assemblyName}.{resourcePath}";
                return resourcePath;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid resource name: " + path ?? "null", e);
            }
        }

        private void loadResourceFolders()
        {
            if (Repeat.OnceOnly("LoadResourceFolders"))
            {
                _resourceFolders = new Dictionary<string, List<string>>(50);
                foreach (string name in _assembly.GetManifestResourceNames())
                {
                    string folder = getFolderName(name);
                    if (folder == null) continue;
                    _resourceFolders.GetOrAdd(folder, () => new List<string>(20)).Add(name);
                }
            }
        }

        private string getFolderName(string resource)
        {
            int lastDot = resource.LastIndexOf('.');
            if (lastDot < 0) return null;
            string folder = resource.Substring(0, lastDot);
            lastDot = folder.LastIndexOf('.');
            if (lastDot < 0) return null;
            folder = folder.Substring(0, lastDot);
            return folder;
        }
    }
}
