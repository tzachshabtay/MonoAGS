﻿using System;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public partial class EntityDesigner
    {
        private class RotateHandle
        {
            private ILabel _handle;
            private readonly IInput _input;
            private readonly Direction _direction;
            private readonly IGameState _state;
            private readonly ActionManager _actions;
            private readonly AGSEditor _editor;

            private readonly ITextConfig _idleConfig;
            private readonly ITextConfig _hoverConfig;

            private IRotateComponent _rotate;
            private IDrawableInfoComponent _drawable;
            private float _xOnDown, _yOnDown;
            private bool _isDown;
            private bool _isVisible;

            public RotateHandle(ILabel handle, AGSEditor editor, IGameState state, IInput input, ActionManager actions, Direction direction)
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

                state.UI.Add(_handle);
            }

            public void SetRotate(IRotateComponent rotate)
            {
                _rotate = rotate;
                SetVisible(_isVisible);
            }

            public void SetDrawable(IDrawableInfoComponent drawable) => _drawable = drawable;

            public void SetVisible(bool visible)
            {
                _isVisible = visible;
                var handle = _handle;
                if (handle == null) return;
                handle.Visible = visible && _rotate != null;
            }

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
                var rotate = _rotate;
                if (!_isDown || rotate == null) return;
                var handle = _handle;
                if (handle == null) return;

                if (!_input.LeftMouseButtonDown)
                {
                    _isDown = false;
                    handle.TextConfig = _idleConfig;
                    return;
                }

                float offsetX = _input.MousePosition.XMainViewport - _xOnDown;
                float offsetY = _input.MousePosition.YMainViewport - _yOnDown;

                if (MathUtils.FloatEquals(offsetX, 0f) || MathUtils.FloatEquals(offsetY, 0f)) return;

                (offsetX, offsetY) = _editor.ToGameSize(offsetX, offsetY);

                float angle = (float)MathHelper.RadiansToDegrees(-Math.Atan2(offsetX, offsetY)) + 90f;

                if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
                {
                    float clamp = AGSDraggableComponent.ClampingToWhenAlt;
                    angle = (float)Math.Round(angle / clamp) * clamp;
                }

                PropertyInfo prop = rotate.GetType().GetProperty(nameof(IRotateComponent.Angle));
                PropertyAction action = new PropertyAction(new InspectorProperty(rotate, null, nameof(IRotate.Angle), prop), angle, _editor.Project.Model);
                _actions.RecordAction(action);
            }

            private void setIcon()
            {
                var handle = _handle;
                if (handle == null) return;
                handle.Pivot = new PointF(0.5f, 0.5f);
                switch (_direction)
                {
                    case Direction.UpRight:
                        icon(FontIcons.RotateLeft);
                        break;
                    case Direction.UpLeft:
                        icon(FontIcons.RotateRight);
                        break;
                    case Direction.DownRight:
                        icon(FontIcons.RotateRight);
                        _handle.Angle = 180f;
                        break;
                    case Direction.DownLeft:
                        icon(FontIcons.RotateLeft);
                        _handle.Angle = 180f;
                        break;
                }
            }

            private void icon(string text)
            {
                var handle = _handle;
                if (handle == null) return;
                handle.Text = text;
            }

            private void move(float x, float y)
            {
                var handle = _handle;
                if (handle == null) return;
                (x, y) = _editor.ToEditorResolution(x, y, _drawable);
                handle.Position = new Position(x, y);
            }

            private void onMouseDown(MouseButtonEventArgs args)
            {
                _xOnDown = args.MousePosition.XMainViewport;
                _yOnDown = args.MousePosition.YMainViewport;
                _isDown = true;
            }

            private void onMouseLeave()
            {
                if (_isDown) return;
                var handle = _handle;
                if (handle == null) return;
                handle.TextConfig = _idleConfig;
            }

            private void onMouseEnter()
            {
                var handle = _handle;
                if (handle == null) return;
                handle.TextConfig = _hoverConfig;
            }
        }
    }
}
