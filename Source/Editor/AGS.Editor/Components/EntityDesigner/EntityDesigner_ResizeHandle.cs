using System;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public partial class EntityDesigner
    {
        private class ResizeHandle
        {
            private ILabel _handle;
            private readonly IInput _input;
            private readonly Direction _direction;
            private readonly IGameState _state;
            private readonly ActionManager _actions;
            private readonly AGSEditor _editor;

            private readonly ITextConfig _idleConfig;
            private readonly ITextConfig _hoverConfig;

            private IScaleComponent _scale;
            private IDrawableInfoComponent _drawable;
            private float _widthOnDown, _heightOnDown;
            private float _xOnDown, _yOnDown;
            private bool _isDown;
            private bool _isVisible;

            public ResizeHandle(ILabel handle, AGSEditor editor, IGameState state, IInput input, ActionManager actions, Direction direction)
            {
                _editor = editor;
                _actions = actions;
                _state = state;
                _idleConfig = handle.TextConfig;
                _hoverConfig = AGSTextConfig.ChangeColor(handle.TextConfig, Colors.Yellow, Colors.White, 0f);
                _direction = direction;
                _input = input;
                _handle = handle;
                setIcon();
                _handle.Visible = false;
                _handle.Enabled = true;
                _handle.RenderLayer = AGSLayers.UI;
                _handle.TextBackgroundVisible = false;
                _handle.MouseEnter.Subscribe(onMouseEnter);
                _handle.MouseDown.Subscribe(onMouseDown);
                _handle.MouseLeave.Subscribe(onMouseLeave);
                _isVisible = true;

                state.UI.Add(_handle);
            }

            public void SetScale(IScaleComponent scale)
            {
                _scale = scale;
                SetVisible(_isVisible);
            }

            public void SetVisible(bool visible)
            {
                _isVisible = visible;
                var handle = _handle;
                if (handle == null) return;
                handle.Visible = visible && _scale != null;
            }

            public void SetDrawable(IDrawableInfoComponent drawable) => _drawable = drawable;

            public void Dispose()
            {
                var handle = _handle;
                if (handle != null)
                {
                    handle.Dispose();
                    _state.UI.Remove(handle);
                    _handle = null;
                }
            }

            public void UpdatePosition(IBoundingBoxComponent box)
            {
                const float padding = 1f;
                float offsetHoriz = 2f + padding;
                float offsetVert = 2f + padding;

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

                var handle = _handle;
                if (handle == null) return;

                if (!_input.LeftMouseButtonDown)
                {
                    _isDown = false;
                    handle.TextConfig = _idleConfig;
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
                var handle = _handle;
                if (handle == null) return;
                handle.Angle = angle;
            }

            private void icon(string text)
            {
                var handle = _handle;
                if (handle == null) return;
                handle.Text = text;
            }

            private void pivot(float x, float y)
            {
                var handle = _handle;
                if (handle == null) return;
                handle.Pivot = new PointF(x, y);
            }

            private void move(float x, float y)
            {
                var handle = _handle;
                if (handle == null) return;
                (x, y) = _editor.ToEditorResolution(x, y, _drawable);
                handle.Position = new Position(x, y);
            }

            private void scale(float width, float height)
            {
                var handle = _handle;
                if (handle == null) return;
                (width, height) = _editor.ToGameSize(width, height);
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
                ScaleAction action = new ScaleAction(handle.GetFriendlyName(), _scale, w, h);
                _actions.RecordAction(action);
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

            private void onMouseLeave(MousePositionEventArgs args)
            {
                if (_isDown) return;
                var handle = _handle;
                if (handle == null) return;
                handle.TextConfig = _idleConfig;
            }

            private void onMouseEnter(MousePositionEventArgs args)
            {
                var handle = _handle;
                if (handle == null) return;
                handle.TextConfig = _hoverConfig;
            }
        }
    }
}
