using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class GameCanvas
    {
        private readonly AGSEditor _editor;
        private readonly IObject _selectionMarker;
        private readonly GameToolbar _toolbar;
        private readonly GameDebugTree _tree;

        public GameCanvas(AGSEditor editor, GameToolbar toolbar, GameDebugTree tree)
        {
            _toolbar = toolbar;
            _tree = tree;
            _editor = editor;
            _selectionMarker = editor.Editor.Factory.Object.GetObject("SelectionMarker");
            _selectionMarker.Visible = false;
            _selectionMarker.Border = AGSBorders.SolidColor(GameViewColors.HoveredText, 2f);
            _selectionMarker.RenderLayer = AGSLayers.UI;

            editor.Editor.State.UI.Add(_selectionMarker);
        }

        public void Init()
        {
            _editor.Editor.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            _editor.Editor.Input.MouseUp.Subscribe(onMouseUp);
        }

        public static void ExpandAroundGameObject(AGSEditor editor, IBoundingBoxComponent boxComponent, IImageComponent image, IObject objToExpand)
        {
            if (boxComponent == null || image == null) return;
            var box = boxComponent.WorldBoundingBox;

            objToExpand.Pivot = image.Pivot;
            var x = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, image.Pivot.X);
            var y = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, image.Pivot.Y);

            (x, y) = editor.ToEditorResolution(x, y);

            (var minX, var minY) = editor.ToEditorResolution(box.MinX, box.MinY);
            (var maxX, var maxY) = editor.ToEditorResolution(box.MaxX, box.MaxY);

            objToExpand.Location = new AGSLocation(x, y);
            objToExpand.BaseSize = new SizeF(maxX - minX, maxY - minY);
        }

        private void onRepeatedlyExecute()
        {
            if (!_toolbar.IsPaused)
            {
                _selectionMarker.Visible = false;
                return;
            }
            var obj = _editor.Game.HitTest.ObjectAtMousePosition;
            if (obj == null || obj.HasComponent<EntityDesigner>())
            {
                _selectionMarker.Visible = false;
                return;
            }

            ExpandAroundGameObject(_editor, obj.GetComponent<IBoundingBoxComponent>(), obj.GetComponent<IImageComponent>(), _selectionMarker);
            _selectionMarker.Visible = true;
        }

        private void onMouseUp(MouseButtonEventArgs args)
        {
            if (args.Button != MouseButton.Left) return;
            if (!_selectionMarker.Visible) return;
            var obj = _editor.Game.HitTest.ObjectAtMousePosition;
            if (obj == null) return;
            _tree.Select(obj);
        }
    }
}