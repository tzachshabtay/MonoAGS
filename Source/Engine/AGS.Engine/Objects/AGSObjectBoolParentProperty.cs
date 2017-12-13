using System;
using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public abstract class AGSObjectBoolParentProperty<TComponent> : AGSComponent where TComponent : IComponent
    {
        private readonly Predicate<IObject> _getProperty;
        private readonly string _valuePropertyName, _underlyingPropertyName;
        private IInObjectTree _tree;
        private bool _underlyingValue, _lastValue, _initializedValue;
        private IComponent _lastParentComponent;
        private IComponentBinding _lastParentBinding;
        private IEntity _entity;

        public AGSObjectBoolParentProperty(Predicate<IObject> getProperty,
                                           string valuePropertyName, string underlyingPropertyName)
        {
            _valuePropertyName = valuePropertyName;
            _underlyingPropertyName = underlyingPropertyName;
            _getProperty = getProperty;
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

        [Property(Browsable = false)]
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

        [Property(Browsable = false)]
        public bool UnderlyingValue 
        {
            get => _underlyingValue;
            private set
            {
                if (_underlyingValue == value) return;
                _underlyingValue = value;
                OnPropertyChanged(_underlyingPropertyName);
                refreshValue();
            }
        }

		private bool getBooleanFromParentIfNeeded(IObject parent)
		{
			if (!UnderlyingValue) return false;
			if (parent == null) return true;
			return _getProperty(parent);
		}

        private void onParentChanged()
        {
            var lastParentBinding = _lastParentBinding;
            if (lastParentBinding != null)
            {
                lastParentBinding.Unbind();
            }
            var lastParentComponent = _lastParentComponent;
            if (lastParentComponent != null)
            {
                lastParentComponent.PropertyChanged -= onParentPropertyChanged;
            }
            var newParent = _tree.TreeNode.Parent;
            if (newParent != null)
            {
                _lastParentBinding = newParent.Bind<TComponent>(
                    c => { _lastParentComponent = c; c.PropertyChanged += onParentPropertyChanged; },
                    c => { _lastParentComponent = null; c.PropertyChanged -= onParentPropertyChanged; });
            }
            refreshValue();
        }

        private void onParentPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != _valuePropertyName) return;
            refreshValue();
        }

        private void refreshValue()
        {
            var newValue = Value;
			if (!_initializedValue)
			{
                _initializedValue = true;
                _lastValue = newValue;
                OnPropertyChanged(_valuePropertyName);
                return;
			}
            if (_lastValue == newValue) return;
            _lastValue = newValue;
            OnPropertyChanged(_valuePropertyName);
        }
	}

	public class VisibleProperty : AGSObjectBoolParentProperty<IVisibleComponent>, IVisibleComponent
	{
        public VisibleProperty() : base(o => o.Visible, nameof(Visible), nameof(UnderlyingVisible)){}

        [DoNotNotify]
		public bool Visible { get => Value; set => Value = value; }

        [DoNotNotify]
        public bool UnderlyingVisible => UnderlyingValue;
    }

	public class EnabledProperty : AGSObjectBoolParentProperty<IEnabledComponent>, IEnabledComponent
	{
        public EnabledProperty() : base(o => o.Enabled, nameof(Enabled), nameof(UnderlyingEnabled)){}

        [DoNotNotify]
        public bool Enabled { get => Value; set => Value = value; }

        [DoNotNotify]
        public bool UnderlyingEnabled => UnderlyingValue;

        public bool ClickThrough { get; set; }
	}
}

