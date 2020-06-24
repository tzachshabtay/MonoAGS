﻿using System.Collections.Generic;
using System.Reflection;

namespace AGS.Editor
{
    public class MethodTypeDescriptor : ITypeDescriptor
    {
        private readonly MethodBase _method;
        private readonly Dictionary<InspectorCategory, List<IProperty>> _props;
        private readonly HashSet<string> _hideProperties;
        private readonly Dictionary<string, object> _overrideDefaults;

        public MethodTypeDescriptor(MethodBase method, HashSet<string> hideProperties, Dictionary<string, object> overrideDefaults)
        {
            _method = method;
            _hideProperties = hideProperties;
            _overrideDefaults = overrideDefaults;
            _props = new Dictionary<InspectorCategory, List<IProperty>>();
        }

        public Dictionary<InspectorCategory, List<IProperty>> GetProperties()
        {
            var parameters = _method.GetParameters();
            if (parameters.Length == 0) return _props;

            InspectorCategory cat = new InspectorCategory("General");
            _props[cat] = new List<IProperty>();
            foreach (var parameter in parameters)
            {
                if (_hideProperties.Contains(parameter.Name)) continue;

                _overrideDefaults.TryGetValue(parameter.Name, out var overrideDefault);
                var param = new MethodParam(parameter, null, overrideDefault);
                _props[cat].Add(param);
                ObjectTypeDescriptor.RefreshChildrenProperties(param);
            }
            return _props;
        }
    }
}
