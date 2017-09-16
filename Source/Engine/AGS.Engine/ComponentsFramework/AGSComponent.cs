using System;
using System.ComponentModel;
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

        [Property(Browsable = false)]
        public virtual string Name { get { return friendlyName(); } }

		public virtual void Init(IEntity entity) {}
        public virtual void AfterInit() { }

        #endregion

        #region IDisposable implementation

        public virtual void Dispose()
		{
		}

		#endregion

        private string friendlyName()
        {
            return _type.Name
                        .Replace("AGS", "")
                        .Replace("Component", "")
                        .Replace("Behavior", "")
                        .Replace("Property", "");
        }
	}
}

