using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AGS.Editor
{
    [DataContract]
    public class ValueModel
    {
        [JsonConstructor]
        public ValueModel(object value, MethodModel initializer = null)
        {
            Value = value;
            Initializer = initializer;
        }

        [DataMember(Name = "Value")]
        public object Value { get; }

        [DataMember(Name = "Initializer")]
        public MethodModel Initializer { get; }

        public override string ToString() => Value?.ToString() ?? InspectorProperty.NullValue;
    }
}
