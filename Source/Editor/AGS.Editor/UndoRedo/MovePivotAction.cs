using System;
using AGS.API;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class MovePivotAction : AbstractAction
    {
        private string _actionDisplayName;
        private DateTime _timestamp;
        private readonly IImageComponent _image;
        private readonly ITranslateComponent _translate;
        private readonly StateModel _model;
        private float _fromPivotX, _fromPivotY, _fromX, _fromY, _toPivotX, _toPivotY, _toX, _toY;
        private Action _undoModel;

        public MovePivotAction(string entityName, IImageComponent image, ITranslateComponent translate, 
                               float toPivotX, float toPivotY, float toX, float toY, StateModel model)
        {
            _model = model;
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
            execute();
        }

        protected override void UnExecuteCore()
        {
            _image.Pivot = new PointF(_fromPivotX, _fromPivotY);
            _translate.Position = new Position(_fromX, _fromY);
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
            if (followingAction is MovePivotAction next && next._translate == this._translate &&
                next._image == this._image && isRecent(next))
            {
                _toX = next._toX;
                _toY = next._toY;
                _toPivotX = next._toPivotX;
                _toPivotY = next._toPivotY;
                _actionDisplayName = next._actionDisplayName;
                execute();
                return true;
            }
            return false;
        }

        private bool isRecent(MovePivotAction followingAction)
        {
            var timeDelta = followingAction._timestamp.Subtract(_timestamp);
            return timeDelta.Seconds < 2;
        }

        private void execute()
        {
            var pivot = new PointF(_toPivotX, _toPivotY);
            var position = new Position(_toX, _toY);
            _image.Pivot = pivot;
            _translate.Position = position;
            ModelAction.Execute(_model, 
                (_image, nameof(IImageComponent.Pivot), pivot), 
                (_translate, nameof(ITranslateComponent.Position), position));
        }
    }
}