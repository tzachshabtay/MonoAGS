using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    public class FileSystemResourcePack : IResourcePack
    {
        private Dictionary<string, List<string>> _externalFolders;
        private readonly Func<string, List<string>> _getFilesInFolderFunc;
        private readonly IFileSystem _fileSystem;
        private readonly string _rootFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.Engine.FileSystemResourcePack"/> class.
        /// </summary>
        /// <param name="fileSystem">File system.</param>
        /// <param name="assembly">The entry assembly (this will be used to figure out the root folder, if no root folder is given).</param>
        /// <param name="rootFolderPath">The root folder from which to resolve relative paths. 
        /// If not given the engine will try looking for the "Assets" folder itself.</param>
        public FileSystemResourcePack(IFileSystem fileSystem, Assembly assembly, string rootFolderPath = null)
        {
            _fileSystem = fileSystem;
            _rootFolderPath = rootFolderPath ?? autoDetectAssetsFolder(assembly);
            _externalFolders = new Dictionary<string, List<string>>(10);
            _getFilesInFolderFunc = getFilesInFolder; //Caching delegate to avoid memory allocations in critical path
        }

        public string ResolvePath(string path)
        {
            if (shouldIgnoreFile(path)) return null;
            path = getAbsolutePath(path);
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path)?.ToUpper();
            Trace.Assert(filename != null);
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
            folder = getAbsolutePath(folder);
            List<IResource> resources = new List<IResource>();
            foreach (var file in _fileSystem.GetFiles(folder))
            {
                var resource = LoadResource(file);
                if (resource == null) continue;
                resources.Add(resource);
            }
            return resources;
        }

        private string getAbsolutePath(string path) => Path.IsPathRooted(path) ? path : Path.Combine(_rootFolderPath, path);

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

        private string findAssetsDir(IEnumerable<string> dirs)
        {
            var assetsDir = dirs.FirstOrDefault(
                    p => Path.GetFileName(p)?.ToLowerInvariant() == EmbeddedResourcesPack.AssetsFolder.ToLowerInvariant());
            if (assetsDir != null)
            {
                Debug.WriteLine($"Found assets directory at: {assetsDir}");
            }
            return assetsDir;
        }

        private string autoDetectAssetsFolder(Assembly assembly)
        {
            string exeDir = assembly == null ? _fileSystem.GetCurrentDirectory() :
                Path.GetDirectoryName(assembly.GetName().CodeBase.Replace("file:///", ""));
            if (string.IsNullOrEmpty(exeDir)) return "";
            string dir = exeDir;
            while (true)
            {
                var childDirs = _fileSystem.GetDirectories(dir).ToArray();
                var assetsDir = findAssetsDir(childDirs);
                if (assetsDir != null) return assetsDir;

                foreach (var childDir in childDirs)
                {
                    assetsDir = findAssetsDir(_fileSystem.GetDirectories(childDir));
                    if (assetsDir != null) return assetsDir;
                }
                DirectoryInfo dirInfo = Directory.GetParent(dir);
                if (dirInfo == null)
                {
                    Debug.WriteLine($"Did not find assets directory, using executable directory: {exeDir}");
                    return exeDir;
                }
                dir = dirInfo.FullName;
            }
        }
    }
}
