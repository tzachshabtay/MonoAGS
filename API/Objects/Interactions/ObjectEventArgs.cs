namespace AGS.API
{
    public class ObjectEventArgs : AGSEventArgs
	{
		public ObjectEventArgs (IObject obj)
		{
			Object = obj;
		}

		public IObject Object { get; private set; }

		public override string ToString ()
		{
			return Object == null ? "Null" : Object.ToString ();
		}
	}
}

