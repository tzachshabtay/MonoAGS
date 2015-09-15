using System;

namespace AGS.API
{
	public class CustomInteractionEventArgs : ObjectEventArgs
	{
		public CustomInteractionEventArgs (IObject obj, string interactionName) : base(obj)
		{
			InteractionName = interactionName;
		}

		public string InteractionName { get; private set; }

		public override string ToString ()
		{
			return string.Format ("Custom interaction {0} on {1}", InteractionName ?? "Null", 
				Object == null ? "Null" : Object.ToString());
		}
	}
}

