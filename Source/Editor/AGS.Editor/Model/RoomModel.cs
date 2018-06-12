using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AGS.Editor
{
    public class RoomModel
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string BackgroundEntity { get; set; }

        public HashSet<string> Entities { get; set; }

        public static RoomModel Load(string path) => AGSProject.LoadJson<RoomModel>(path);
    }
}