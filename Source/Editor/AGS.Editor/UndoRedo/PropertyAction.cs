using System;
using System.Reflection;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class PropertyAction : AbstractAction
    {
        private string _actionDisplayName;
        private DateTime _timestamp;

        public PropertyAction(InspectorProperty property, object value)
        {
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
            Property.SetValue(ParentObject, Value, null);
        }

        protected override void UnExecuteCore()
        {
            Property.SetValue(ParentObject, OldValue, null);
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
                Property.SetValue(ParentObject, Value, null);
                return true;
            }
            return false;
        }

        private bool isRecent(PropertyAction followingAction)
        {
            var timeDelta = followingAction._timestamp.Subtract(_timestamp);
            return timeDelta.Seconds < 2;
        }
	}
}