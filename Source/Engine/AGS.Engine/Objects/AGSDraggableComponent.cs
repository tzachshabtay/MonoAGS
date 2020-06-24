﻿using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSDraggableComponent : AGSComponent, IDraggableComponent
    {
        private readonly IInput _input;
        private readonly IGameEvents _gameEvents;
        private readonly IGameSettings _settings;
        private float _dragObjectStartX, _dragObjectStartY, _dragMouseStartX, _dragMouseStartY;
        private ITranslate _translate;
        private IDrawableInfoComponent _drawable;

        public AGSDraggableComponent(IInput input, IGameEvents gameEvents, IRuntimeSettings settings,
                                     IBlockingEvent<(float dragStartX, float dragStartY)> onDragStart)
        {
            _input = input;
            _settings = settings;
            _gameEvents = gameEvents;
            OnDragStart = onDragStart;
            IsDragEnabled = true;
        }

        public static float ClampingToWhenAlt = 5f;

        public float? DragMaxX { get; set; }

        public float? DragMaxY { get; set; }

        public float? DragMinX { get; set; }

        public float? DragMinY { get; set; }

        public bool IsDragEnabled { get; set; }

        public bool IsCurrentlyDragged { get; private set; }

        public IBlockingEvent<(float dragStartX, float dragStartY)> OnDragStart { get; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<ITranslateComponent>(c => _translate = c, _ => _translate = null);
            Entity.Bind<IUIEvents>(c => c.MouseDown.Subscribe(onMouseDown), c => c.MouseDown.Unsubscribe(onMouseDown));
            Entity.Bind<IDrawableInfoComponent>(c => _drawable = c, _ => _drawable = null);

            _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public void SimulateMouseDown(MouseButtonEventArgs args) => onMouseDown(args);

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
            (_dragMouseStartX, _dragMouseStartY) = getMousePosition();
            OnDragStart.Invoke((_dragObjectStartX, _dragObjectStartY));
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

            var diffX = mousePos.x - _dragMouseStartX;
            var diffY = mousePos.y - _dragMouseStartY;
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

            bool canDragX = true;
            bool canDragY = true;
            if (DragMinX != null && translateX < DragMinX.Value) canDragX = false;
            else if (DragMaxX != null && translateX > DragMaxX.Value) canDragX = false;
            if (DragMinY != null && translateY < DragMinY.Value) canDragY = false;
            else if (DragMaxY != null && translateY > DragMaxY.Value) canDragY = false;

            if (canDragX) translate.X = translateX;
            if (canDragY) translate.Y = translateY;
        }

        private (float x, float y) getMousePosition()
        {
            float mouseX = _input.MousePosition.XMainViewport;
            float mouseY = _input.MousePosition.YMainViewport;

            var resolution = _drawable?.RenderLayer?.IndependentResolution;
            if (resolution != null)
            {
                mouseX *= ((float)resolution.Value.Width / _settings.VirtualResolution.Width);
                mouseY *= ((float)resolution.Value.Height / _settings.VirtualResolution.Height);
            }

            return (mouseX, mouseY);
        }
    }
}