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

        private MethodModel _initializer;

        private bool _isDefault = true;

		public InspectorProperty(IComponent obj, IProperty parent, string name, PropertyInfo prop, string displayName = null)
            : this(obj, obj, parent, name, prop, displayName)
        {}

        public InspectorProperty(IComponent component, object obj, IProperty parent, string name, PropertyInfo prop, string displayName = null)
        {
			Prop = prop;
			Name = name;
            DisplayName = displayName ?? name.Humanize();
			Object = obj;
            Component = component;
            Parent = parent;
			Children = new List<IProperty>();
            IsReadonly = prop.SetMethod == null || !prop.SetMethod.IsPublic;
            Refresh();
		}

		public string Name { get; }
        public string DisplayName { get; }
		public string ValueString { get; private set; }
		public PropertyInfo Prop { get; private set; }
		public object Object { get; }
        public IComponent Component { get; }
        public IProperty Parent { get; }
        public List<IProperty> Children { get; }
        public bool IsReadonly { get; }

        public Type PropertyType => Prop.PropertyType;

        public const string NullValue = "(None)";

        public ValueModel Value
        {
            get => new ValueModel(Prop.GetValue(Object, null), _initializer, isDefault: _isDefault, type: PropertyType);
            set
            {
                _isDefault = false;
                Prop.SetValue(Object, value.Value, null);
                _initializer = value.Initializer;
            }
        }

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

        public override string ToString() => $"{DisplayName}: {ValueString}";

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
