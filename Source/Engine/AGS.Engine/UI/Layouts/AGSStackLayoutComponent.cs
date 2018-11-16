using System;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSStackLayoutComponent : AGSComponent, IStackLayoutComponent
    {
        private IInObjectTreeComponent _tree;
        private float _absoluteSpacing, _relativeSpacing, _startLocation;
        private LayoutDirection _direction;
        private bool _isPaused;
        private int _pendingLayouts;
        private EntityListSubscriptions<IObject> _subscriptions;
        private IGameEvents _gameEvents;
        private bool _isDirty, _layoutAfterCrop, _centerLayout;
        private int _inUpdate; //For preventing re-entrancy

        public AGSStackLayoutComponent(IGameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _isPaused = true;
            _isDirty = true;
            OnLayoutChanged = new AGSEvent();
            EntitiesToIgnore = new AGSConcurrentHashSet<string>();
            _direction = LayoutDirection.Vertical;
            _relativeSpacing = -1f; //a simple vertical layout top to bottom by default.

            //Using low callback priority to make sure we only adjust the layout after all bounding box calculations has already happened.
            //Otherwise, this scenario can happen: the entity with the stack layout can subscribe to the bounding box with children event,
            //and adjust the starting point of the layout to start from the new top of the bounding box whenever the size changes.
            //If the stack layout updates in the middle of all the child bounding box calculations, it can trigger an endless loop:
            //layout update -> bounding box with children update -> layout update -> etc, 
            //because the box in the bottom of the layout might remain below the bottom of the parent box border and increase the size of the parent each time.
            gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute, CallbackPriority.Low);
        }

        public LayoutDirection Direction { get => _direction; set { _direction = value; onSomethingChanged(); } }
        public float AbsoluteSpacing { get => _absoluteSpacing; set { _absoluteSpacing = value; onSomethingChanged(); } }
        public float RelativeSpacing { get => _relativeSpacing; set { _relativeSpacing = value; onSomethingChanged(); } }
        public float StartLocation { get => _startLocation; set { _startLocation = value; onSomethingChanged(); } }
        public bool LayoutAfterCrop { get => _layoutAfterCrop; set { _layoutAfterCrop = value; onSomethingChanged(); } }
        public bool CenterLayout { get => _centerLayout; set { _centerLayout = value; onSomethingChanged(); } }
        public IBlockingEvent OnLayoutChanged { get; }
        public IConcurrentHashSet<string> EntitiesToIgnore { get; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IInObjectTreeComponent>(c => { _tree = c; subscribeChildren(); onSomethingChanged(); },
                                                c => { _tree = null; unsubscribeChildren(); });
            EntitiesToIgnore.OnListChanged.Subscribe(onEntitiesToIgnoreChanged);
        }

        public void StartLayout()
        {
            _isPaused = false;
        }

        public void StopLayout()
        {
            _isPaused = true;
        }

        public void ForceRefreshLayout() => adjustLayout();

        public float DryRun()
        {
            var tree = _tree;
            if (tree == null) throw new NullReferenceException("tree");
            return Math.Abs(adjustLayout(tree, true, _centerLayout));
        }

        public override void Dispose()
        {
            base.Dispose();
            StopLayout();
            EntitiesToIgnore?.OnListChanged?.Unsubscribe(onEntitiesToIgnoreChanged);
            _gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            unsubscribeChildren();
        }

        private void onRepeatedlyExecute()
        {
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                if (!_isDirty) return;
                adjustLayout();
            }
            finally
            {
                _inUpdate = 0;
            }
        }

        private void subscribeChildren()
        {
            var boundingBoxSubscription = new EntitySubscription<IBoundingBoxWithChildrenComponent>(null,
                c => c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged),
                c => c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged));

            var visibleSubscription = new EntitySubscription<IVisibleComponent>(onVisibleChanged, propertyNames: nameof(IVisibleComponent.Visible));

            _subscriptions = new EntityListSubscriptions<IObject>(_tree.TreeNode.Children, false, onSomethingChanged, boundingBoxSubscription, visibleSubscription);
        }

        private void unsubscribeChildren()
        {
            _subscriptions?.Unsubscribe();
        }

        private void onSomethingChanged()
        {
            _isDirty = true;
        }

        private void onVisibleChanged()
        {
            onSomethingChanged();
        }

        private void onSizeChanged()
        {
            onSomethingChanged();
        }

        private void onEntitiesToIgnoreChanged(AGSHashSetChangedEventArgs<string> args)
        {
            onSomethingChanged();
        }

        private void adjustLayout()
        {
            if (_isPaused) return;

            var tree = _tree;
            if (tree == null) return;

            int pendingLayouts = Interlocked.Increment(ref _pendingLayouts);
            if (pendingLayouts > 1)
            {
                return;
            }

            adjustLayout(tree, false, _centerLayout);
        }

        private AGSBoundingBox? getChildBox(IEntity child)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (_relativeSpacing == 0f) return null;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            if (_layoutAfterCrop) return child.AddComponent<IBoundingBoxWithChildrenComponent>()?.BoundingBoxWithChildren;
            return child.AddComponent<IBoundingBoxWithChildrenComponent>()?.PreCropBoundingBoxWithChildren;
        }

        private float adjustLayout(IInObjectTreeComponent tree, bool isDryRun, bool centerLayout)
        {
            float location = isDryRun ? 0f : StartLocation;
            if (centerLayout && !isDryRun)
            {
                float size = adjustLayout(tree, true, false);
                location = StartLocation - size / 2f;
            }
            try
            {
                if (!isDryRun)
                {
                    _isDirty = false;
                }
                var lockStep = isDryRun ? null : new TreeLockStep(tree, obj => obj.UnderlyingVisible && !EntitiesToIgnore.Contains(obj.ID));
                lockStep?.Lock();
                foreach (var child in tree.TreeNode.Children)
                {
                    if (!child.UnderlyingVisible || EntitiesToIgnore.Contains(child.ID)) continue;
                    float step;
                    if (Direction == LayoutDirection.Vertical)
                    {
                        if (!isDryRun)
                        {
                            child.Y = location;
                        }
                        step = getChildBox(child)?.Height ?? 0f;
                    }
                    else
                    {
                        if (!isDryRun)
                        {
                            child.X = location;
                        }
                        step = getChildBox(child)?.Width ?? 0f;
                    }
                    location += step * RelativeSpacing + AbsoluteSpacing;
                }
                lockStep?.Unlock();
            }
            finally
            {
                if (!isDryRun)
                {
                    if (Interlocked.Exchange(ref _pendingLayouts, 0) <= 1)
                    {
                        OnLayoutChanged.Invoke();
                    }
                    else
                    {
                        location = adjustLayout(tree, false, centerLayout);
                    }
                }
            }
            return Math.Abs(location - AbsoluteSpacing);
        }
	}
}
