using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IScaleComponent))]
    public class EntityDesigner : AGSComponent
    {
        private readonly IGameFactory _factory;
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private readonly IInput _input;
        private readonly List<ResizeHandle> _resizeHandles;
        private IBoundingBoxComponent _box;

        public EntityDesigner(IGameFactory factory, IGameState state, IGameEvents events, IInput input)
        {
            _factory = factory;
            _state = state;
            _events = events;
            _input = input;
            _resizeHandles = new List<ResizeHandle>(8);
        }

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            addResizeHandles(entity, Direction.Right, Direction.Left, Direction.Up, Direction.Down,
                             Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft);

            entity.Bind<IScaleComponent>(c => setScale(c), _ => setScale(null));

            entity.Bind<IBoundingBoxComponent>(
                c => { _box = c; c.OnBoundingBoxesChanged.Subscribe(onBoundingBoxChanged); updatePositions(); },
                c => { _box = null; c.OnBoundingBoxesChanged.Unsubscribe(onBoundingBoxChanged); });

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		public override void Dispose()
		{
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
		}

        private void setScale(IScaleComponent scale)
        {
            foreach (var handle in _resizeHandles) handle.SetScale(scale);
        }

        private void addResizeHandles(IEntity entity, params Direction[] directions)
        {
            foreach (var direction in directions)
            {
                var obj = _factory.Object.GetObject($"{entity.ID}_ResizeHandle{direction}");
                _resizeHandles.Add(new ResizeHandle(obj, _state, _input, direction));
            }
        }

		private void onRepeatedlyExecute(IRepeatedlyExecuteEventArgs obj)
        {
            foreach (var handle in _resizeHandles) handle.Visit();
        }

        private void onBoundingBoxChanged()
        {
            updatePositions();
        }

        private void updatePositions()
        {
            var box = _box;
            if (box == null) return;
            foreach (var handle in _resizeHandles) handle.UpdatePosition(box);
        }

        private class ResizeHandle
        {
            private readonly IObject _handle;
            private readonly IInput _input;
            private readonly Direction _direction;

            private IScaleComponent _scale;
            private float _widthOnDown, _heightOnDown;
            private float _xOnDown, _yOnDown;
            private bool _isDown;

            public ResizeHandle(IObject handle, IGameState state, IInput input, Direction direction)
            {
                _direction = direction;
                _input = input;
                _handle = handle;
                _handle.Image = new EmptyImage(5f, 5f);
                setPivot();
                _handle.Visible = false;
                _handle.RenderLayer = AGSLayers.UI;
                var uiEvents = _handle.AddComponent<IUIEvents>();
                uiEvents.MouseDown.Subscribe(onMouseDown);
                HoverEffect.Add(_handle, Colors.Blue, Colors.Yellow);

                state.UI.Add(_handle);
            }

            public void SetScale(IScaleComponent scale)
            {
                _handle.Visible = (scale != null);
                _scale = scale;
            }

            public void UpdatePosition(IBoundingBoxComponent box)
            {
                switch (_direction)
                {
                    case Direction.Right:
                        move(box.WorldBoundingBox.MaxX + 1f, (box.WorldBoundingBox.MinY + box.WorldBoundingBox.MaxY) / 2f);
                        break;
                    case Direction.Left:
                        move(box.WorldBoundingBox.MinX - 1f, (box.WorldBoundingBox.MinY + box.WorldBoundingBox.MaxY) / 2f);
                        break;
                    case Direction.Up:
                        move((box.WorldBoundingBox.MinX + box.WorldBoundingBox.MaxX) / 2f, box.WorldBoundingBox.MaxY + 1f);
                        break;
                    case Direction.Down:
                        move((box.WorldBoundingBox.MinX + box.WorldBoundingBox.MaxX) / 2f, box.WorldBoundingBox.MinY - 1f);
                        break;
                    case Direction.UpRight:
                        move(box.WorldBoundingBox.MaxX + 1f, box.WorldBoundingBox.MaxY + 1f);
                        break;
                    case Direction.UpLeft:
                        move(box.WorldBoundingBox.MinX - 1f, box.WorldBoundingBox.MaxY + 1f);
                        break;
                    case Direction.DownRight:
                        move(box.WorldBoundingBox.MaxX + 1f, box.WorldBoundingBox.MinY - 1f);
                        break;
                    case Direction.DownLeft:
                        move(box.WorldBoundingBox.MinX - 1f, box.WorldBoundingBox.MinY - 1f);
                        break;
                }
            }

            public void Visit()
            {
                if (!_isDown) return;

                if (!_input.LeftMouseButtonDown)
                {
                    _isDown = false;
                    return;
                }
                switch (_direction)
                {
                    case Direction.Right:
                        scale(_input.MousePosition.XMainViewport - _xOnDown, 0f);
                        break;
                    case Direction.Left:
                        scale(_xOnDown - _input.MousePosition.XMainViewport, 0f);
                        break;
                    case Direction.Up:
                        scale(0f, _input.MousePosition.YMainViewport - _yOnDown);
                        break;
                    case Direction.Down:
                        scale(0f, _yOnDown - _input.MousePosition.YMainViewport);
                        break;
                    case Direction.UpRight:
                        scale(_input.MousePosition.XMainViewport - _xOnDown, _input.MousePosition.YMainViewport - _yOnDown);
                        break;
                    case Direction.UpLeft:
                        scale(_xOnDown - _input.MousePosition.XMainViewport, _input.MousePosition.YMainViewport - _yOnDown);
                        break;
                    case Direction.DownRight:
                        scale(_input.MousePosition.XMainViewport - _xOnDown, _yOnDown - _input.MousePosition.YMainViewport);
                        break;
                    case Direction.DownLeft:
                        scale(_xOnDown - _input.MousePosition.XMainViewport, _yOnDown - _input.MousePosition.YMainViewport);
                        break;
                }
            }

            private void setPivot()
            {
                switch (_direction)
                {
                    case Direction.Right:
                        pivot(0f, 0.5f);
                        break;
                    case Direction.Left:
                        pivot(1f, 0.5f);
                        break;
                    case Direction.Up:
                        pivot(0.5f, 0f);
                        break;
                    case Direction.Down:
                        pivot(0.5f, 1f);
                        break;
                    case Direction.UpRight:
                        pivot(0f, 0f);
                        break;
                    case Direction.UpLeft:
                        pivot(1f, 0f);
                        break;
                    case Direction.DownRight:
                        pivot(0f, 1f);
                        break;
                    case Direction.DownLeft:
                        pivot(1f, 1f);
                        break;
                }
            }

            private void pivot(float x, float y)
            {
                _handle.Pivot = new PointF(x, y);
            }

            private void move(float x, float y)
            {
                _handle.Location = new AGSLocation(x, y);
            }

            private void scale(float width, float height)
            {
                float w = _widthOnDown + width;
                float h = _heightOnDown + height;
                if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
                {
                    float clamp = AGSDraggableComponent.ClampingToWhenAlt;
                    w = (float)Math.Round(w / clamp) * clamp;
                    h = (float)Math.Round(h / clamp) * clamp;
                }
                if (_input.IsKeyDown(Key.ControlLeft) || _input.IsKeyDown(Key.ControlRight))
                {
                    float targetAspectRatio = _widthOnDown / _heightOnDown;
                    float heightCandidate = w / targetAspectRatio;
                    float widthCandidate = h * targetAspectRatio;
                    if (Math.Abs(heightCandidate - _heightOnDown) < Math.Abs(widthCandidate - _widthOnDown))
                    {
                        w = widthCandidate;
                    }
                    else h = heightCandidate;
                }
                _scale.ScaleTo(w, h);
            }

            private void onMouseDown(MouseButtonEventArgs args)
            {
                var scaleComponent = _scale;
                if (scaleComponent == null) return;
                _widthOnDown = scaleComponent.Width;
                _heightOnDown = scaleComponent.Height;
                _xOnDown = args.MousePosition.XMainViewport;
                _yOnDown = args.MousePosition.YMainViewport;
                _isDown = true;
            }
        }
	}
}