using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AGS.Editor
{
    public class ComponentModel
    {
        [DataMember]
        public Type ComponentConcreteType { get; set; }

        [DataMember]
        public Dictionary<string, object> Properties { get; set; }
    }
}