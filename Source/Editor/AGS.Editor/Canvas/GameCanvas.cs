using AGS.API;
using AGS.Engine;
using Autofac;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class GameCanvas
    {
        private readonly AGSEditor _editor;
        private readonly IObject _selectionMarker;
        private readonly GameToolbar _toolbar;
        private readonly GameDebugTree _tree;
        private readonly CanvasMenu _menu;
        private DragHandle _dragHandle;

        public GameCanvas(AGSEditor editor, GameToolbar toolbar, GameDebugTree tree)
        {
            _toolbar = toolbar;
            _tree = tree;
            _editor = editor;
            var canvasHitTest = new CanvasHitTest(editor);
            editor.CanvasHitTest = canvasHitTest;
            _menu = new CanvasMenu(editor, toolbar);
            _selectionMarker = editor.Editor.Factory.Object.GetObject("SelectionMarker");
            _selectionMarker.Visible = false;
            _selectionMarker.Border = editor.Editor.Factory.Graphics.Borders.SolidColor(GameViewColors.HoveredText, 2f);
            _selectionMarker.RenderLayer = AGSLayers.UI;

            editor.Editor.State.UI.Add(_selectionMarker);
        }

        public void Init()
        {
            _editor.Editor.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            _editor.Editor.Input.MouseDown.Subscribe(onMouseDown);
            _editor.Editor.Input.MouseUp.Subscribe(onMouseUp);
            _menu.Load();
        }

        public static void ExpandAroundGameObject(AGSEditor editor, IBoundingBoxComponent boxComponent, IDrawableInfoComponent drawable,
                                                  IImageComponent image, IObject objToExpand, bool updatePosition)
        {
            if (boxComponent == null || image == null) return;
            var box = boxComponent.WorldBoundingBox;

            objToExpand.Pivot = image.Pivot;
            var x = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, image.Pivot.X);
            var y = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, image.Pivot.Y);

            (x, y) = editor.ToEditorResolution(x, y, drawable);

            (var minX, var minY) = editor.ToEditorResolution(box.MinX, box.MinY, drawable);
            (var maxX, var maxY) = editor.ToEditorResolution(box.MaxX, box.MaxY, drawable);

            if (updatePosition)
            {
                objToExpand.Position = (x, y);
            }
            objToExpand.BaseSize = new SizeF(maxX - minX, maxY - minY);
        }

        private void onRepeatedlyExecute()
        {
            if (!_toolbar.IsPaused)
            {
                _selectionMarker.Visible = false;
                return;
            }
            _editor.CanvasHitTest.Refresh();
            var obj = _editor.CanvasHitTest.ObjectAtMousePosition;
            if (obj == null || obj.HasComponent<EntityDesigner>())
            {
                _selectionMarker.Visible = false;
                return;
            }

            ExpandAroundGameObject(_editor, obj.GetComponent<IBoundingBoxComponent>(), obj.GetComponent<IDrawableInfoComponent>(),
                                   obj.GetComponent<IImageComponent>(), _selectionMarker, true);
            _selectionMarker.Z = 10000f; //Ensuring the selection marker is behind the designer handles (resize/rotate/pivot handles)
            _selectionMarker.Visible = true;
        }

        private void onMouseDown(MouseButtonEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            if (!_selectionMarker.Visible) return;
            var obj = _editor.CanvasHitTest.ObjectAtMousePosition;
            if (obj == null) return;

            _dragHandle?.Dispose();
            var handle = _editor.Editor.Factory.Object.GetObject($"{obj.ID}_DraggingHandle");
            _dragHandle = new DragHandle(handle, _editor, _editor.Editor.State, 
                                         _editor.EditorResolver.Container.Resolve<ActionManager>(), false);
            _dragHandle.SetBox(obj.GetComponent<IBoundingBoxComponent>());
            _dragHandle.SetTranslate(obj.GetComponent<ITranslateComponent>());
            _dragHandle.SetImage(obj.GetComponent<IImageComponent>());
            handle.GetComponent<IDraggableComponent>().SimulateMouseDown(args);
        }

        private void onMouseUp(MouseButtonEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            if (!_selectionMarker.Visible) return;
            if (_editor.Editor.State.FocusedUI.FocusedWindow != null) return;
            var obj = _editor.CanvasHitTest.ObjectAtMousePosition;
            if (obj == null) return;
            var dragHandle = _dragHandle;
            _dragHandle?.Dispose();
            if (dragHandle != null && dragHandle.WasDragged) return;
            _tree.Select(obj);
        }
    }
}
