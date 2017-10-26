using System;
using System.Collections.Generic;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    public class InspectorProperty
    {
        private static Dictionary<Type, Tuple<MethodInfo, CustomStringValueAttribute>> _customValueProviders = 
            new Dictionary<Type, Tuple<MethodInfo, CustomStringValueAttribute>>();

		public InspectorProperty(object obj, string name, PropertyInfo prop)
		{
			Prop = prop;
			Name = name;
			Object = obj;
			Children = new List<InspectorProperty>();
            IsReadonly = prop.SetMethod == null || !prop.SetMethod.IsPublic;
            Refresh();
		}

		public string Name { get; private set; }
		public string Value { get; private set; }
		public PropertyInfo Prop { get; private set; }
		public object Object { get; private set; }
		public List<InspectorProperty> Children { get; private set; }
        public bool IsReadonly { get; private set; }

        public const string NullValue = "(null)";

		public void Refresh()
		{
			object val = Prop.GetValue(Object);
            if (val == null) Value = NullValue;
            else
            {
                var provider = _customValueProviders.GetOrAdd(Prop.PropertyType, () =>
                {
                    var methods = Prop.PropertyType.GetRuntimeMethods();
                    foreach (var candidate in methods)
                    {
                        var attr = candidate.GetCustomAttribute<CustomStringValueAttribute>();
                        if (attr != null) return new Tuple<MethodInfo, CustomStringValueAttribute>(candidate, attr);
                    }
                    return null;
                });
                MethodInfo method = getCustomStringMethod(provider);
                Value = method == null ? val.ToString() : method.Invoke(val, null).ToString();
            }
		}

        private MethodInfo getCustomStringMethod(Tuple<MethodInfo, CustomStringValueAttribute> provider)
        {
            if (provider == null) return null;
            switch (provider.Item2.ApplyWhen)
            {
                case CustomStringApplyWhen.Both:
                    return provider.Item1;
                case CustomStringApplyWhen.CanWrite:
                    return IsReadonly ? null : provider.Item1;
                case CustomStringApplyWhen.ReadOnly:
                    return IsReadonly ? provider.Item1 : null;
                default:
                    throw new NotSupportedException(provider.Item2.ApplyWhen.ToString());
            }
        }
    }
}
