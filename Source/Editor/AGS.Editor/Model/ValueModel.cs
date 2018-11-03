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
        public ValueModel(object value, MethodModel initializer = null, Dictionary<string, ValueModel> children = null)
        {
            Value = value;
            Initializer = initializer;
            Children = children ?? new Dictionary<string, ValueModel>();
        }

        [DataMember(Name = "Value")]
        public object Value { get; }

        [DataMember(Name = "Initializer")]
        public MethodModel Initializer { get; }

        [DataMember(Name = "Children")]
        public Dictionary<string, ValueModel> Children { get; }

        public override string ToString() => Value?.ToString() ?? InspectorProperty.NullValue;
    }
}