using AGS.API;

namespace AGS.Engine
{
    public class AGSDraggableComponent : AGSComponent, IDraggableComponent
    {
        private readonly IInput _input;
        private readonly IGameEvents _gameEvents;
        private IUIEvents _uiEvents;
        private float _dragObjectStartX, _dragObjectStartY, _dragMouseStartX, _dragMouseStartY;
        private ITranslate _transform;

        public AGSDraggableComponent(IInput input, IGameEvents gameEvents)
        {
            _input = input;
            _gameEvents = gameEvents;
            _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            IsDragEnabled = true;
        }

        public float? DragMaxX { get; set; }

        public float? DragMaxY { get; set; }

        public float? DragMinX { get; set; }

        public float? DragMinY { get; set; }

        public bool IsDragEnabled { get; set; }

        public bool IsCurrentlyDragged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _transform = entity.GetComponent<ITranslateComponent>();
            _uiEvents = entity.GetComponent<IUIEvents>();
            _uiEvents.MouseDown.Subscribe(onMouseDown);
        }

        public override void Dispose()
        {
            base.Dispose();
            _uiEvents.MouseDown.Unsubscribe(onMouseDown);
            _gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
        }

        private void onMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (!IsDragEnabled || args.Button != MouseButton.Left) return;
            IsCurrentlyDragged = true;
            _dragObjectStartX = _transform.X;
            _dragObjectStartY = _transform.Y;
            _dragMouseStartX = _input.MouseX;
            _dragMouseStartY = _input.MouseY;
        }

        private void onRepeatedlyExecute(object sender, AGSEventArgs args)
        {
            if (!IsCurrentlyDragged) return;
            if (!IsDragEnabled || !_input.LeftMouseButtonDown)
            {
                IsCurrentlyDragged = false;
                return;
            }            

            float mouseX = _input.MouseX;
            float mouseY = _input.MouseY;

            if (DragMinX != null && mouseX < DragMinX.Value) return;
            if (DragMaxX != null && mouseX > DragMaxX.Value) return;
            if (DragMinY != null && mouseY < DragMinY.Value) return;
            if (DragMaxY != null && mouseY > DragMaxY.Value) return;

            _transform.X = _dragObjectStartX + (mouseX - _dragMouseStartX);
            _transform.Y = _dragObjectStartY + (mouseY - _dragMouseStartY);
        }        
    }
}
