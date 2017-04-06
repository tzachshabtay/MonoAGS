using System;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class AGSObjectBoolParentProperty : AGSComponent
	{
		private readonly Predicate<IObject> _getProperty;
		private IInObjectTree _tree;
        private readonly IGameState _state;
        private readonly IInput _input;
        private IObject _obj;

        public AGSObjectBoolParentProperty(Predicate<IObject> getProperty, IGameState state, IInput input)
		{
            _state = state;
            _input = input;
			_getProperty = getProperty;
            UnderlyingValue = true;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
            _obj = entity as IObject;
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

        public bool UnderlyingValue { get; private set; }

		private bool getBooleanFromParentIfNeeded(IObject parent)
		{
			if (!UnderlyingValue) return false;
            if (_obj != null)
            {
                //todo: make this more efficient
                if (_obj != _input.Cursor && !_state.UI.Contains(_obj) && 
                    (_state.Room == null || !_state.Room.Objects.Contains(_obj)) &&
                    (_state.Room == null || !_state.Room.Areas.Select(a => a.Mask.DebugDraw).Contains(_obj)))
                    return false;
            }
			if (parent == null) return true;
			return _getProperty(parent);
		}
	}

	public class VisibleProperty : AGSObjectBoolParentProperty, IVisibleComponent
	{
        public VisibleProperty(IGameState state, IInput input) : 
            base(o => o.Visible, state, input){}
		public bool Visible { get { return Value; } set { Value = value; } }
		public bool UnderlyingVisible { get { return UnderlyingValue; } }
	}

	public class EnabledProperty : AGSObjectBoolParentProperty, IEnabledComponent
	{
		public EnabledProperty(IGameState state, IInput input) : 
            base(o => o.Enabled, state, input){}
		public bool Enabled { get { return Value; } set { Value = value; } }
		public bool UnderlyingEnabled { get { return UnderlyingValue; } }
	}
}

