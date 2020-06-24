﻿using System;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public abstract class AGSObjectBoolParentProperty<TComponent> : AGSComponent where TComponent : IComponent
    {
        private readonly Predicate<IObject> _getProperty;
        private readonly string _valuePropertyName, _underlyingPropertyName;
        private IInObjectTreeComponent _tree;
        private bool _underlyingValue, _lastValue, _initializedValue;
        private IComponent _lastParentComponent;
        private IComponentBinding _lastParentBinding;

        public AGSObjectBoolParentProperty(Predicate<IObject> getProperty,
                                           string valuePropertyName, string underlyingPropertyName)
        {
            _valuePropertyName = valuePropertyName;
            _underlyingPropertyName = underlyingPropertyName;
            _getProperty = getProperty;
            UnderlyingValue = true;
        }

        public override void AfterInit()
        {
            base.AfterInit();
            Entity.Bind<IInObjectTreeComponent>(c => { _tree = c; c.TreeNode.OnParentChanged.Subscribe(onParentChanged); onParentChanged(); },
                                       c => { _tree = null; c.TreeNode.OnParentChanged.Unsubscribe(onParentChanged); });
        }

        public override void Dispose()
        {
            base.Dispose();
            _lastParentBinding?.Unbind();
            _lastParentBinding = null;
            _lastParentComponent = null;
        }

        [Property(Browsable = false)]
		public bool Value 
		{
            get => _lastValue;
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
            _lastParentBinding?.Unbind();
            var lastParentComponent = _lastParentComponent;
            if (lastParentComponent != null)
            {
                lastParentComponent.PropertyChanged -= onParentPropertyChanged;
            }

            _lastParentBinding = _tree.TreeNode.Parent?.Bind<TComponent>(
                    c => { _lastParentComponent = c; c.PropertyChanged += onParentPropertyChanged; },
                    c => { _lastParentComponent = null; c.PropertyChanged -= onParentPropertyChanged; });
            
            refreshValue();
        }

        private void onParentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName != _valuePropertyName) return;
            refreshValue();
        }

        private void refreshValue()
        {
            var newValue = calculateValue();
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

        private bool calculateValue()
        {
            var tree = _tree;
            if (tree == null || tree.TreeNode == null) return UnderlyingValue;
            return getBooleanFromParentIfNeeded(tree.TreeNode.Parent);
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
