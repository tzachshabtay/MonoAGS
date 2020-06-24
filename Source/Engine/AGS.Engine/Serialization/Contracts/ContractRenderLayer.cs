using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractRenderLayer : IContract<IRenderLayer>
	{
	    [ProtoMember(1)]
		public int Z { get; set; }

		#region IContract implementation

		public IRenderLayer ToItem(AGSSerializationContext context)
		{
			return new AGSRenderLayer (Z, new PointF (1f, 1f));
		}

		public void FromItem(AGSSerializationContext context, IRenderLayer item)
		{
			Z = item.Z;
		}

		#endregion
	}
}

