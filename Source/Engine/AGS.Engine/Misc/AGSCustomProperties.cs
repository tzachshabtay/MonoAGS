using System;
using AGS.API;
using System.Collections.Concurrent;

namespace AGS.Engine
{
    public class AGSCustomProperties : AGSComponent, ICustomProperties
    {
        private readonly ICustomPropertiesPerType<int> _ints;
        private readonly ICustomPropertiesPerType<float> _floats;
        private readonly ConcurrentDictionary<string, string> _strings;
        private readonly ConcurrentDictionary<string, bool> _bools;

        public AGSCustomProperties()
        {
            Ints = new AGSCustomPropertiesPerType<int>();
            Floats = new AGSCustomPropertiesPerType<float>();
            Strings = new AGSCustomPropertiesPerType<string>();
            Bools = new AGSCustomPropertiesPerType<bool>();
            Entities = new AGSCustomPropertiesPerType<IEntity>();
        }

        #region ICustomProperties implementation

        public ICustomPropertiesPerType<int> Ints { get; private set; }
        public ICustomPropertiesPerType<float> Floats { get; private set; }
        public ICustomPropertiesPerType<string> Strings { get; private set; }
        public ICustomPropertiesPerType<bool> Bools { get; private set; }
        public ICustomPropertiesPerType<IEntity> Entities { get; private set; }

        public void RegisterCustomData(ICustomSerializable customData)
        {
            throw new NotImplementedException();
        }

        public void CopyFrom(ICustomProperties properties)
        {
            if (properties == null) return;
            Ints.CopyFrom(properties.Ints);
            Floats.CopyFrom(properties.Floats);
            Strings.CopyFrom(properties.Strings);
            Bools.CopyFrom(properties.Bools);
            Entities.CopyFrom(properties.Entities);
		}

		#endregion
	}

    public class AGSCustomPropertiesComponent : AGSComponent, ICustomPropertiesComponent
    {
        public AGSCustomPropertiesComponent(ICustomProperties properties)
        {
            Properties = properties;
        }

        public ICustomProperties Properties { get; private set; }
    }
}

