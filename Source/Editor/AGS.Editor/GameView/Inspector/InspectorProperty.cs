using System;
using System.Collections.Generic;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using Humanizer;

namespace AGS.Editor
{
    public class InspectorProperty : IProperty
    {
        private static Dictionary<Type, (MethodInfo, CustomStringValueAttribute)> _customValueProviders = 
            new Dictionary<Type, (MethodInfo, CustomStringValueAttribute)>();

		public InspectorProperty(object obj, string name, PropertyInfo prop, string displayName = null)
		{
			Prop = prop;
			Name = name;
            DisplayName = displayName ?? name.Humanize();
			Object = obj;
			Children = new List<IProperty>();
            IsReadonly = prop.SetMethod == null || !prop.SetMethod.IsPublic;
            Refresh();
		}

		public string Name { get; }
        public string DisplayName { get; }
		public string ValueString { get; private set; }
		public PropertyInfo Prop { get; private set; }
		public object Object { get; }
		public List<IProperty> Children { get; }
        public bool IsReadonly { get; }

        public Type PropertyType => Prop.PropertyType;

        public const string NullValue = "(None)";

        public object Value { get => Prop.GetValue(Object, null); set => Prop.SetValue(Object, value, null); }

        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            return Prop.GetCustomAttribute<TAttribute>();
        }

		public void Refresh()
		{
            try
            {
                object val = Prop.GetValue(Object);
                if (val == null) ValueString = NullValue;
                else
                {
                    var provider = _customValueProviders.GetOrAdd(Prop.PropertyType, () =>
                    {
                        var methods = Prop.PropertyType.GetRuntimeMethods();
                        foreach (var candidate in methods)
                        {
                            var attr = candidate.GetCustomAttribute<CustomStringValueAttribute>();
                            if (attr != null) return (candidate, attr);
                        }
                        return default;
                    });
                    MethodInfo method = getCustomStringMethod(provider);
                    ValueString = method == null ? val.ToString() : method.Invoke(val, null).ToString();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to refresh property: {Name}", e);
            }
		}

        private MethodInfo getCustomStringMethod((MethodInfo method, CustomStringValueAttribute attr)provider)
        {
            if (provider.Equals(default)) return null;
            switch (provider.attr.ApplyWhen)
            {
                case CustomStringApplyWhen.Both:
                    return provider.method;
                case CustomStringApplyWhen.CanWrite:
                    return IsReadonly ? null : provider.method;
                case CustomStringApplyWhen.ReadOnly:
                    return IsReadonly ? provider.method : null;
                default:
                    throw new NotSupportedException(provider.attr.ApplyWhen.ToString());
            }
        }
    }
}
