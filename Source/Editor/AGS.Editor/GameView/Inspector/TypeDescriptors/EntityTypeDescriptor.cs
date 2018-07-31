using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class EntityTypeDescriptor : ITypeDescriptor
    {
        private readonly IEntity _entity;
        private readonly Dictionary<InspectorCategory, List<IProperty>> _props;

        public EntityTypeDescriptor(IEntity entity)
        {
            _entity = entity;
            _props = new Dictionary<InspectorCategory, List<IProperty>>();
        }

        public Dictionary<InspectorCategory, List<IProperty>> GetProperties()
        {
            foreach (var component in _entity)
            {
                InspectorCategory cat = new InspectorCategory(component.Name);
                ObjectTypeDescriptor.AddProperties(cat, component, _props);
            }
            addEntityProps(_entity);
            return _props;
        }

        private void addEntityProps(IEntity entity)
        {
            InspectorCategory cat = new InspectorCategory("Hotspot");
            var prop = entity.GetType().GetProperty(nameof(IEntity.DisplayName));
            InspectorProperty property = ObjectTypeDescriptor.AddProperty(entity, prop, ref cat);
            if (property == null) return;
            _props.GetOrAdd(cat, () => new List<IProperty>()).Add(property);
        }
    }
}