using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AGS.Editor
{
    [DataContract]
    public class ValueModel
    {
        [JsonConstructor]
        public ValueModel(object value, MethodModel initializer = null, Dictionary<string, ValueModel> children = null, bool isDefault = false)
        {
            Value = value;
            Initializer = initializer;
            Children = children ?? new Dictionary<string, ValueModel>();
            IsDefault = isDefault;
        }

        [DataMember(Name = "Value")]
        public object Value { get; }

        [DataMember(Name = "Initializer")]
        public MethodModel Initializer { get; }

        [DataMember(Name = "Children")]
        public Dictionary<string, ValueModel> Children { get; }

        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; }

        public override string ToString() => Value?.ToString() ?? InspectorProperty.NullValue;
    }
}