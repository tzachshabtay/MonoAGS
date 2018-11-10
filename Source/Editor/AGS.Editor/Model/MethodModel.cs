using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AGS.Editor
{
    [DataContract]
    public class MethodModel
    {
        [DataMember(Name = "InstanceName")]
        public string InstanceName { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Parameters")]
        public ValueModel[] Parameters { get; set; }

        [DataMember(Name = "ReturnType")]
        public Type ReturnType { get; set; }
    }
}