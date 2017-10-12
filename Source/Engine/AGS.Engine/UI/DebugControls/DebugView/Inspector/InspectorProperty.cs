using System;
using System.Collections.Generic;
using System.Reflection;

namespace AGS.Engine
{
    public class InspectorProperty
    {
		public InspectorProperty(object obj, string name, PropertyInfo prop)
		{
			Prop = prop;
			Name = name;
			Object = obj;
			Children = new List<InspectorProperty>();
			Refresh();
		}

		public string Name { get; private set; }
		public string Value { get; private set; }
		public PropertyInfo Prop { get; private set; }
		public object Object { get; private set; }
		public List<InspectorProperty> Children { get; private set; }

		public void Refresh()
		{
			object val = Prop.GetValue(Object);
			Value = val == null ? "(null)" : val.ToString();
		}
    }
}
