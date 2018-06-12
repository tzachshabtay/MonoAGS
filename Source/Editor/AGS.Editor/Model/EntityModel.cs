using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AGS.API;

namespace AGS.Editor
{
    public class EntityModel
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public Type EntityConcreteType { get; set; }

        [DataMember]
        public Dictionary<Type, ComponentModel> Components { get; set; }

        public void GenerateCode(string name, StringBuilder code)
        {
            if (DisplayName != null)
            {
                code.AppendLine($"{name}.{nameof(IEntity.DisplayName)} = {DisplayName}");
            }
            foreach (var pair in Components)
            {
                string componentName = $"_{pair.Key.Name}Component";
                pair.Value.GenerateCode(componentName, code);
            }
        }

        public static EntityModel Load(string path) => AGSProject.LoadJson<EntityModel>(path);
    }
}