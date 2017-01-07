using System;
using System.Collections.Generic;
using System.IO;

namespace AGS.Engine.Android
{
	public class AndroidFileSystem : IFileSystem
	{
		#region IFileSystem implementation

        public string StorageFolder { get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); } }

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

        public IEnumerable<string> GetLogicalDrives()
        {
            return Directory.GetLogicalDrives();
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public bool DirectoryExists(string folder)
        {
            return Directory.Exists(folder);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public Stream Open(string path)
		{
			return File.OpenRead(path);
		}

		public Stream Create(string path)
		{
			return File.Create(path);
		}

		#endregion
	}
}

