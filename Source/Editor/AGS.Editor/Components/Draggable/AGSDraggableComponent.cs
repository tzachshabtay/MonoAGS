using System;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class AGSDraggableComponent : AGSComponent, IDraggableComponent
    {
        private readonly IInput _input;
        private readonly IGameEvents _gameEvents;
        private readonly ActionManager _actions;
        private IGame _game;
        private readonly AGSEditor _editor;
        private IEntity _entity;
        private float _dragObjectStartX, _dragObjectStartY, _dragMouseStartX, _dragMouseStartY;
        private ITranslate _translate;
        private IDrawableInfoComponent _drawable;

        public AGSDraggableComponent(AGSEditor editor, ActionManager actions)
        {
            _editor = editor;
            _actions = actions;
            _input = editor.Editor.Input;
            _gameEvents = editor.Editor.Events;
            IsDragEnabled = true;
        }

        public static float ClampingToWhenAlt = 5f;

        public float? DragMaxX { get; set; }

        public float? DragMaxY { get; set; }

        public float? DragMinX { get; set; }

        public float? DragMinY { get; set; }

        public bool IsDragEnabled { get; set; }

        public bool IsCurrentlyDragged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<ITranslateComponent>(c => _translate = c, _ => _translate = null);
            entity.Bind<IDrawableInfoComponent>(c => _drawable = c, _ => _drawable = null);
            entity.Bind<EditorUIEvents>(c => c.MouseDown.Subscribe(onMouseDown), c => c.MouseDown.Unsubscribe(onMouseDown));

            _game = _editor.Game.Find<IEntity>(entity.ID) == null ? _editor.Editor : _editor.Game;
            _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
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
            var mousePos = getMousePosition();
            _dragMouseStartX = mousePos.X;
            _dragMouseStartY = mousePos.Y;
        }

        private void onRepeatedlyExecute()
        {
            if (!IsCurrentlyDragged) return;
            if (!IsDragEnabled || !_input.LeftMouseButtonDown)
            {
                IsCurrentlyDragged = false;
                return;
            }

            var mousePos = getMousePosition();

			var translate = _translate;
			if (translate == null) return;

            var diffX = mousePos.X - _dragMouseStartX;
            var diffY = mousePos.Y - _dragMouseStartY;
            if (_game == _editor.Game)
            {
                (diffX, diffY) = _editor.ToGameResolution(diffX, diffY);
            }
            var translateX = _dragObjectStartX + diffX;
            var translateY = _dragObjectStartY + diffY;

            if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
            {
                translateX = (float)Math.Round(translateX / ClampingToWhenAlt) * ClampingToWhenAlt;
                translateY = (float)Math.Round(translateY / ClampingToWhenAlt) * ClampingToWhenAlt;
            }
            if (_input.IsKeyDown(Key.ControlLeft) || _input.IsKeyDown(Key.ControlRight))
            {
                if (Math.Abs(diffX) > Math.Abs(diffY))
                {
                    translateY = translate.Y;
                }
                else translateX = translate.X;
            }

            if (DragMinX != null && translateX < DragMinX.Value) translateX = translate.X;
            else if (DragMaxX != null && translateX > DragMaxX.Value) translateX = translate.X;
            if (DragMinY != null && translateY < DragMinY.Value) translateY = translate.Y;
            else if (DragMaxY != null && translateY > DragMaxY.Value) translateY = translate.Y;

            InspectorProperty property = new InspectorProperty(translate, "Location", translate.GetType().GetProperty(nameof(ITranslate.Location)));
            PropertyAction action = new PropertyAction(property, new AGSLocation(translateX, translateY));
            _actions.RecordAction(action);
        }

        private Vector2 getMousePosition()
        {
			float mouseX = _input.MousePosition.XMainViewport;
			float mouseY = _input.MousePosition.YMainViewport;

            var resolution = _drawable?.RenderLayer?.IndependentResolution;
			if (resolution != null)
			{
                mouseX *= ((float)resolution.Value.Width / _game.Settings.VirtualResolution.Width);
                mouseY *= ((float)resolution.Value.Height / _game.Settings.VirtualResolution.Height);
			}
			
            return new Vector2(mouseX, mouseY);
		}
    }
}