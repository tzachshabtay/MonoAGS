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

        public bool IsDirty { get; set; }

        public static EntityModel Load(string path) => AGSProject.LoadJson<EntityModel>(path);
    }
}