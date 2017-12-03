using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public abstract class AGSComponent : IComponent, INotifyPropertyChanged
	{
		private Type _type;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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

