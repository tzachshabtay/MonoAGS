using System;
using System.IO;
using System.Runtime.Serialization;
using AGS.API;

namespace AGS.Editor
{
    [DataContract]
    public class AGSProject
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DotnetProjectPath { get; set; }

        public string AGSProjectPath { get; private set; }

        public StateModel Model { get; private set; }

        public static T LoadJson<T>(IEditorPlatform platform, IGame game, string path)
        {
            var serializer = platform.GetSerialization(game);
            var stream = File.OpenRead(path);
            string json;
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            T obj = serializer.Deserialize<T>(json);
            return obj;
        }

        public static void SaveJson(IEditorPlatform platform, IGame game, string path, object model)
        {
            var serializer = platform.GetSerialization(game);
            string json = serializer.Serialize(model);
            File.WriteAllText(path, json);
        }

        public static string GetPath(string folder, string id, string fileExtension)
        {
            id = getSafeFilename(id);
            string path = null;
            int uid = 0;
            do
            {
                string suffix = uid == 0 ? "" : $"_{uid.ToString()}";
                string name = $"{id}{suffix}{fileExtension}";
                path = Path.Combine(folder, name);
                uid++;
            }
            while (File.Exists(path));
            return path;
        }

        //https://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
        private static string getSafeFilename(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static AGSProject Load(IEditorPlatform platform, IGame game, string path)
        {
            var proj = LoadJson<AGSProject>(platform, game, path);
            proj.AGSProjectPath = path;
            proj.Model = new StateModel();
            proj.Model.Load(platform, game, Path.GetDirectoryName(path));
            return proj;
        }
    }
}