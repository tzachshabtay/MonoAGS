using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public abstract class AGSComponent : API.IComponent, INotifyPropertyChanged
	{
		private Type _type;

        public event PropertyChangedEventHandler PropertyChanged;

        public int GetPropertyChangedSubscriberCount() => PropertyChanged?.GetInvocationList().Length ?? 0;

        public AGSComponent()
		{
			_type = GetType();
		}

        #region IComponent implementation

        [Property(Browsable = false)]
        public virtual string Name => friendlyName();

        public virtual void Init(IEntity entity) {}
        public virtual void AfterInit() { }

        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
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
                        .Replace("Property", "");
        }
	}
}

