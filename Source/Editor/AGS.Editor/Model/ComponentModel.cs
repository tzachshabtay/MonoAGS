using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AGS.Editor
{
    public class ComponentModel
    {
        [DataMember]
        public Type ComponentConcreteType { get; set; }

        [DataMember]
        public Dictionary<string, string> Properties { get; set; }

        public void GenerateCode(string name, StringBuilder code)
        {
            foreach (var pair in Properties)
            {
                code.AppendLine($"{name}.{pair.Key} = {pair.Value}");
            }
        }
    }
}