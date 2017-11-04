using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        private IBoundingBoxWithChildrenComponent _boundingBoxWithChildren;
        private IInObjectTree _tree;
        private float _absoluteSpacing, _relativeSpacing, _startLocation;
        private LayoutDirection _direction;
        private bool _isPaused;
        private IEntity _entity;

        public AGSStackLayoutComponent()
        {
            _isPaused = true;
            OnLayoutChanged = new AGSEvent();
            _direction = LayoutDirection.Vertical;
            _relativeSpacing = -1f; //a simple vertical layout top to bottom by default.
        }

        public LayoutDirection Direction { get { return _direction; } set { _direction = value; adjustLayout(); } }
        public float AbsoluteSpacing { get { return _absoluteSpacing; } set { _absoluteSpacing = value; adjustLayout(); } }
        public float RelativeSpacing { get { return _relativeSpacing; } set { _relativeSpacing = value; adjustLayout(); } }
        public float StartLocation { get { return _startLocation; } set { _startLocation = value; adjustLayout(); } }
        public IEvent OnLayoutChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);
            entity.Bind<IBoundingBoxWithChildrenComponent>(c => { _boundingBoxWithChildren = c; c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged); adjustLayout(); }, 
                                                    c => { c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged); _boundingBoxWithChildren = null; });
            entity.Bind<IInObjectTree>(c => { _tree = c; adjustLayout(); }, _ => _tree = null);
        }

        public void StartLayout()
        {
            _isPaused = false;
            adjustLayout();
        }

        public void StopLayout()
        {
            _isPaused = true;
        }

        private void onSizeChanged()
        {
            adjustLayout();
        }

        private void adjustLayout()
        {
            if (_isPaused) return;
            float location = StartLocation;

            var tree = _tree;
            if (tree == null) return;

            foreach (var child in tree.TreeNode.Children)
            {
                if (!child.UnderlyingVisible) continue;
                float step;
                if (Direction == LayoutDirection.Vertical)
                {
                    child.Y = location;
                    step = child.AddComponent<IBoundingBoxWithChildrenComponent>().BoundingBoxWithChildren.Height;
                }
                else
                {
                    child.X = location;
                    step = child.AddComponent<IBoundingBoxWithChildrenComponent>().BoundingBoxWithChildren.Width;
                }
                location += step * RelativeSpacing + AbsoluteSpacing;
            }
            OnLayoutChanged.Invoke();
        }


	}
}
