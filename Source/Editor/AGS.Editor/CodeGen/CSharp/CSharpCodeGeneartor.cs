using System;
using System.Text;
using AGS.API;

namespace AGS.Editor
{
	public class CSharpCodeGeneartor : ICodeGenerator
    {
        public void GenerateCode(EntityModel model, StringBuilder code)
        {
            if (!model.IsDirty) return;
            string name = getVariableName(model.ID);
            if (model.DisplayName != null)
            {
                code.AppendLine($"{name}.{nameof(IEntity.DisplayName)} = {'"'}{model.DisplayName}{'"'};");
            }
            foreach (var pair in model.Components)
            {
                string componentName = getComponentName(pair.Key.Name);
                generateCode(pair.Value, componentName, code);
            }
        }

        private void generateCode(ComponentModel model, string name, StringBuilder code)
        {
            foreach (var pair in model.Properties)
            {
                code.AppendLine($"{name}.{pair.Key} = {getValueString(pair.Value)};");
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