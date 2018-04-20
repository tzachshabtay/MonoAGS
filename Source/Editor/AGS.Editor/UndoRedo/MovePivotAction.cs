using System;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class MovePivotAction : AbstractAction
    {
        private string _actionDisplayName;
        private DateTime _timestamp;
        private readonly IImageComponent _image;
        private readonly ITranslateComponent _translate;
        private float _fromPivotX, _fromPivotY, _fromX, _fromY, _toPivotX, _toPivotY, _toX, _toY;

        public MovePivotAction(string entityName, IImageComponent image, ITranslateComponent translate, 
                               float toPivotX, float toPivotY, float toX, float toY)
        {
            _timestamp = DateTime.Now;
            _translate = translate;
            _image = image;
            _toPivotX = toPivotX;
            _toPivotY = toPivotY;
            _toX = toX;
            _toY = toY;
            _actionDisplayName = $"{entityName?.ToString() ?? "Null"}.Pivot = ({toPivotX},{toPivotY})";
        }

        public override string ToString() => _actionDisplayName;

        protected override void ExecuteCore()
        {
            _fromPivotX = _image.Pivot.X;
            _fromPivotY = _image.Pivot.Y;
            _fromX = _translate.X;
            _fromY = _translate.Y;
            _image.Pivot = new PointF(_toPivotX, _toPivotY);
            _translate.Location = new AGSLocation(_toX, _toY);
        }

        protected override void UnExecuteCore()
        {
            _image.Pivot = new PointF(_fromPivotX, _fromPivotY);
            _translate.Location = new AGSLocation(_fromX, _fromY);
        }

        /// <summary>
        /// Subsequent changes of the same property on the same object are consolidated into one action
        /// </summary>
        /// <param name="followingAction">Subsequent action that is being recorded</param>
        /// <returns>true if it agreed to merge with the next action, 
        /// false if the next action should be recorded separately</returns>
        public override bool TryToMerge(IAction followingAction)
        {
            if (followingAction is MovePivotAction next && next._translate == this._translate &&
                next._image == this._image && isRecent(next))
            {
                _toX = next._toX;
                _toY = next._toY;
                _toPivotX = next._toPivotX;
                _toPivotY = next._toPivotY;
                _actionDisplayName = next._actionDisplayName;
                _image.Pivot = new PointF(_toPivotX, _toPivotY);
                _translate.Location = new AGSLocation(_toX, _toY);
                return true;
            }
            return false;
        }

        private bool isRecent(MovePivotAction followingAction)
        {
            var timeDelta = followingAction._timestamp.Subtract(_timestamp);
            return timeDelta.Seconds < 2;
        }
    }
}