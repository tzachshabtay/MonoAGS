using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using AGS.API;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class PropertyAction : AbstractAction
    {
        private string _actionDisplayName;
        private readonly DateTime _timestamp;
        private readonly StateModel _model;
        private Action _undoModel;

        public PropertyAction(IProperty property, object value, StateModel model) : 
            this(property, new ValueModel(value, type: property.PropertyType), model) {}

        public PropertyAction(IProperty property, ValueModel value, StateModel model)
        {
            _model = model;
            _timestamp = DateTime.Now;
            Property = property;
            Value = value;
            _actionDisplayName = $"{property.Object?.ToString() ?? "Null"}.{property.Name} = {value?.ToString() ?? "Null"}";
        }

        public IProperty Property { get; set; }
        public ValueModel Value { get; set; }
        public ValueModel OldValue { get; set; }

        public override string ToString() => _actionDisplayName;

        protected override void ExecuteCore()
        {
            OldValue = Property.Value;
            execute();
        }

        protected override void UnExecuteCore()
        {
            Property.Value = OldValue;
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
            if (followingAction is PropertyAction next && next.Property == this.Property
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
            Property.Value = Value;
            if (Property.Component != null)
            {
                List<IProperty> propertyChain = Property.Parent == null ? null : getPropertyChain(Property);
                _undoModel = ModelAction.Execute(_model, (Property.Component, Property.Name, Value, propertyChain));
            }
            else if (Property.Name == nameof(IEntity.DisplayName) && Property.Object is IEntity entity)
            {
                var entityModel = ModelAction.GetEntity(entity, _model);
                Trace.Assert(entityModel != null);
                var oldDisplayName = entityModel.DisplayName;
                entityModel.DisplayName = Value?.Value as string;
                _undoModel = () => entity.DisplayName = oldDisplayName;
            }
            else
            {
                Debug.WriteLine($"No component associated with property {Property.DisplayName} of {Property.Object?.ToString() ?? "null"}, can't update model.");
            }
        }

        private List<IProperty> getPropertyChain(IProperty property)
        {
            List<IProperty> chain = new List<IProperty>();
            while (property.Parent != null)
            {
                chain.Insert(0, property.Parent);
                property = property.Parent;
            }
            return chain;
        }
	}
}