using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractBlend : IContract<IBlend>
	{
		[ProtoMember(1)]
		public float[] Factors { get; set; }

		[ProtoMember(2)]
		public float[] Positions { get; set; }

		#region IContract implementation

		public IBlend ToItem(AGSSerializationContext context)
		{
			return new AGSBlend (Factors, Positions);
		}

		public void FromItem(AGSSerializationContext context, IBlend item)
		{
			Factors = item.Factors;
			Positions = item.Positions;
		}

		#endregion
		
	}
}

