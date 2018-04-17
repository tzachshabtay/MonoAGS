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
            var config = FontIcons.IconConfig;
            var idle = new ButtonAnimation(null, config, null);
            var hover = new ButtonAnimation(null, AGSTextConfig.ChangeColor(config, Colors.Yellow, Colors.White, 0f), null);
            var pushed = hover;
            foreach (var direction in directions)
            {
                var label = _factory.UI.GetButton($"{entity.ID}_ResizeHandle{direction}", idle, hover, pushed, 0f, 0f, width: 5f, height: 5f, addToUi: false);
                _resizeHandles.Add(new ResizeHandle(label, _state, _input, direction));
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
            private readonly IButton _handle;
            private readonly IInput _input;
            private readonly Direction _direction;

            private IScaleComponent _scale;
            private float _widthOnDown, _heightOnDown;
            private float _xOnDown, _yOnDown;
            private bool _isDown;

            public ResizeHandle(IButton handle, IGameState state, IInput input, Direction direction)
            {
                _direction = direction;
                _input = input;
                _handle = handle;
                setIcon();
                _handle.Visible = false;
                _handle.Enabled = true;
                _handle.RenderLayer = AGSLayers.UI;
                _handle.TextBackgroundVisible = false;
                _handle.MouseDown.Subscribe(onMouseDown);

                state.UI.Add(_handle);
            }

            public void SetScale(IScaleComponent scale)
            {
                _handle.Visible = (scale != null);
                _scale = scale;
            }

            public void UpdatePosition(IBoundingBoxComponent box)
            {
                const float padding = 1f;
                float offsetHoriz = FontIcons.IconConfig.Font.SizeInPoints / 2f + padding;
                float offsetVert = FontIcons.IconConfig.Font.SizeInPoints / 2f + padding;

                switch (_direction)
                {
                    case Direction.Right:
                        move(box.WorldBoundingBox.MaxX + padding, (box.WorldBoundingBox.MinY + box.WorldBoundingBox.MaxY) / 2f);
                        break;
                    case Direction.Left:
                        move(box.WorldBoundingBox.MinX - padding, (box.WorldBoundingBox.MinY + box.WorldBoundingBox.MaxY) / 2f);
                        break;
                    case Direction.Up:
                        move((box.WorldBoundingBox.MinX + box.WorldBoundingBox.MaxX) / 2f, box.WorldBoundingBox.MaxY + padding);
                        break;
                    case Direction.Down:
                        move((box.WorldBoundingBox.MinX + box.WorldBoundingBox.MaxX) / 2f, box.WorldBoundingBox.MinY - padding);
                        break;
                    case Direction.UpRight:
                        move(box.WorldBoundingBox.MaxX + offsetHoriz, box.WorldBoundingBox.MaxY + offsetVert);
                        break;
                    case Direction.UpLeft:
                        move(box.WorldBoundingBox.MinX - offsetHoriz, box.WorldBoundingBox.MaxY + offsetVert);
                        break;
                    case Direction.DownRight:
                        move(box.WorldBoundingBox.MaxX + offsetHoriz, box.WorldBoundingBox.MinY - offsetVert);
                        break;
                    case Direction.DownLeft:
                        move(box.WorldBoundingBox.MinX - offsetHoriz, box.WorldBoundingBox.MinY - offsetVert);
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

            private void setIcon()
            {
                switch (_direction)
                {
                    case Direction.Right:
                        icon(FontIcons.ResizeHorizontal);
                        pivot(0f, 0.5f);
                        break;
                    case Direction.Left:
                        icon(FontIcons.ResizeHorizontal);
                        pivot(1f, 0.5f);
                        break;
                    case Direction.Up:
                        icon(FontIcons.ResizeVertical);
                        pivot(0.5f, 0f);
                        break;
                    case Direction.Down:
                        icon(FontIcons.ResizeVertical);
                        pivot(0.5f, 1f);
                        break;
                    case Direction.UpRight:
                        icon(FontIcons.ResizeHorizontal);
                        rotate(45f);
                        pivot(0.5f, 0.5f);
                        break;
                    case Direction.UpLeft:
                        icon(FontIcons.ResizeHorizontal);
                        rotate(-45f);
                        pivot(0.5f, 0.5f);
                        break;
                    case Direction.DownRight:
                        icon(FontIcons.ResizeHorizontal);
                        rotate(-45f);
                        pivot(0.5f, 0.5f);
                        break;
                    case Direction.DownLeft:
                        icon(FontIcons.ResizeHorizontal);
                        rotate(45f);
                        pivot(0.5f, 0.5f);
                        break;
                }
            }

            private void rotate(float angle)
            {
                _handle.Angle = angle;
            }

            private void icon(string text)
            {
                _handle.Text = text;
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