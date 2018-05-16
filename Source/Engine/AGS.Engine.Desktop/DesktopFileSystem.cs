using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AGS.Engine.Desktop
{
	public class DesktopFileSystem : IFileSystem
	{
        #region IFileSystem implementation

        public string StorageFolder => Directory.GetCurrentDirectory();  //todo: find a suitable save location on desktop

        public IEnumerable<string> GetFiles(string folder)
		{
            try
            {
                if (!Directory.Exists(folder)) return new List<string>();
                return Directory.GetFiles(folder);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine($"GetFiles: Permission denied for {folder}");
                return new List<string>();
            }
		}

        public IEnumerable<string> GetDirectories(string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) return new List<string>();
                return Directory.GetDirectories(folder);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine($"GetDirectories: Permission denied for {folder}");
                return new List<string>();
            }
        }

        public IEnumerable<string> GetLogicalDrives() => Directory.GetLogicalDrives();

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public bool DirectoryExists(string folder) => Directory.Exists(folder);

        public bool FileExists(string path) => File.Exists(path);

        public Stream Open(string path) => File.OpenRead(path);

        public Stream Create(string path) => File.Create(path);

        public void Delete(string path)
        {
            File.Delete(path);
        }

		#endregion
		
	}
}