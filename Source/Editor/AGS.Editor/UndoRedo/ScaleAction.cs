using System;
using AGS.API;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class ScaleAction : AbstractAction
    {
        private string _actionDisplayName;
        private DateTime _timestamp;
        private readonly IScaleComponent _scale;
        private float _fromWidth, _fromHeight, _toWidth, _toHeight;

        public ScaleAction(string entityName, IScaleComponent scale, float toWidth, float toHeight)
        {
            _timestamp = DateTime.Now;
            _scale = scale;
            _toWidth = toWidth;
            _toHeight = toHeight;
            _actionDisplayName = $"{entityName?.ToString() ?? "Null"}.ScaleTo({toWidth},{toHeight})";
        }

        public override string ToString() => _actionDisplayName;

        protected override void ExecuteCore()
        {
            _fromWidth = _scale.Width;
            _fromHeight = _scale.Height;
            _scale.ScaleTo(_toWidth, _toHeight);
        }

        protected override void UnExecuteCore()
        {
            _scale.ScaleTo(_fromWidth, _fromHeight);
        }

        /// <summary>
        /// Subsequent changes of the same property on the same object are consolidated into one action
        /// </summary>
        /// <param name="followingAction">Subsequent action that is being recorded</param>
        /// <returns>true if it agreed to merge with the next action, 
        /// false if the next action should be recorded separately</returns>
        public override bool TryToMerge(IAction followingAction)
        {
            if (followingAction is ScaleAction next && next._scale == this._scale && isRecent(next))
            {
                _toWidth = next._toWidth;
                _toHeight = next._toHeight;
                _actionDisplayName = next._actionDisplayName;
                _scale.ScaleTo(_toWidth, _toHeight);
                return true;
            }
            return false;
        }

        private bool isRecent(ScaleAction followingAction)
        {
            var timeDelta = followingAction._timestamp.Subtract(_timestamp);
            return timeDelta.Seconds < 2;
        }
    }
}