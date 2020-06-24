using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using AGS.API;

namespace AGS.Editor
{
    [DataContract]
    public class RoomModel
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "BackgroundEntity")]
        public string BackgroundEntity { get; set; }

        [DataMember(Name = "Entities")]
        public HashSet<string> Entities { get; set; }

        public string Folder { get; private set; }

        public const string Filename = "room.json";

        public static RoomModel Load(IEditorPlatform platform, IGame game, string path)
        {
            var model = AGSProject.LoadJson<RoomModel>(platform, game, path);
            model.Folder = getDirectoryName(Path.GetDirectoryName(path));
            return model;
        }

        public void Save(IEditorPlatform platform, IGame game, string folderPath) => AGSProject.SaveJson(platform, game, Path.Combine(getFolder(folderPath), Filename), this);

        private string getFolder(string folder)
        {
            if (Folder != null)
            {
                return Path.Combine(folder, Folder);
            }
            string path = AGSProject.GetPath(folder, ID, "");
            Directory.CreateDirectory(path);
            Folder = getDirectoryName(path);
            return path;
        }

        private static string getDirectoryName(string path)
        {
            return Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
        }
    }
}