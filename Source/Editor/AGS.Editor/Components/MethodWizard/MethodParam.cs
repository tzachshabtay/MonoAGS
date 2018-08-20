using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace AGS.Editor
{
	public class MethodParam : IProperty, INotifyPropertyChanged
    {
        private readonly ParameterInfo _param;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public MethodParam(ParameterInfo parameter, object obj, object overrideDefault = null)
        {
            _param = parameter;
            Object = obj;
            Children = new List<IProperty>();
            Value = overrideDefault ?? (parameter.HasDefaultValue ? parameter.DefaultValue : GetDefaultValue(PropertyType));
        }

        public string Name => _param.Name;

        public object Object { get; }

        public string ValueString => Value?.ToString() ?? "null";

        public Type PropertyType => _param.ParameterType;

        public List<IProperty> Children { get; private set; }

        public bool IsReadonly => false;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _param.GetCustomAttribute<TAttribute>();
        }

        public object Value { get; set; }

        public void Refresh() {}

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }
    }
}