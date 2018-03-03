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

        public string AGSProjectPath { get; set; }

        public static AGSProject Load(string path)
        {
            var stream = File.OpenRead(path);
            string json;
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            AGSProject proj = JsonConvert.DeserializeObject<AGSProject>(json);
            proj.AGSProjectPath = path;
            return proj;
        }
    }
}
