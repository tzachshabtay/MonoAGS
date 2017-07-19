using AGS.API;

namespace AGS.Engine
{
    public class AGSDraggableComponent : AGSComponent, IDraggableComponent
    {
        private readonly IInput _input;
        private readonly IGameEvents _gameEvents;
        private float _dragObjectStartX, _dragObjectStartY, _dragMouseStartX, _dragMouseStartY;
        private ITranslate _translate;

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
            entity.Bind<ITranslateComponent>(c => _translate = c, _ => _translate = null);
            entity.Bind<IUIEvents>(c => c.MouseDown.Subscribe(onMouseDown), c => c.MouseDown.Unsubscribe(onMouseDown));
        }

        public override void Dispose()
        {
            base.Dispose();
            _gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
        }

        private void onMouseDown(MouseButtonEventArgs args)
        {
            if (!IsDragEnabled || args.Button != MouseButton.Left) return;
            IsCurrentlyDragged = true;
            _dragObjectStartX = _translate.X;
            _dragObjectStartY = _translate.Y;
            _dragMouseStartX = _input.MouseX;
            _dragMouseStartY = _input.MouseY;
        }

        private void onRepeatedlyExecute(object args)
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
            var translate = _translate;
            if (translate == null) return;

            translate.X = _dragObjectStartX + (mouseX - _dragMouseStartX);
            translate.Y = _dragObjectStartY + (mouseY - _dragMouseStartY);
        }        
    }
}
