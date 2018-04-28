using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IRotateComponent))]
    [RequiredComponent(typeof(EditorUIEvents))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(ITranslateComponent))]
    public partial class EntityDesigner : AGSComponent
    {
        private readonly IGameFactory _factory;
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private readonly IInput _input;
        private readonly List<ResizeHandle> _resizeHandles;
        private readonly List<RotateHandle> _rotateHandles;
        private readonly AGSEditor _editor;
        private PivotHandle _pivotHandle;
        private DragHandle _dragHandle;
        private readonly ActionManager _actions;
        private IBoundingBoxComponent _box;
        private bool _resizeVisible;

        public EntityDesigner(AGSEditor editor, ActionManager actions)
        {
            _actions = actions;
            _editor = editor;
            _factory = editor.Editor.Factory;
            _state = editor.Editor.State;
            _events = editor.Editor.Events;
            _input = editor.Editor.Input;
            _resizeVisible = true;
            _resizeHandles = new List<ResizeHandle>(8);
            _rotateHandles = new List<RotateHandle>(4);
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            addResizeHandles(entity, Direction.Right, Direction.Left, Direction.Up, Direction.Down,
                             Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft);
            addRotateHandles(entity, Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft);
            addPivotHandle(entity);
            addDragHandle(entity);

            entity.Bind<IScaleComponent>(setScale, _ => setScale(null));
            entity.Bind<IRotateComponent>(setRotate, _ => setRotate(null));
            entity.Bind<IImageComponent>(setImage, _ => setImage(null));
            entity.Bind<ITranslateComponent>(setTranslate, _ => setTranslate(null));

            entity.Bind<IBoundingBoxComponent>(
                c => { _box = c; c.OnBoundingBoxesChanged.Subscribe(onBoundingBoxChanged); _pivotHandle.SetBox(c); _dragHandle.SetBox(c); updatePositions(); },
                c => { _box = null; c.OnBoundingBoxesChanged.Unsubscribe(onBoundingBoxChanged); _pivotHandle.SetBox(null); _dragHandle.SetBox(null); });

            entity.Bind<EditorUIEvents>(c => c.MouseClicked.Subscribe(onMouseClicked), c => c.MouseClicked.Unsubscribe(onMouseClicked));

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            foreach (var handle in _resizeHandles) handle.Dispose();
            foreach (var handle in _rotateHandles) handle.Dispose();
            _pivotHandle.Dispose();
            _dragHandle.Dispose();
        }

        private void onMouseClicked(MouseClickEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            const long millis = 300;
            if (args.ClickTimeInMilliseconds > millis && _dragHandle.LastDragged > DateTime.Now.AddMilliseconds(-millis))
            {
                //Checking against "false alarms" -> the user meant to do a quick drag of the object, not to change between scale/rotate handles
                return;
            }
            _resizeVisible = !_resizeVisible;
            foreach (var handle in _resizeHandles) handle.SetVisible(_resizeVisible);
            foreach (var handle in _rotateHandles) handle.SetVisible(!_resizeVisible);
        }

        private void setTranslate(ITranslateComponent translate)
        {
            _pivotHandle.SetTranslate(translate);
            _dragHandle.SetTranslate(translate);
        }

        private void setImage(IImageComponent image)
        {
            _pivotHandle.SetImage(image);
            _dragHandle.SetImage(image);
        }

        private void setScale(IScaleComponent scale)
        {
            foreach (var handle in _resizeHandles) handle.SetScale(scale);
        }

        private void setRotate(IRotateComponent rotate)
        {
            foreach (var handle in _rotateHandles) handle.SetRotate(rotate);
        }

        private void addResizeHandles(IEntity entity, params Direction[] directions)
        {
            var config = FontIcons.IconConfig;
            foreach (var direction in directions)
            {
                var label = _factory.UI.GetLabel($"{entity.ID}_ResizeHandle{direction}", "", 25f, 25f, 0f, 0f, config: config, addToUi: false);
                _resizeHandles.Add(new ResizeHandle(label, _editor, _state, _input, _actions, direction));
            }
        }

        private void addRotateHandles(IEntity entity, params Direction[] directions)
        {
            var config = FontIcons.IconConfig;
            foreach (var direction in directions)
            {
                var label = _factory.UI.GetLabel($"{entity.ID}_RotateHandle{direction}", "", 25f, 25f, 0f, 0f, config: config, addToUi: false);
                _rotateHandles.Add(new RotateHandle(label, _editor, _state, _input, _actions, direction));
            }
        }

        private void addPivotHandle(IEntity entity)
        {
            var config = FontIcons.IconConfig;
            var label = _factory.UI.GetLabel($"{entity.ID}_PivotHAndle", "", 25f, 25f, 0f, 0f, config: config, addToUi: false);
            _pivotHandle = new PivotHandle(label, _editor, _state, _input, _actions);
        }

        private void addDragHandle(IEntity entity)
        {
            var obj = _factory.Object.GetObject($"{entity.ID}_DragHandle");
            _dragHandle = new DragHandle(obj, _editor, _state, _input, _actions);
        }

        private void onRepeatedlyExecute(IRepeatedlyExecuteEventArgs obj)
        {
            foreach (var handle in _resizeHandles) handle.Visit();
            foreach (var handle in _rotateHandles) handle.Visit();
            _pivotHandle.Visit();
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
            foreach (var handle in _rotateHandles) handle.UpdatePosition(box);
            _pivotHandle.UpdatePosition();
            _dragHandle.UpdatePosition();
        }
    }
}