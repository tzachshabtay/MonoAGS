using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using Humanizer;

namespace AGS.Editor
{
    public class ObjectTypeDescriptor : ITypeDescriptor
    {
        private readonly object _obj;
        private readonly Dictionary<InspectorCategory, List<IProperty>> _props;

        public ObjectTypeDescriptor(object obj)
        {
            _obj = obj;
            _props = new Dictionary<InspectorCategory, List<IProperty>>();
        }

        public Dictionary<InspectorCategory, List<IProperty>> GetProperties()
        {
            InspectorCategory cat = new InspectorCategory("General");
            AddProperties(cat, _obj, _props);
            return _props;
        }

        public static void AddProperties(InspectorCategory defaultCategory, object obj, Dictionary<InspectorCategory, List<IProperty>> properties)
        {
            var props = getProperties(obj.GetType());
            foreach (var prop in props)
            {
                var cat = defaultCategory;
                InspectorProperty property = AddProperty(obj, obj as IComponent, null, prop, ref cat);
                if (property == null) continue;
                properties.GetOrAdd(cat, () => new List<IProperty>(props.Length)).Add(property);
            }
        }

        public static InspectorProperty AddProperty(object obj, IComponent component, IProperty parent, PropertyInfo prop, ref InspectorCategory cat)
        {
            var attr = prop.GetCustomAttribute<PropertyAttribute>();
            if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
            if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IBlockingEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
            string name = prop.Name;
            string displayName = null;
            bool forceReadonly = false;
            if (attr != null)
            {
                if (!attr.Browsable) return null;
                if (attr.Category != null) cat = new InspectorCategory(attr.Category, attr.CategoryZ, attr.CategoryExpand);
                displayName = attr.DisplayName;
                forceReadonly = attr.ForceReadonly;
            }
            InspectorProperty property = new InspectorProperty(component, obj, parent, name, prop, displayName, forceReadonly);
            RefreshChildrenProperties(property);
            return property;
        }

        public static void RefreshChildrenProperties(IProperty property)
        {
            property.Children.Clear();
            var val = property.Value.Value;
            if (val == null) return;
            var objType = val.GetType();
            if (objType.GetTypeInfo().GetCustomAttribute<PropertyFolderAttribute>(true) != null)
            {
                var props = getProperties(objType);
                foreach (var childProp in props)
                {
                    InspectorCategory dummyCat = null;
                    var childProperty = AddProperty(val, property.Component, property, childProp, ref dummyCat);
                    if (childProperty == null) continue;
                    property.Children.Add(childProperty);
                }
            }
        }

        private static PropertyInfo[] getProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}