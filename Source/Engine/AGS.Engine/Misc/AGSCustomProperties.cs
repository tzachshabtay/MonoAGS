using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSCustomProperties : AGSComponent, ICustomProperties
    {
        private AGSCustomPropertiesPerType<int> _ints;
        private AGSCustomPropertiesPerType<float> _floats;
        private AGSCustomPropertiesPerType<string> _strings;
        private AGSCustomPropertiesPerType<bool> _bools;
        private AGSCustomPropertiesPerType<IEntity> _entities;

        #region ICustomProperties implementation

        public ICustomPropertiesPerType<int> Ints { get { _ints = _ints ?? new AGSCustomPropertiesPerType<int>(); return _ints; } }
        public ICustomPropertiesPerType<float> Floats { get { _floats = _floats ?? new AGSCustomPropertiesPerType<float>(); return _floats; } }
        public ICustomPropertiesPerType<string> Strings { get { _strings = _strings ?? new AGSCustomPropertiesPerType<string>(); return _strings; } }
        public ICustomPropertiesPerType<bool> Bools { get { _bools = _bools ?? new AGSCustomPropertiesPerType<bool>(); return _bools; } }
        public ICustomPropertiesPerType<IEntity> Entities { get { _entities = _entities ?? new AGSCustomPropertiesPerType<IEntity>(); return _entities; } }

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

        public override string ToString() => "Custom Properties";

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