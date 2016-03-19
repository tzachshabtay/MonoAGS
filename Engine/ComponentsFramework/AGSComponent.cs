using System;
using AGS.API;

namespace AGS.Engine
{
	public abstract class AGSComponent : IComponent
	{
		private Type _type;

		public AGSComponent()
		{
			_type = GetType();
		}

		#region IComponent implementation

		public virtual string Name { get { return _type.Name; } }

		public virtual bool AllowMultiple { get { return false; } }

		public bool Enabled { get; set; }

		#endregion

		#region IDisposable implementation

		public virtual void Dispose()
		{
		}

		#endregion
	}
}

