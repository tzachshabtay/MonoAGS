using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public partial class EntityDesigner
    {
        private class PivotHandle
        {
            private readonly ILabel _handle;
            private readonly IInput _input;
            private readonly IGameState _state;
            private readonly ActionManager _actions;
            private readonly AGSEditor _editor;

            private readonly ITextConfig _idleConfig;
            private readonly ITextConfig _hoverConfig;

            private IImageComponent _image;
            private IBoundingBoxComponent _boundingBox;
            private ITranslateComponent _translate;
            private PointF _pivotOnDown;
            private float _xOnDown, _yOnDown, _translateXOnDown, _translateYOnDown;
            private bool _isDown;

            public PivotHandle(ILabel handle, AGSEditor editor, IGameState state, IInput input, ActionManager actions)
            {
                _editor = editor;
                _actions = actions;
                _state = state;
                _idleConfig = handle.TextConfig;
                _hoverConfig = AGSTextConfig.ChangeColor(handle.TextConfig, Colors.Yellow, Colors.White, 0f);
                _input = input;
                _handle = handle;
                _handle.Text = FontIcons.Pivot;
                _handle.Pivot = new PointF(0.5f, 0.5f);
                _handle.Visible = false;
                _handle.Enabled = true;
                _handle.RenderLayer = AGSLayers.UI;
                _handle.TextBackgroundVisible = false;
                _handle.MouseEnter.Subscribe(onMouseEnter);
                _handle.MouseDown.Subscribe(onMouseDown);
                _handle.MouseLeave.Subscribe(onMouseLeave);

                state.UI.Add(_handle);
            }

            public void SetImage(IImageComponent image)
            {
                var prevImage = _image;
                if (prevImage != null)
                {
                    prevImage.PropertyChanged -= onImagePropertyChanged;
                }
                _image = image;
                if (image != null)
                {
                    image.PropertyChanged += onImagePropertyChanged;
                }
                setVisible();
            }

            public void SetBox(IBoundingBoxComponent box)
            {
                _boundingBox = box;
                setVisible();
            }

            public void SetTranslate(ITranslateComponent translate)
            {
                _translate = translate;
                setVisible();
            }

            public void Dispose()
            {
                _handle.Visible = false;
                _state.UI.Remove(_handle);
            }

            public void UpdatePosition()
            {
                var image = _image;
                var boxComponent = _boundingBox;
                if (image == null || boxComponent == null) return;
                var box = boxComponent.WorldBoundingBox;
                var pivot = image.Pivot;
                var x = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, pivot.X);
                var y = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, pivot.Y);

                (x, y) = _editor.ToEditorResolution(x, y);

                _handle.Location = new AGSLocation(x, y);
            }

            public void Visit()
            {
                if (!_isDown) return;

                if (!_input.LeftMouseButtonDown)
                {
                    _isDown = false;
                    _handle.TextConfig = _idleConfig;
                    return;
                }

                var boundingBox = _boundingBox;
                if (boundingBox == null) return;

                float xDiff = _input.MousePosition.XMainViewport - _xOnDown;
                float yDiff = _input.MousePosition.YMainViewport - _yOnDown;
                (xDiff, yDiff) = _editor.ToGameResolution(xDiff, yDiff);
                float pivotXOffset = xDiff / boundingBox.WorldBoundingBox.Width;
                float pivotYOffset = yDiff / boundingBox.WorldBoundingBox.Height;
                float toPivotX = _pivotOnDown.X + pivotXOffset;
                float toPivotY = _pivotOnDown.Y + pivotYOffset;
                bool shouldRecalculateDiffs = false;
                if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
                {
                    shouldRecalculateDiffs = true;
                    toPivotX = (float)Math.Round(toPivotX, 1);
                    toPivotY = (float)Math.Round(toPivotY, 1);
                }
                if (_input.IsKeyDown(Key.ControlLeft) || _input.IsKeyDown(Key.ControlRight))
                {
                    shouldRecalculateDiffs = true;
                    toPivotX = MathUtils.Clamp(toPivotX, 0f, 1f);
                    toPivotY = MathUtils.Clamp(toPivotY, 0f, 1f);
                }
                if (shouldRecalculateDiffs)
                {
                    xDiff = boundingBox.WorldBoundingBox.Width * (toPivotX - _pivotOnDown.X);
                    yDiff = boundingBox.WorldBoundingBox.Height * (toPivotY - _pivotOnDown.Y);
                }
                float toX = _translateXOnDown + xDiff;
                float toY = _translateYOnDown + yDiff;
                MovePivotAction action = new MovePivotAction(_handle.GetFriendlyName(), _image, _translate,
                                                             toPivotX, toPivotY, toX, toY);
                _actions.RecordAction(action);
            }

            private void setVisible()
            {
                _handle.Visible = _boundingBox != null && _image != null && _translate != null;
            }

            private void onMouseDown(MouseButtonEventArgs args)
            {
                var imageComponent = _image;
                var translateComponent = _translate;
                if (imageComponent == null || translateComponent == null) return;
                _pivotOnDown = imageComponent.Pivot;
                _xOnDown = args.MousePosition.XMainViewport;
                _yOnDown = args.MousePosition.YMainViewport;
                _translateXOnDown = translateComponent.X;
                _translateYOnDown = translateComponent.Y;
                _isDown = true;
            }

            private void onMouseLeave(MousePositionEventArgs args)
            {
                if (_isDown) return;
                _handle.TextConfig = _idleConfig;
            }

            private void onMouseEnter(MousePositionEventArgs args)
            {
                _handle.TextConfig = _hoverConfig;
            }

            private void onImagePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IImageComponent.Pivot)) return;
                UpdatePosition();
            }
        }
    }
}