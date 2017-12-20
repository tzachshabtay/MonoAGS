using System.Collections.Generic;
using System.Collections.Concurrent;
using AGS.API;

namespace AGS.Engine
{
    public class AGSCustomPropertiesPerType<TValue> : ICustomPropertiesPerType<TValue>
    {
        private readonly ConcurrentDictionary<string, TValue> _properties;

        public AGSCustomPropertiesPerType()
        {
            _properties = new ConcurrentDictionary<string, TValue>();
        }

        public TValue GetValue(string name, TValue defaultValue = default)
        {
            //Tempting to use GetOrAdd here, but unfortunately it results in memory allocation which we want to avoid as this can be part of a critical path
            if (!_properties.TryGetValue(name, out var val))
            {
                val = defaultValue;
                _properties.TryAdd(name, val);
            }
            return val;
        }

        public void SetValue(string name, TValue value)
        {
            _properties[name] = value;
        }

        public IDictionary<string, TValue> AllProperties()
        {
            return _properties;
        }

        public void CopyFrom(ICustomPropertiesPerType<TValue> properties)
        {
            foreach (var property in properties.AllProperties())
            {
                _properties[property.Key] = property.Value;
            }
        }
    }
}
