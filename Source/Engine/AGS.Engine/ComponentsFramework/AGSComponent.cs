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

        [Property(Browsable = false)]
        public IEntity Entity { get; private set; }

        [Property(Browsable = false)]
        public Type RegistrationType { get; private set; }

        public void Init(IEntity entity, Type registrationType) 
        { 
            Entity = entity;
            RegistrationType = registrationType;
            Init();
        }

        public virtual void Init() {}
        
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
            PropertyChanged = null;
            Entity = null;
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