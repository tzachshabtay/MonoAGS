using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class FileSystemResourcePack : IResourcePack
    {
        private Dictionary<string, List<string>> _externalFolders;
        private readonly Func<string, List<string>> _getFilesInFolderFunc;
        private readonly IFileSystem _fileSystem;

        public FileSystemResourcePack(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _externalFolders = new Dictionary<string, List<string>>(10);
            _getFilesInFolderFunc = getFilesInFolder; //Caching delegate to avoid memory allocations in critical path
        }

        public string ResolvePath(string path)
        {
            if (shouldIgnoreFile(path)) return null;
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path).ToUpper();
            if (filename.Contains(".")) return path;

            //We're assuming we received a file name without the extension: let's try to find a matching resource/file.
            var files = _externalFolders.GetOrAdd(folder, _getFilesInFolderFunc);
            var matchingFile = files.FirstOrDefault(f => f.ToUpperInvariant().Contains(filename));
            return matchingFile;
        }

        public IResource LoadResource(string path)
        {
            path = ResolvePath(path);
            if (path == null) return null;

            return loadFile(path);
        }

        public List<IResource> LoadResources(string folder)
        {
            List<IResource> resources = new List<IResource>();
            foreach (var file in _fileSystem.GetFiles(folder))
            {
                var resource = LoadResource(file);
                if (resource == null) continue;
                resources.Add(resource);
            }
            return resources;
        }

        private List<string> getFilesInFolder(string folder)
        {
            return _fileSystem.GetFiles(folder).ToList();
        }

        private IResource loadFile(string path)
        {
            if (!_fileSystem.FileExists(path))
            {
                Debug.WriteLine($"File system resource pack- failed to find resource at path: {path}");
                return null;
            }
            return new AGSResource(path, _fileSystem.Open(path));
        }

        private bool shouldIgnoreFile(string path)
        {
            return path == null ||
                path.EndsWith(".DS_Store", StringComparison.Ordinal); //Mac OS file
        }
    }
}
