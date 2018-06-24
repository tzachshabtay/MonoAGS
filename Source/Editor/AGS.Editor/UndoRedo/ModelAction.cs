using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class ModelAction
    {
        public static Action Execute(StateModel model, params (IComponent component, string propertyName, object value)[] properties)
        {
            if (properties.Length == 0 || properties[0].component?.Entity == null)
                return null;

            var entityModel = getEntity(properties[0].component.Entity, model);
            bool wasDirty = entityModel.IsDirty;

            var oldProperties = new List<(ComponentModel componentModel, string name, bool hadValue, object oldValue)>();
            foreach ((IComponent component, string propertyName, object value) in properties)
            {
                var componentModel = entityModel.Components.GetOrAdd(component.RegistrationType, _ => new ComponentModel
                    { ComponentConcreteType = component.GetType(), Properties = new Dictionary<string, object>() });

                if (componentModel.Properties.TryGetValue(propertyName, out object oldValue))
                {
                    oldProperties.Add((componentModel, propertyName, true, oldValue));
                }
                else
                {
                    oldProperties.Add((componentModel, propertyName, false, oldValue));
                }
                componentModel.Properties[propertyName] = value;
                entityModel.IsDirty = true;
            }

            Action undoModel = () =>
            {
                foreach ((ComponentModel componentModel, string name, bool hadValue, object oldValue) in oldProperties)
                {
                    if (hadValue) componentModel.Properties[name] = oldValue;
                    else componentModel.Properties.Remove(name);
                }
                if (!wasDirty) entityModel.IsDirty = false;
            };
            return undoModel;
        }

        private static EntityModel getEntity(IEntity entity, StateModel model)
        {
            return model.Entities.GetOrAdd(entity.ID, id =>
            {
                var e = new EntityModel
                {
                    ID = id,
                    DisplayName = entity.DisplayName,
                    EntityConcreteType = entity.GetType(),
                    Components = new Dictionary<Type, ComponentModel>()
                };
                var room = entity.GetComponent<IHasRoomComponent>()?.Room;
                if (room != null)
                {
                    var roomModel = getRoom(room, model);
                    if (roomModel.BackgroundEntity != entity.ID)
                        roomModel.Entities.Add(entity.ID);
                }
                else
                {
                    model.Guis.Add(entity.ID);
                }
                return e;
            });
        }

        private static RoomModel getRoom(IRoom room, StateModel model)
        {
            var roomModel = model.Rooms.FirstOrDefault(r => r.ID == room.ID);
            if (roomModel == null)
            {
                roomModel = new RoomModel { ID = room.ID, BackgroundEntity = room.Background?.Entity?.ID, Entities = new HashSet<string>() };
                model.Rooms.Add(roomModel);
            }
            return roomModel;
        }
    }
}