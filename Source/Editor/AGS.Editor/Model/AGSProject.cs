using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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

        public static T LoadJson<T>(string path)
        {
            var stream = File.OpenRead(path);
            string json;
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

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