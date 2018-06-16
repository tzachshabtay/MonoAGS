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
        public Dictionary<string, object> Properties { get; set; }

        public void GenerateCode(string name, StringBuilder code)
        {
            foreach (var pair in Properties)
            {
                code.AppendLine($"{name}.{pair.Key} = {getValueString(pair.Value)};");
            }
        }

        private string getValueString(object val)
        {
            switch (val)
            {
                case null:
                    return "null";
                case float f:
                    return $"{f.ToString()}f";
                case double d1:
                    return $"{d1.ToString()}d";
                case decimal d2:
                    return $"{d2.ToString()}m";
                case byte v1:
                case int v2:
                case uint v3:
                case long v4:
                case ulong v5:
                case sbyte v6:
                case short v7:
                case ushort v8:
                    return val.ToString();
                case string s:
                    return $"{'"'}{s}{'"'}";
                case char c:
                    return $"{"'"}{c}{"'"}";
                case bool b:
                    return b.ToString().ToLowerInvariant();
                default:
                    string valueString = val.ToString();
                    Type type = val.GetType();
                    if (type.IsEnum)
                    {
                        return $"{type.Name}.{valueString}";
                    }
                    if (valueString.Contains(","))
                    { //assuming tuple for now
                        return $"({valueString})";
                    }
                    return valueString;
            }
        }
    }
}