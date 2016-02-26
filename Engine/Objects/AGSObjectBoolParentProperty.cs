using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSObjectBoolParentProperty
	{
		private readonly IObject _obj;
		private readonly Predicate<IObject> _getProperty;

		public AGSObjectBoolParentProperty(IObject obj, Predicate<IObject> getProperty)
		{
			_obj = obj;
			_getProperty = getProperty;
			UnderlyingValue = true;
		}
			
		public bool Value 
		{
			get { return getBooleanFromParentIfNeeded(_obj.TreeNode.Parent); }
			set { UnderlyingValue = value;}
		}

		public bool UnderlyingValue { get; private set; }

		private bool getBooleanFromParentIfNeeded(IObject parent)
		{
			if (!UnderlyingValue) return false;
			if (parent == null) return true;
			return _getProperty(parent);
		}
	}

	public class VisibleProperty : AGSObjectBoolParentProperty
	{
		public VisibleProperty(IObject obj) : base(obj, o => o.Visible){}
	}

	public class EnabledProperty : AGSObjectBoolParentProperty
	{
		public EnabledProperty(IObject obj) : base(obj, o => o.Enabled){}
	}
}

