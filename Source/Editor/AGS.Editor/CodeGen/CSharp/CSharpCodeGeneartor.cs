using System;
using System.Globalization;
using System.Linq;
using System.Text;
using AGS.API;

namespace AGS.Editor
{
	public class CSharpCodeGeneartor : ICodeGenerator
    {
        public void GenerateCode(string namespaceName, EntityModel model, StringBuilder code)
        {
            if (!model.IsDirty) return;
            (string name, string className) = getNames(model.ID);
            generateHeader(namespaceName, className, code);
            if (model.Initializer != null)
            {
                indent(code).Append($"{name} = ");
                generateCode(model.Initializer, code);
            }
            if (model.DisplayName != null)
            {
                indent(code).AppendLine($"{name}.{nameof(IEntity.DisplayName)} = {'"'}{model.DisplayName}{'"'};");
            }
            foreach (var pair in model.Components)
            {
                string componentName = getComponentName(pair.Key.Name);
                if (!pair.Key.IsAssignableFrom(model.EntityConcreteType))
                {
                    indent(code);
                    bool needVar = pair.Value.Properties.Count > 0;
                    if (needVar) code.Append($"var {componentName} = ");
                    code.AppendLine($"{name}.AddComponent<{pair.Key.Name}>();");
                }
                generateCode(pair.Value, componentName, code);
            }
            generateFooter(code);
        }

        private void generateHeader(string namespaceName, string className, StringBuilder code)
        {
            code.AppendLine(
$@"using AGS.API;
using AGS.Engine;
using System;
using System.Linq;
using System.Text;
using using System.Collections.Generic;

namespace {namespaceName}
{{
    public class {className}
    {{
        private IGameState _state;
        private IGameFactory _factory;

        public {className} (IGameState state, IGameFactory factory)
        {{
            _state = state;
            _factory = factory;
        }}

        public void Load()
        {{");
        }

        private void generateFooter(StringBuilder code)
        {
            code.AppendLine(
$@"        }}
    }}
}}");
        }

        private StringBuilder indent(StringBuilder code)
        {
            code.Append("            ");
            return code;
        }

        private void generateCode(MethodModel model, StringBuilder code)
        {
            var parameters = string.Join(", ", model.Parameters.Select(p => getValueString(p)));
            if (string.IsNullOrEmpty(model.InstanceName))
            {
                code.AppendLine($"{model.Name}({parameters});");
                return;
            }
            code.AppendLine($"{model.InstanceName}.{model.Name}({parameters});");
        }

        private void generateCode(ComponentModel model, string name, StringBuilder code)
        {
            foreach (var pair in model.Properties)
            {
                indent(code).AppendLine($"{name}.{pair.Key} = {getValueString(pair.Value)};");
            }
        }

        private string getComponentName(string interfaceName)
        {
            if (interfaceName.StartsWith("I", false, CultureInfo.InvariantCulture)) interfaceName = interfaceName.Substring(1);
            interfaceName = $"{char.ToLower(interfaceName[0])}{interfaceName.Substring(1)}";
            return getNames(interfaceName).varName;
        }

        private (string varName, string className) getNames(string name)
        {
            StringBuilder sb = new StringBuilder(name.Length);
            foreach (var c in name)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '_') continue;
                sb.Append(c);
            }
            if (sb.Length == 0) throw new ArgumentException($"{name} is not a legal name: it must have at least one letter or digit");
            string varName = sb[0] == '_' ? sb.ToString() : $"_{sb.ToString()}";
            string className = varName.Substring(1, 1).ToUpperInvariant() + (varName.Length > 2 ? varName.Substring(2) : "");
            return (varName, className);
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