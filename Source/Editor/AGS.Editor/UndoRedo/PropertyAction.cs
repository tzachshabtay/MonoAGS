using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class PropertyAction : AbstractAction
    {
        private string _actionDisplayName;
        private readonly DateTime _timestamp;
        private readonly StateModel _model;
        private Action _undoModel;

        public PropertyAction(InspectorProperty property, object value, StateModel model)
        {
            _model = model;
            _timestamp = DateTime.Now;
            ParentObject = property.Object;
            Property = property.Prop;
            Value = value;
            _actionDisplayName = $"{property.Object?.ToString() ?? "Null"}.{property.Name} = {value?.ToString() ?? "Null"}";
        }

        public object ParentObject { get; set; }
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }
        public object OldValue { get; set; }

        public override string ToString() => _actionDisplayName;

        protected override void ExecuteCore()
        {
            OldValue = Property.GetValue(ParentObject, null);
            execute();
        }

        protected override void UnExecuteCore()
        {
            Property.SetValue(ParentObject, OldValue, null);
            _undoModel?.Invoke();
        }

        /// <summary>
        /// Subsequent changes of the same property on the same object are consolidated into one action
        /// </summary>
        /// <param name="followingAction">Subsequent action that is being recorded</param>
        /// <returns>true if it agreed to merge with the next action, 
        /// false if the next action should be recorded separately</returns>
        public override bool TryToMerge(IAction followingAction)
        {
            if (followingAction is PropertyAction next && next.ParentObject == this.ParentObject && next.Property == this.Property
                && isRecent(next))
            {
                Value = next.Value;
                _actionDisplayName = next._actionDisplayName;
                execute();
                return true;
            }
            return false;
        }

        private bool isRecent(PropertyAction followingAction)
        {
            var timeDelta = followingAction._timestamp.Subtract(_timestamp);
            return timeDelta.Seconds < 2;
        }

        private void execute()
        {
            Property.SetValue(ParentObject, Value, null);

            if (ParentObject is API.IComponent component && component.Entity != null)
            {
                var entityModel = _model.Entities.GetOrAdd(component.Entity.ID, id =>
                {
                    var e = new EntityModel
                    {
                        ID = id,
                        DisplayName = component.Entity.DisplayName,
                        EntityConcreteType = component.Entity.GetType(),
                        Components = new Dictionary<Type, ComponentModel>()
                    };
                    var room = component.Entity.GetComponent<IHasRoomComponent>()?.Room;
                    if (room != null)
                    {
                        var roomModel = getRoom(room);
                        if (roomModel.BackgroundEntity != component.Entity.ID)
                            roomModel.Entities.Add(component.Entity.ID);
                    }
                    else
                    {
                        _model.Guis.Add(component.Entity.ID);
                    }
                    return e;
                });

                var componentModel = entityModel.Components.GetOrAdd(component.RegistrationType, _ => new ComponentModel
                    { ComponentConcreteType = component.GetType(), Properties = new Dictionary<string, object>() });

                bool wasDirty = entityModel.IsDirty;

                if (componentModel.Properties.TryGetValue(Property.Name, out object oldValue))
                {
                    _undoModel = () =>
                    {
                        componentModel.Properties[Property.Name] = oldValue;
                        if (!wasDirty) entityModel.IsDirty = false;
                    };
                }
                else
                {
                    _undoModel = () =>
                    {
                        componentModel.Properties.Remove(Property.Name);
                        if (!wasDirty) entityModel.IsDirty = false;
                    };
                }
                componentModel.Properties[Property.Name] = Value;
                entityModel.IsDirty = true;
            }
        }

        private RoomModel getRoom(IRoom room)
        {
            var roomModel = _model.Rooms.FirstOrDefault(r => r.ID == room.ID);
            if (roomModel == null)
            {
                roomModel = new RoomModel { ID = room.ID, BackgroundEntity = room.Background?.Entity?.ID, Entities = new HashSet<string>() };
                _model.Rooms.Add(roomModel);
            }
            return roomModel;
        }
	}
}