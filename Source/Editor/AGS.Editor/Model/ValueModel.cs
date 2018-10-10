using System;
using System.Runtime.Serialization;

namespace AGS.Editor
{
    [DataContract]
    public class ValueModel
    {
        [DataMember(Name = "Value")]
        public object Value { get; set; }

        [DataMember(Name = "Initializer")]
        public MethodModel Initializer { get; set; }

        public override string ToString() => Value?.ToString() ?? InspectorProperty.NullValue;
    }
}
