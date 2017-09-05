using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSObjectBoolParentProperty : AGSComponent
	{
		private readonly Predicate<IObject> _getProperty;
        private readonly Func<IObject, IEvent> _getOnValueChanged;
		private IInObjectTree _tree;
        private bool _underlyingValue, _lastValue, _initializedValue;
        private IObject _lastParent;
        private IEntity _entity;

        public AGSObjectBoolParentProperty(Predicate<IObject> getProperty, Func<IObject, IEvent> getOnValueChanged)
		{
            OnUnderlyingValueChanged = new AGSEvent();
            OnValueChanged = new AGSEvent();
			_getProperty = getProperty;
            _getOnValueChanged = getOnValueChanged;
            UnderlyingValue = true;
		}

		public override void Init(IEntity entity)
		{
            _entity = entity;
			base.Init(entity);

		}

        public override void AfterInit()
        {
            base.AfterInit();
			_entity.Bind<IInObjectTree>(c => { _tree = c; c.TreeNode.OnParentChanged.Subscribe(onParentChanged); onParentChanged(); },
									   c => { _tree = null; c.TreeNode.OnParentChanged.Unsubscribe(onParentChanged); });
        }
			
		public bool Value 
		{
			get 
            {
                var tree = _tree;
                if (tree == null || tree.TreeNode == null) return UnderlyingValue;
                return getBooleanFromParentIfNeeded(tree.TreeNode.Parent); 
            }
			set { UnderlyingValue = value;}
		}

        public bool UnderlyingValue 
        { 
            get { return _underlyingValue; }
            private set
            {
                if (_underlyingValue == value) return;
                _underlyingValue = value;
                OnUnderlyingValueChanged.Invoke();
                refreshValue();
            }
        }

        public IEvent OnUnderlyingValueChanged { get; private set; }

        public IEvent OnValueChanged { get; private set; }

		private bool getBooleanFromParentIfNeeded(IObject parent)
		{
			if (!UnderlyingValue) return false;
			if (parent == null) return true;
			return _getProperty(parent);
		}

        private void onParentChanged()
        {
            var lastParent = _lastParent;
            if (lastParent != null)
            {
                _getOnValueChanged(lastParent).Unsubscribe(refreshValue);
            }
            var newParent = _tree.TreeNode.Parent;
            if (newParent != null)
            {
                _getOnValueChanged(newParent).Subscribe(refreshValue);
            }
            _lastParent = newParent;
            refreshValue();
        }

        private void refreshValue()
        {
            var newValue = Value;
			if (!_initializedValue)
			{
                _initializedValue = true;
                _lastValue = newValue;
                OnValueChanged.Invoke();
                return;
			}
            if (_lastValue == newValue) return;
            _lastValue = newValue;
            OnValueChanged.Invoke();
        }
	}

	public class VisibleProperty : AGSObjectBoolParentProperty, IVisibleComponent
	{
        public VisibleProperty() : base(o => o.Visible, o => o.OnVisibleChanged){}
		public bool Visible { get { return Value; } set { Value = value; } }
		public bool UnderlyingVisible { get { return UnderlyingValue; } }
        public IEvent OnUnderlyingVisibleChanged { get { return OnUnderlyingValueChanged; } }
        public IEvent OnVisibleChanged { get { return OnValueChanged; } }
	}

	public class EnabledProperty : AGSObjectBoolParentProperty, IEnabledComponent
	{
        public EnabledProperty() : base(o => o.Enabled, o => o.OnEnabledChanged){}
        public bool Enabled { get { return Value; } set { Value = value; } }
		public bool UnderlyingEnabled { get { return UnderlyingValue; } }
        public bool ClickThrough { get; set; }
		public IEvent OnUnderlyingEnabledChanged { get { return OnUnderlyingValueChanged; } }
		public IEvent OnEnabledChanged { get { return OnValueChanged; } }
	}
}

