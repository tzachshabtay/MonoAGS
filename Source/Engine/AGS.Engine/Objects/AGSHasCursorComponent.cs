using AGS.API;

namespace AGS.Engine
{
    public class AGSHasCursorComponent : AGSComponent, IHasCursorComponent
    {
        private bool _showingObjectSpecificCursor;
        private IObject _lastCursor;
        private bool _cursotWasSaved;

        private readonly IGameEvents _events;
        private readonly IHitTest _hitTest;
        private readonly IInput _input;

        public AGSHasCursorComponent(IGameEvents events, IHitTest hitTest, IInput input)
        {
            _events = events;
            _hitTest = hitTest;
            _input = input;
            events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public IObject SpecialCursor { get; set; }

		public override void Dispose()
		{
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
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
            if (_input.Cursor != specialCursor.SpecialCursor)
            {
                _lastCursor = _input.Cursor;
                _cursotWasSaved = true;
                _input.Cursor = specialCursor.SpecialCursor;
            }
            _showingObjectSpecificCursor = true;
        }

        private void turnOffObjectSpecificCursor()
        {
            if (!_showingObjectSpecificCursor) return;
            _showingObjectSpecificCursor = false;
            var lastCursor = _lastCursor;
            if (_cursotWasSaved)
            {
                _input.Cursor = _lastCursor;
            }
        }
    }
}