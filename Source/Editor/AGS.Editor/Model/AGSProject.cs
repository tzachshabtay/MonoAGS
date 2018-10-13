using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AGS.Editor
{
    public class AGSProject
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DotnetProjectPath { get; set; }

        public string AGSProjectPath { get; private set; }

        public StateModel Model { get; private set; }

        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static T LoadJson<T>(string path)
        {
            var stream = File.OpenRead(path);
            string json;
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            T obj = JsonConvert.DeserializeObject<T>(json, JsonSettings);
            return obj;
        }

        public static void SaveJson(string path, object model)
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented, JsonSettings);
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

        public static AGSProject Load(string path)
        {
            var proj = LoadJson<AGSProject>(path);
            proj.AGSProjectPath = path;
            proj.Model = new StateModel();
            proj.Model.Load(Path.GetDirectoryName(path));
            return proj;
        }
    }
}