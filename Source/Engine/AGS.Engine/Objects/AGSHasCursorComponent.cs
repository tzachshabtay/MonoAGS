using AGS.API;

namespace AGS.Engine
{
    public class AGSHasCursorComponent : AGSComponent, IHasCursorComponent
    {
        private bool _showingObjectSpecificCursor;
        private IObject _lastCursor;
        private bool _cursorWasSaved;

        private readonly IGameEvents _events;
        private readonly IHitTest _hitTest;
        private readonly IGameState _state;

        private const string SPECIAL_CURSOR_TAG = "SpecialCursor";

        public AGSHasCursorComponent(IGameEvents events, IHitTest hitTest, IGameState state)
        {
            _events = events;
            _hitTest = hitTest;
            _state = state;
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public IObject SpecialCursor { get; set; }

		public override void Dispose()
		{
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            turnOffObjectSpecificCursor();
		}

		private void onRepeatedlyExecute()
        {
            IObject hotspot = _hitTest.ObjectAtMousePosition;
            if (hotspot == null)
            {
                turnOffObjectSpecificCursor();
                return;
            }
            IHasCursorComponent specialCursor = hotspot.GetComponent<IHasCursorComponent>();
            if (specialCursor == null)
            {
                turnOffObjectSpecificCursor();
                return;
            }
            var currentCursor = _state.Cursor;
            bool isCurrentSpecialCursor = currentCursor?.Properties.Bools.GetValue(SPECIAL_CURSOR_TAG, false) ?? false;
            if (!isCurrentSpecialCursor)
            {
                _lastCursor = currentCursor;
                _cursorWasSaved = true;
                specialCursor.SpecialCursor.Properties.Bools.SetValue(SPECIAL_CURSOR_TAG, true);
                _state.Cursor = specialCursor.SpecialCursor;
            }
            _showingObjectSpecificCursor = true;
        }

        private void turnOffObjectSpecificCursor()
        {
            if (!_showingObjectSpecificCursor) return;
            _showingObjectSpecificCursor = false;
            var lastCursor = _lastCursor;
            if (_cursorWasSaved)
            {
                _state.Cursor = _lastCursor;
            }
        }
    }
}