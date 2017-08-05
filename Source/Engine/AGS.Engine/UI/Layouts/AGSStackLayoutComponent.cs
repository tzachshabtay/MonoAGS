using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        private ISizeWithChildrenComponent _size;
        private IInObjectTree _tree;
        private float _absoluteSpacing, _relativeSpacing;
        private LayoutDirection _direction;
        private bool _isPaused;
        private IEntity _entity;

        public AGSStackLayoutComponent()
        {
            _isPaused = true;
            OnLayoutChanged = new AGSEvent<object>();
            _direction = LayoutDirection.Vertical;
            _relativeSpacing = -1f; //a simple vertical layout top to bottom by default.
        }

        public LayoutDirection Direction { get { return _direction; } set { _direction = value; adjustLayout(); } }
        public float AbsoluteSpacing { get { return _absoluteSpacing; } set { _absoluteSpacing = value; adjustLayout(); } }
        public float RelativeSpacing { get { return _relativeSpacing; } set { _relativeSpacing = value; adjustLayout(); } }
        public IEvent<object> OnLayoutChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            _entity = entity;
            base.Init(entity);
            entity.Bind<ISizeWithChildrenComponent>(c => { _size = c; c.OnSizeWithChildrenChanged.Subscribe(onSizeChanged); adjustLayout(); }, 
                                                    c => { c.OnSizeWithChildrenChanged.Unsubscribe(onSizeChanged); _size = null; });
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

        private void onSizeChanged(object state)
        {
            adjustLayout();
        }

        private void adjustLayout()
        {
            if (_isPaused) return;
            float location = 0f;

            var tree = _tree;
            if (tree == null) return;
            foreach (var child in tree.TreeNode.Children)
            {
                if (!child.UnderlyingVisible) continue;
                float step;
                if (Direction == LayoutDirection.Vertical)
                {
                    child.Y = location;
                    step = child.AddComponent<ISizeWithChildrenComponent>().SizeWithChildren.Height;
                }
                else
                {
                    child.X = location;
                    step = child.AddComponent<ISizeWithChildrenComponent>().SizeWithChildren.Width;
                }
                location += step * RelativeSpacing + AbsoluteSpacing;
            }
            OnLayoutChanged.Invoke(null);
        }


	}
}
