using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using AGS.API;
using Newtonsoft.Json;

namespace AGS.Editor
{
    [DataContract]
    public class EntityModel
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "DisplayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "EntityConcreteType")]
        public Type EntityConcreteType { get; set; }

        [DataMember(Name = "Components")]
        public Dictionary<Type, ComponentModel> Components { get; set; }

        [DataMember(Name = "Initializer")]
        public MethodModel Initializer { get; set; }

        public string Filename { get; private set; }

        public bool IsDirty { get; set; }

        public static EntityModel Load(string path)
        {
            if (!path.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase)) return null;
            try
            {
                var model = AGSProject.LoadJson<EntityModel>(path);
                model.Filename = Path.GetFileName(path);
                return model;
            }
            catch (JsonReaderException e)
            {
                Debug.WriteLine($"Exception while trying to read json from {path}.{Environment.NewLine}Exception: {e.ToString()}");
                throw;
            }
        }

        public void Save(string folderPath) => AGSProject.SaveJson(getFilepath(folderPath), this);

        private string getFilepath(string folder)
        {
            if (Filename != null)
            {
                return Path.Combine(folder, Filename);
            }
            string path = AGSProject.GetPath(folder, ID, ".json");
            Filename = Path.GetFileName(path);
            return path;
        }
    }
}