using System;
using System.Diagnostics;
using AGS.API;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class ResourceLoader : IResourceLoader
	{
        private readonly List<IResource> _emptyResources = new List<IResource>(1);
        private List<IResourcePack> _sortedResourcePacks = new List<IResourcePack>(1);

        public ResourceLoader()
		{
            ResourcePacks = new AGSBindingList<ResourcePack>(5);
            ResourcePacks.OnListChanged.Subscribe(args => _sortedResourcePacks = ResourcePacks.OrderByDescending(r => r.Priority).Select(r => r.Pack).ToList());
		}

        public IAGSBindingList<ResourcePack> ResourcePacks { get; private set; }

		public IResource LoadResource(string path)
		{
            Debug.WriteLine("Loading resource from " + (path ?? "null"));
			if (shouldIgnoreFile(path)) return null;
            foreach (var pack in _sortedResourcePacks)
            {
                var resource = pack.LoadResource(path);
                if (resource != null) return resource;
            }
            return null;
		}

		public List<IResource> LoadResourcesFromPaths(params string[] paths)
		{
            if (paths.Length == 0)
            {
                return _emptyResources;
            }
            Debug.WriteLine("Loading resources from paths " + string.Join(", ", paths));
			List<IResource> resources = new List<IResource> (paths.Length);
			foreach (string path in paths)
			{
				if (shouldIgnoreFile(path)) continue;

				IResource resource = LoadResource(path);
				if (resource == null)
				{
					Debug.WriteLine($"Failed to load resource {path}.");
					continue;
				}
				resources.Add(resource);
			}
			return resources;
		}

		public List<IResource> LoadResources(string folder)
		{
            Debug.WriteLine("Loading resources from folder " + folder);
            foreach (var pack in _sortedResourcePacks)
            {
                var resources = pack.LoadResources(folder);
                if ((resources?.Count ?? 0) > 0) return resources.OrderBy(r => r.ID).ToList();
            }
            return _emptyResources;
		}

        public string ResolvePath(string path)
        {
            foreach (var pack in _sortedResourcePacks)
            {
                var newPath = pack.ResolvePath(path);
                if (!string.IsNullOrEmpty(newPath)) return newPath;
            }
            return null;
        }

		private bool shouldIgnoreFile(string path)
		{
			return path == null || 
                path.EndsWith(".DS_Store", StringComparison.Ordinal); //Mac OS file
		}
	}
}
