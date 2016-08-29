using System.Collections.Generic;
using System.IO;

namespace AGS.Engine
{
	public interface IFileSystem
	{
		IEnumerable<string> GetFiles(string folder);
        IEnumerable<string> GetDirectories(string folder);
        IEnumerable<string> GetLogicalDrives();
        string GetCurrentDirectory();
        bool DirectoryExists(string folder);

		Stream Open(string path);
		Stream Create(string path);
	}
}

