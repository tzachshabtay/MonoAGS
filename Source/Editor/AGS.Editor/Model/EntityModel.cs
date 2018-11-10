using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        [DataMember(Name = "Parent")]
        public string Parent { get; set; }

        [DataMember(Name = "Children")]
        public List<string> Children { get; set; }

        public string ScriptName { get; set; }

        public string Filename { get; private set; }

        public bool IsDirty { get; set; }

        public static EntityModel FromEntity(IEntity entity)
        {
            var tree = entity.GetComponent<IInObjectTreeComponent>()?.TreeNode;
            var e = new EntityModel
            {
                ID = entity.ID,
                DisplayName = entity.DisplayName,
                EntityConcreteType = entity.GetType(),
                Components = new Dictionary<Type, ComponentModel>(20),
                IsDirty = true,
                Parent = tree?.Parent?.ID,
                Children = tree?.Children?.Select(c => c.ID)?.ToList() ?? new List<string>()
            };
            return e;
        }

        public static EntityModel Load(IEditorPlatform platform, IGame game, string path)
        {
            if (!path.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase)) return null;
            try
            {
                var model = AGSProject.LoadJson<EntityModel>(platform, game, path);
                model.Filename = Path.GetFileName(path);
                return model;
            }
            catch (JsonReaderException e)
            {
                Debug.WriteLine($"Exception while trying to read json from {path}.{Environment.NewLine}Exception: {e.ToString()}");
                throw;
            }
        }

        public void Save(IEditorPlatform platform, IGame game, string folderPath) => AGSProject.SaveJson(platform, game, getFilepath(folderPath), this);

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