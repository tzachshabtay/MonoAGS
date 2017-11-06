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
            EntitiesToIgnore = new AGSConcurrentHashSet<string>();
            _direction = LayoutDirection.Vertical;
            _relativeSpacing = -1f; //a simple vertical layout top to bottom by default.
        }

        public LayoutDirection Direction { get { return _direction; } set { if (_direction == value) return; _direction = value; adjustLayout(); } }
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
        public float AbsoluteSpacing { get { return _absoluteSpacing; } set { if (_absoluteSpacing == value) return; _absoluteSpacing = value; adjustLayout(); } }
        public float RelativeSpacing { get { return _relativeSpacing; } set { if (_relativeSpacing == value) return; _relativeSpacing = value; adjustLayout(); } }
        public float StartLocation { get { return _startLocation; } set { if (_startLocation == value) return; _startLocation = value; adjustLayout(); } }
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
        public IEvent OnLayoutChanged { get; private set; }
        public IConcurrentHashSet<string> EntitiesToIgnore { get; private set; }

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);
            entity.Bind<IBoundingBoxWithChildrenComponent>(c => { _boundingBoxWithChildren = c; c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged); adjustLayout(); }, 
                                                    c => { c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged); _boundingBoxWithChildren = null; });
            entity.Bind<IInObjectTree>(c => { _tree = c; adjustLayout(); }, _ => _tree = null);
            EntitiesToIgnore.OnListChanged.Subscribe(onEntitiesToIgnoreChanged);
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

        private void onEntitiesToIgnoreChanged(AGSHashSetChangedEventArgs<string> args)
        {
            adjustLayout();
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
                if (!child.UnderlyingVisible || EntitiesToIgnore.Contains(child.ID)) continue;
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
