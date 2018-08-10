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
            model.ScriptName = getClassMemberName(model.ID);
            code.AppendLine(generateClass(model, namespaceName));
        }

        private string generateDisplayName(EntityModel model)
        {
            if (model.DisplayName == null) return "";
            return $"{model.ScriptName}.{nameof(IEntity.DisplayName)} = {'"'}{model.DisplayName}{'"'};";
        }

        private string generateEntityMember(EntityModel model)
        {
            if (model.Initializer == null) return "";
            return $"private {model.Initializer.ReturnType.Name} {model.ScriptName}";
        }

        private string generateEntityFactory(EntityModel model)
        {
            if (model.Initializer == null) return "";
            return $"{model.ScriptName} = {generateMethod(model.Initializer)}";
        }

        private string generateComponents(EntityModel model)
        {
            StringBuilder code = new StringBuilder();
            foreach (var pair in model.Components)
            {
                bool isBuiltIn = pair.Key.IsAssignableFrom(model.EntityConcreteType);
                string componentName = isBuiltIn ? model.ScriptName : getComponentName(pair.Key.Name);
                if (!isBuiltIn)
                {
                    indent(code);
                    bool needVar = pair.Value.Properties.Count > 0;
                    if (needVar) code.Append($"var {componentName} = ");
                    code.AppendLine($"{model.ScriptName}.AddComponent<{pair.Key.Name}>();");
                }
                generateSetProperty(pair.Value, componentName, code);
            }
            return code.ToString();
        }

        private string generateClass(EntityModel model, string namespaceName)
        {
            string className = getClassName(model.ID);
            return
$@"using AGS.API;
using AGS.Engine;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace {namespaceName}
{{
    public class {className}
    {{
        private readonly IGameState _state;
        private readonly IGameFactory _factory;
        {generateEntityMember(model)}

        public {className} (IGameState state, IGameFactory factory)
        {{
            _state = state;
            _factory = factory;
        }}

        public void Load()
        {{
            {generateEntityFactory(model)}
            {generateDisplayName(model)}
{generateComponents(model)}
        }}
    }}
}}";
        }

        private StringBuilder indent(StringBuilder code)
        {
            code.Append("            ");
            return code;
        }

        private string generateMethod(MethodModel model)
        {
            var parameters = string.Join(", ", model.Parameters.Select(p => getValueString(p)));
            if (string.IsNullOrEmpty(model.InstanceName))
            {
                return $"{model.Name}({parameters});";
            }
            return $"{model.InstanceName}.{model.Name}({parameters});";
        }

        private void generateSetProperty(ComponentModel model, string name, StringBuilder code)
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
            return getRawName(interfaceName);
        }

        private string getRawName(string name)
        {
            StringBuilder sb = new StringBuilder(name.Length);
            foreach (var c in name)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '_') continue;
                sb.Append(c);
            }
            if (sb.Length == 0) throw new ArgumentException($"{name} is not a legal name: it must have at least one letter or digit");
            return sb.ToString();
        }

        private string getClassMemberName(string name)
        {
            name = getRawName(name);
            return name[0] == '_' ? name : $"_{name}";
        }

        private string getClassName(string name)
        {
            name = getRawName(name);
            var suffix = name.Length > 1 ? name.Substring(1) : "";
            return $"{char.ToUpperInvariant(name[0])}{suffix}";
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