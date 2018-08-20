using System;
using System.Reflection;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class PropertyAction : AbstractAction
    {
        private string _actionDisplayName;
        private readonly DateTime _timestamp;
        private readonly StateModel _model;
        private Action _undoModel;

        public PropertyAction(IProperty property, object value, StateModel model)
        {
            _model = model;
            _timestamp = DateTime.Now;
            Property = property;
            Value = value;
            _actionDisplayName = $"{property.Object?.ToString() ?? "Null"}.{property.Name} = {value?.ToString() ?? "Null"}";
        }

        public IProperty Property { get; set; }
        public object Value { get; set; }
        public object OldValue { get; set; }

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
            if (Property.Object is API.IComponent component)
            {
                _undoModel = ModelAction.Execute(_model, (component, Property.Name, Value));
            }
        }
	}
}