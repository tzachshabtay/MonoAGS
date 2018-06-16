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

        public void GenerateCode(string name, StringBuilder code)
        {
            if (!IsDirty) return;
            name = getVariableName(name);
            if (DisplayName != null)
            {
                code.AppendLine($"{name}.{nameof(IEntity.DisplayName)} = {'"'}{DisplayName}{'"'};");
            }
            foreach (var pair in Components)
            {
                string componentName = getComponentName(pair.Key.Name);
                pair.Value.GenerateCode(componentName, code);
            }
        }

        private string getComponentName(string interfaceName)
        {
            if (interfaceName.StartsWith("I")) interfaceName = interfaceName.Substring(1);
            interfaceName = $"{char.ToLower(interfaceName[0])}{interfaceName.Substring(1)}";
            return getVariableName(interfaceName);
        }

        private string getVariableName(string name)
        {
            StringBuilder varName = new StringBuilder(name.Length);
            foreach (var c in name)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '_') continue;
                varName.Append(c);
            }
            if (varName.Length == 0) throw new ArgumentException($"{name} is not a legal name: it must have at least one letter or digit");
            return varName[0] == '_' ? varName.ToString() : $"_{varName.ToString()}";
        }

        public static EntityModel Load(string path) => AGSProject.LoadJson<EntityModel>(path);
    }
}