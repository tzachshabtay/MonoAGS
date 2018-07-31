using System;
using System.Collections.Generic;
using System.Reflection;

namespace AGS.Editor
{
	public class MethodParam : IProperty
    {
        private readonly ParameterInfo _param;
        private object _value;

        public MethodParam(ParameterInfo parameter, object obj, object overrideDefault = null)
        {
            _param = parameter;
            Object = obj;
            Children = new List<IProperty>();
            _value = overrideDefault ?? (parameter.HasDefaultValue ? parameter.DefaultValue : GetDefaultValue(PropertyType));
        }

        public string Name => _param.Name;

        public object Object { get; }

        public string ValueString => GetValue()?.ToString() ?? "null";

        public Type PropertyType => _param.ParameterType;

        public List<IProperty> Children { get; private set; }

        public bool IsReadonly => false;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _param.GetCustomAttribute<TAttribute>();
        }

        public object GetValue() => _value;

        public void SetValue(object value) => _value = value;

        public void Refresh() {}

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }
    }
}