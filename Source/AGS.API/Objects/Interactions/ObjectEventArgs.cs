namespace AGS.API
{
    /// <summary>
    /// Event arguments for interacting with an object.
    /// </summary>
    public class ObjectEventArgs
	{
		public ObjectEventArgs (IObject obj)
		{
			Object = obj;
		}

        /// <summary>
        /// The object being interacted with.
        /// </summary>
        /// <value>The object.</value>
		public IObject Object { get; private set; }

		public override string ToString ()
		{
			return Object == null ? "Null" : Object.ToString ();
		}
	}
}

