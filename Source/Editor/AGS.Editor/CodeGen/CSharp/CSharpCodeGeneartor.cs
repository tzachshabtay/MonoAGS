﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AGS.API;

namespace AGS.Editor
{
	public class CSharpCodeGeneartor : ICodeGenerator
    {
        private StateModel _stateModel;

        public CSharpCodeGeneartor(StateModel stateModel)
        {
            _stateModel = stateModel;    
        }

        public void GenerateCode(string namespaceName, EntityModel model, StringBuilder code)
        {
            if (!model.IsDirty || model.Parent != null) return;
            code.AppendLine(generateClass(model, namespaceName));
        }

        private string generateDisplayName(EntityModel model)
        {
            if (model.DisplayName == null) return "";
            return $"{model.ScriptName}.{nameof(IEntity.DisplayName)} = {'"'}{model.DisplayName}{'"'};";
        }

        private string generateModels(EntityModel model, Func<EntityModel, string> generateLine, int numIndents = 12)
        {
            StringBuilder code = new StringBuilder();
            List<EntityModel> models = new List<EntityModel>();
            getAllModels(model, models);
            foreach (var entity in models)
            {
                string line = generateLine(entity);
                if (!string.IsNullOrEmpty(line))
                {
                    indent(code, numIndents).AppendLine(line);
                }
            }
            return code.ToString();
        }

        private void getAllModels(EntityModel model, List<EntityModel> models)
        {
            if (model == null) return;
            models.Add(model);
            foreach (var child in model.Children)
            {
                getAllModels(_stateModel.Entities[child], models);
            }
        }

        private string generateEntityMember(EntityModel model)
        {
            if (model.Initializer == null && !needEntity(model)) return "";
            return $"private {getType(model)} {model.ScriptName};";
        }

        private string generateEntityFactory(EntityModel model)
        {
            if (model.Initializer == null)
            {
                if (!needEntity(model)) return "";
                return $"{model.ScriptName} = ({getType(model)}){getRoot(model).ScriptName}.TreeNode.FindDescendant({'"'}{model.ID}{'"'});";
            }
            return $"{model.ScriptName} = {generateMethod(model.Initializer)}";
        }

        private EntityModel getRoot(EntityModel model)
        {
            if (model.Parent == null) return model;
            return getRoot(_stateModel.Entities[model.Parent]);
        }

        private bool needEntity(EntityModel model) => model.Components.Any(c => c.Value.Properties.Count > 0);

        private string getType(EntityModel model)
        {
            return model.Initializer?.ReturnType.Name ?? model.EntityConcreteType.Name;
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

        private void generateScriptNames(EntityModel model)
        {
            List<EntityModel> models = new List<EntityModel>();
            getAllModels(model, models);
            foreach (var entity in models)
            {
                entity.ScriptName = getClassMemberName(entity.ID);
            }
        }

        private string generateClass(EntityModel model, string namespaceName)
        {
            generateScriptNames(model);
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
{generateModels(model, generateEntityMember, 8)}

        public {className}(IGameState state, IGameFactory factory)
        {{
            _state = state;
            _factory = factory;
        }}

        public void Load()
        {{
{generateModels(model, generateEntityFactory)}
{generateModels(model, generateDisplayName)}
{generateModels(model, generateComponents, 0)}
        }}
    }}
}}";
        }

        private StringBuilder indent(StringBuilder code, int chars = 12)
        {
            for (int i = 0; i < chars; i++) code.Append(' ');
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