using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class ModelAction
    {
        /// <summary>
        /// Updates the state model with a list of property + values, and prepares an undo action (if the user selects to undo).
        /// </summary>
        /// <returns>The execute.</returns>
        /// <param name="model">Model.</param>
        /// <param name="properties">Properties.</param>
        public static Action Execute(StateModel model, params (IComponent component, string propertyName, ValueModel value, List<IProperty> propertyChain)[] properties)
        {
            if (properties.Length == 0 || properties[0].component?.Entity == null)
                return null;

            var entityModel = GetEntity(properties[0].component.Entity, model);
            bool wasDirty = entityModel.IsDirty;

            var oldProperties = new List<(ComponentModel componentModel, string name, bool hadValue, ValueModel oldValue, ValueModel parentValue)>();
            foreach ((IComponent component, string propertyName, ValueModel value, List<IProperty> propertyChain) in properties)
            {
                Trace.Assert(component != null);
                var componentModel = entityModel.Components.GetOrAdd(component.RegistrationType, _ => new ComponentModel
                    { ComponentConcreteType = component.GetType(), Properties = new Dictionary<string, ValueModel>() });

                if (propertyChain == null) //the property is a direct child of the component
                {
                    if (componentModel.Properties.TryGetValue(propertyName, out ValueModel oldValue))
                    {
                        oldProperties.Add((componentModel, propertyName, true, oldValue, null));
                    }
                    else
                    {
                        oldProperties.Add((componentModel, propertyName, false, oldValue, null));
                    }
                    componentModel.Properties[propertyName] = value;
                }
                else //The property is nested inside the component, the property chain gives the nesting information
                {
                    ValueModel parentValue = getParent(propertyChain, componentModel);
                    if (parentValue.Children.TryGetValue(propertyName, out ValueModel oldValue))
                    {
                        oldProperties.Add((componentModel, propertyName, true, oldValue, parentValue));
                    }
                    else
                    {
                        oldProperties.Add((componentModel, propertyName, false, oldValue, parentValue));
                    }
                    parentValue.Children[propertyName] = value;
                }
                entityModel.IsDirty = true;
            }

            Action undoModel = () =>
            {
                foreach ((ComponentModel componentModel, string name, bool hadValue, ValueModel oldValue, ValueModel parentValue) in oldProperties)
                {
                    if (parentValue == null)
                    {
                        if (hadValue) componentModel.Properties[name] = oldValue;
                        else componentModel.Properties.Remove(name);
                    }
                    else
                    {
                        if (hadValue) parentValue.Children[name] = oldValue;
                        else parentValue.Children.Remove(name);
                    }
                }
                if (!wasDirty) entityModel.IsDirty = false;
            };
            return undoModel;
        }

        public static EntityModel GetEntity(IEntity entity, StateModel model)
        {
            return model.Entities.GetOrAdd(entity.ID, id =>
            {
                var e = EntityModel.FromEntity(entity);
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

        private static ValueModel getParent(List<IProperty> propertyChain, ComponentModel componentModel)
        {
            var value = componentModel.Properties.GetOrAdd(propertyChain[0].Name, () => propertyChain[0].Value);
            for (int index = 1; index < propertyChain.Count; index++)
            {
                var property = propertyChain[index];
                value = value.Children.GetOrAdd(property.Name, () => property.Value);
            }
            return value;
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