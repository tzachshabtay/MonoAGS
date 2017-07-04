using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSObjectBoolParentProperty : AGSComponent
	{
		private readonly Predicate<IObject> _getProperty;
		private IInObjectTree _tree;
        private bool _underlyingValue;

        public AGSObjectBoolParentProperty(Predicate<IObject> getProperty)
		{
            OnUnderlyingValueChanged = new AGSEvent<object>();
			_getProperty = getProperty;
            UnderlyingValue = true;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_tree = entity.GetComponent<IInObjectTree>();
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
                _underlyingValue = value;
                OnUnderlyingValueChanged.Invoke(null);
            }
        }

        public IEvent<object> OnUnderlyingValueChanged { get; private set; }

		private bool getBooleanFromParentIfNeeded(IObject parent)
		{
			if (!UnderlyingValue) return false;
			if (parent == null) return true;
			return _getProperty(parent);
		}
	}

	public class VisibleProperty : AGSObjectBoolParentProperty, IVisibleComponent
	{
        public VisibleProperty() : base(o => o.Visible){}
		public bool Visible { get { return Value; } set { Value = value; } }
		public bool UnderlyingVisible { get { return UnderlyingValue; } }
        public IEvent<object> OnUnderlyingVisibleChanged { get { return OnUnderlyingValueChanged; } }
	}

	public class EnabledProperty : AGSObjectBoolParentProperty, IEnabledComponent
	{
		public EnabledProperty() : base(o => o.Enabled){}
		public bool Enabled { get { return Value; } set { Value = value; } }
		public bool UnderlyingEnabled { get { return UnderlyingValue; } }
	}
}

