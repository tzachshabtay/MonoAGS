using System.Collections.Generic;

namespace AGS.API
{
    public interface IResourceLoader
	{
		IResource LoadResource(string path);
		List<IResource> LoadResources(params string[] paths);
		List<IResource> LoadResources(string folder);

        string FindFilePath(string path);
	}
}

