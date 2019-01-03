using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AGS.Editor
{
    public class ComponentModel
    {
        [DataMember]
        public Type ComponentConcreteType { get; set; }

        [DataMember]
        public Dictionary<string, ValueModel> Properties { get; set; }
    }
}