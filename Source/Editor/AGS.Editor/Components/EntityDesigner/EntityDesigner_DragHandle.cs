using System;
using System.ComponentModel;
using System.Diagnostics;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public partial class EntityDesigner
    {
        private class DragHandle
        {
            private readonly IObject _handle;
            private readonly IInput _input;
            private readonly IGameState _state;
            private readonly ActionManager _actions;
            private readonly AGSEditor _editor;
            private readonly IDraggableComponent _draggable;
            private readonly ILabel _moveCursor;

            private IBoundingBoxComponent _boundingBox;
            private ITranslateComponent _translate;
            private IImageComponent _image;

            private float _offsetX, _offsetY;

            public DragHandle(IObject handle, AGSEditor editor, IGameState state, IInput input, ActionManager actions)
            {
                _editor = editor;
                _actions = actions;
                _state = state;
                _input = input;
                _handle = handle;
                _handle.Visible = false;
                _handle.Enabled = true;
                _handle.Border = AGSBorders.SolidColor(Colors.WhiteSmoke, 2f);
                _handle.RenderLayer = AGSLayers.UI;
                _handle.IsPixelPerfect = false;
                _handle.AddComponent<IUIEvents>();
                _draggable = _handle.AddComponent<IDraggableComponent>();
                _draggable.OnDragStart.Subscribe(onDragStart);

                _moveCursor = editor.Editor.Factory.UI.GetLabel("MoveCursor", "", 25f, 25f, 0f, 0f, config: FontIcons.IconConfig, addToUi: false);
                _moveCursor.Pivot = new PointF(0.5f, 0.5f);
                _moveCursor.Text = FontIcons.Move;

                _handle.AddComponent<IHasCursorComponent>().SpecialCursor = _moveCursor;

                state.UI.Add(_handle);

                _handle.GetComponent<ITranslateComponent>().PropertyChanged += onHandleMoved;
            }

            public DateTime LastDragged { get; private set; }

            public void SetBox(IBoundingBoxComponent box)
            {
                _boundingBox = box;
                setVisible();
                UpdatePosition();
            }

            public void SetTranslate(ITranslateComponent translate)
            {
                _translate = translate;
                setVisible();
            }

            public void SetImage(IImageComponent image)
            {
                _image = image;
                setVisible();
            }

            public void Dispose()
            {
                _handle.Dispose();
                _moveCursor.Dispose();
                _state.UI.Remove(_handle);
            }

            public void UpdatePosition()
            {
                if (_draggable.IsCurrentlyDragged) return;
                GameCanvas.ExpandAroundGameObject(_editor, _boundingBox, _image, _handle);
            }

            private void setVisible()
            {
                _handle.Visible = _boundingBox != null && _translate != null && _image != null;
            }

            private void onDragStart((float dragStartX, float dragStartY) args)
            {
                var (dragStartX, dragStartY) = _editor.ToGameResolution(args.dragStartX, args.dragStartY);
                (_offsetX, _offsetY) = (_translate.X - dragStartX, _translate.Y - dragStartY);
            }

            private void onHandleMoved(object sender, PropertyChangedEventArgs args)
            {
                if (!_draggable.IsCurrentlyDragged) return;
                var box = _boundingBox;
                if (box == null) return;
                var image = _image;
                if (image == null) return;

                LastDragged = DateTime.Now;

                var handleBottomLeft = _handle.WorldBoundingBox.BottomLeft;
                var (entityBottomLeftX, entityBottomLeftY) = _editor.ToGameResolution(handleBottomLeft.X, handleBottomLeft.Y);

                var (translateX, translateY) = _editor.ToGameResolution(_handle.X, _handle.Y);
                (translateX, translateY) = (translateX + _offsetX, translateY + _offsetY);

                InspectorProperty property = new InspectorProperty(_translate, "Location", _translate.GetType().GetProperty(nameof(ITranslate.Location)));
                PropertyAction action = new PropertyAction(property, new AGSLocation(translateX, translateY));
                _actions.RecordAction(action);
            }
        }
    }
}