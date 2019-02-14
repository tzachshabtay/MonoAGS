using System.ComponentModel;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class AGSSplitPanelComponent : AGSComponent, ISplitPanelComponent
    {
        private readonly IGameState _state;
        private readonly IGameFactory _factory;

        private IPanel _topPanel, _bottomPanel;
        private bool _isHorizontal;
        private Vector2 _startPositionDragLine, _startPositionBottomPanel;
		private SizeF _startSizeTopPanel, _startSizeBottomPanel;
        private IComponentBinding _splitLineMoveBinding;

        public AGSSplitPanelComponent(IGameState state, IGameFactory factory)
        {
            _state = state;
            _factory = factory;
        }

        public IPanel TopPanel 
        {
            get => _topPanel;
            set 
            { 
                if (_topPanel == value) return; 
                _topPanel = value;
                if (value != null)
                {
                    _startSizeTopPanel = new SizeF(value.Width, value.Height);
                }
                createNewSplitLine(); 
            } 
        }
        public IPanel BottomPanel 
        {
            get => _bottomPanel;
            set 
            { 
                if (_bottomPanel == value) return; 
                _bottomPanel = value;
                if (value != null)
                {
                    _startSizeBottomPanel = new SizeF(value.Width, value.Height);
                    _startPositionBottomPanel = new Vector2(value.X, value.Y);
                }
                createNewSplitLine(); 
            } 
        }
        public bool IsHorizontal { get => _isHorizontal; set { _isHorizontal = value; createNewSplitLine(); } }
        public IObject DragLine { get; private set; }

        private void disposeExistingSplitLine()
        {
            var existing = DragLine;
            if (existing != null)
            {
                existing.Visible = false;
                _state.UI.Remove(existing);
                _state.Room.Objects.Remove(existing);
                _splitLineMoveBinding?.Unbind();
                var translate = existing.GetComponent<ITranslateComponent>();
                if (translate != null)
                {
                    translate.PropertyChanged -= onSplitLineMoved;
                }
                existing.Dispose();
            }
        }

        private void createNewSplitLine()
        {
            disposeExistingSplitLine();
			var topPanel = TopPanel;
			if (topPanel == null) return;

            const float lineWidth = 5f;
            string suffix = IsHorizontal ? "Horiz" : "Vert";
            var splitLine = _factory.Object.GetObject($"{topPanel.ID}_SplitLine_{suffix}");
            _state.FocusedUI.CannotLoseFocus.Add(splitLine.ID);
            splitLine.RenderLayer = topPanel.RenderLayer;
			var crop = topPanel.GetComponent<ICropChildrenComponent>();
			crop?.EntitiesToSkipCrop.Add(splitLine.ID);
            HoverEffect.Add(splitLine, Colors.Transparent, Colors.Yellow.WithAlpha(100));
			splitLine.Pivot = new PointF(0f, 0f);
            positionSplitLine(splitLine, topPanel, lineWidth);
            splitLine.Z = -1f;
            topPanel.OnBoundingBoxesChanged.Subscribe(() => 
            {
                positionSplitLine(splitLine, topPanel, lineWidth);    
            });
            topPanel.Bind<IVisibleComponent>(c => c.PropertyChanged += onTopPanelVisibleChanged, c => c.PropertyChanged -= onTopPanelVisibleChanged);
            _startPositionDragLine = new Vector2(splitLine.X, splitLine.Y);
            splitLine.AddComponent<IUIEvents>();
            var draggable = splitLine.AddComponent<IDraggableComponent>();
            draggable.DragMinX = IsHorizontal ? (float?)null : splitLine.X;
            draggable.DragMaxX = IsHorizontal ? (float?)null : splitLine.X;
            draggable.DragMinY = IsHorizontal ? splitLine.Y : (float?)null;
            draggable.DragMaxY = IsHorizontal ? splitLine.Y : (float?)null;

            var room = topPanel.Room;
            if (room != null) room.Objects.Add(splitLine);
            else _state.UI.Add(splitLine);
            _splitLineMoveBinding = splitLine.Bind<ITranslateComponent>(
                c => c.PropertyChanged += onSplitLineMoved,
                c => c.PropertyChanged -= onSplitLineMoved);
            DragLine = splitLine;
        }

        private void onTopPanelVisibleChanged(object _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
            var line = DragLine;
            if (line == null) return;
            var panel = _topPanel;
            line.Visible = panel != null && panel.Visible;
        }

        private void positionSplitLine(IObject splitLine, IPanel topPanel, float lineWidth)
        {
			var boundingBoxes = topPanel.GetBoundingBoxes(_state.Viewport);
			var box = boundingBoxes.ViewportBox;
			float width = IsHorizontal ? lineWidth : box.Width;
			float height = IsHorizontal ? box.Height : lineWidth;
			splitLine.Image = new EmptyImage(width, height);
            float pivotX = -topPanel.Width * topPanel.Pivot.X;
			splitLine.X = IsHorizontal ? pivotX + topPanel.X + box.Width - lineWidth / 2f : pivotX;
			splitLine.Y = box.MinY;
        }

        private void onSplitLineMoved(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.Position)) return;
			var topPanel = TopPanel;
            if (topPanel == null) return;
			var bottomPanel = BottomPanel;
            var dragLine = DragLine;
            if (dragLine == null) return;

            if (IsHorizontal)
            {
                float delta = dragLine.X - _startPositionDragLine.X;
                topPanel.Image = new EmptyImage(_startSizeTopPanel.Width + delta, topPanel.Height);
                if (bottomPanel != null)
                {
                    bottomPanel.Image = new EmptyImage(_startSizeBottomPanel.Width - delta, bottomPanel.Height);
                    bottomPanel.X = _startPositionBottomPanel.X + delta;
                }
            }
            else
            {
                float delta = dragLine.Y - _startPositionDragLine.Y;
                topPanel.Image = new EmptyImage(topPanel.Width, _startSizeTopPanel.Height - delta);
                topPanel.Y = _startPositionBottomPanel.Y + delta;
                if (bottomPanel != null)
                {
                    bottomPanel.Image = new EmptyImage(bottomPanel.Width, _startSizeBottomPanel.Height + delta);
                    bottomPanel.Y = _startPositionBottomPanel.Y + delta;
                }
            }
        }
	}
}
