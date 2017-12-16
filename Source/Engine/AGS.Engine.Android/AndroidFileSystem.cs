using System;
using System.Collections.Generic;
using System.IO;

namespace AGS.Engine.Android
{
	public class AndroidFileSystem : IFileSystem
	{
        #region IFileSystem implementation

        public string StorageFolder => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public IEnumerable<string> GetFiles(string folder)
		{
            if (!Directory.Exists(folder)) return new List<string>();
			return Directory.GetFiles(folder);
		}

        public IEnumerable<string> GetDirectories(string folder)
        {
            if (!Directory.Exists(folder)) return new List<string>();
            return Directory.GetDirectories(folder);
        }

        public IEnumerable<string> GetLogicalDrives() => Directory.GetLogicalDrives();

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public bool DirectoryExists(string folder) => Directory.Exists(folder);

        public bool FileExists(string path) => File.Exists(path);

        public Stream Open(string path) => File.OpenRead(path);

        public Stream Create(string path) => File.Create(path);

        public void Delete(string path) => File.Delete(path);

        #endregion
    }
}

