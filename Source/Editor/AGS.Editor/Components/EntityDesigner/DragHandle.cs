using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class DragHandle
    {
        private IObject _handle;
        private readonly IGameState _state;
        private readonly ActionManager _actions;
        private readonly AGSEditor _editor;
        private readonly IDraggableComponent _draggable;
        private ILabel _moveCursor;

        private IBoundingBoxComponent _boundingBox;
        private ITranslateComponent _translate;
        private IImageComponent _image;
        private IDrawableInfoComponent _drawable;

        private float _offsetX, _offsetY;

        public DragHandle(IObject handle, AGSEditor editor, IGameState state, ActionManager actions, bool needMoveCursor)
        {
            _editor = editor;
            _actions = actions;
            _state = state;
            _handle = handle;
            _handle.Visible = false;
            _handle.Enabled = true;
            _handle.Border = AGSBorders.SolidColor(Colors.WhiteSmoke, 2f);
            _handle.RenderLayer = AGSLayers.UI;
            _handle.IsPixelPerfect = false;
            _handle.AddComponent<IUIEvents>();
            _draggable = _handle.AddComponent<IDraggableComponent>();
            _draggable.OnDragStart.Subscribe(onDragStart);

            if (needMoveCursor)
            {
                var moveCursor = editor.Editor.Factory.UI.GetLabel("MoveCursor", "", 25f, 25f, 0f, 0f, config: FontIcons.IconConfig, addToUi: false);
                _moveCursor = moveCursor;
                moveCursor.Pivot = new PointF(0.5f, 0.5f);
                moveCursor.Text = FontIcons.Move;

                _handle.AddComponent<IHasCursorComponent>().SpecialCursor = moveCursor;
            }

            state.UI.Add(_handle);

            _handle.GetComponent<ITranslateComponent>().PropertyChanged += onHandleMoved;
        }

        public DateTime LastDragged { get; private set; }

        public bool WasDragged { get => LastDragged != DateTime.MinValue; }

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

        public void SetDrawable(IDrawableInfoComponent drawable) => _drawable = drawable;

        public void Dispose()
        {
            _moveCursor?.Dispose();
            _moveCursor = null;

            var handle = _handle;
            if (handle != null)
            {
                handle.Dispose();
                _state.UI.Remove(handle);
                _handle = null;
            }
        }

        public void UpdatePosition()
        {
            var handle = _handle;
            if (handle == null) return;
            GameCanvas.ExpandAroundGameObject(_editor, _boundingBox, _drawable, _image, handle, !_draggable.IsCurrentlyDragged);
        }

        private void setVisible()
        {
            var handle = _handle;
            if (handle == null) return;
            handle.Visible = _boundingBox != null && _translate != null && _image != null;
        }

        private void onDragStart((float dragStartX, float dragStartY) args)
        {
            var (dragStartX, dragStartY) = _editor.ToGameResolution(args.dragStartX, args.dragStartY, _drawable);
            (_offsetX, _offsetY) = (_translate.X - dragStartX, _translate.Y - dragStartY);
        }

        private void onHandleMoved(object sender, PropertyChangedEventArgs args)
        {
            if (!_draggable.IsCurrentlyDragged) return;
            var box = _boundingBox;
            if (box == null) return;
            var image = _image;
            if (image == null) return;
            var handle = _handle;
            if (handle == null) return;

            LastDragged = DateTime.Now;

            var handleBottomLeft = handle.WorldBoundingBox.BottomLeft;
            var (entityBottomLeftX, entityBottomLeftY) = _editor.ToGameResolution(handleBottomLeft.X, handleBottomLeft.Y, _drawable);

            var (translateX, translateY) = _editor.ToGameResolution(handle.X, handle.Y, _drawable);
            (translateX, translateY) = (translateX + _offsetX, translateY + _offsetY);

            InspectorProperty property = new InspectorProperty(_translate, "Position", _translate.GetType().GetProperty(nameof(ITranslate.Position)));
            PropertyAction action = new PropertyAction(property, new Position(translateX, translateY), _editor.Project.Model);
            _actions.RecordAction(action);
        }
    }
}
