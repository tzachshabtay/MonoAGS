using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractEdge : IContract<IEdge>
	{
	    [ProtoMember(1)]
		public float Value { get; set; }

		#region IContract implementation

		public IEdge ToItem(AGSSerializationContext context)
		{
			AGSEdge edge = new AGSEdge ();
			edge.Value = Value;

			return edge;
		}

		public void FromItem(AGSSerializationContext context, IEdge item)
		{
			Value = item.Value;
		}

		#endregion
	}
}

