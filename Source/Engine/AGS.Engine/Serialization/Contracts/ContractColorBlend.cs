using System.Linq;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractColorBlend : IContract<IColorBlend>
	{
		[ProtoMember(1)]
		public uint[] Colors { get; set; }

		[ProtoMember(2)]
		public float[] Positions { get; set; }

		#region IContract implementation

		public IColorBlend ToItem(AGSSerializationContext context)
		{
			return new AGSColorBlend (Colors.Select(c => Color.FromHexa(c)).ToArray(), Positions);
		}

		public void FromItem(AGSSerializationContext context, IColorBlend item)
		{
			Colors = item.Colors.Select(c => c.Value).ToArray();
			Positions = item.Positions;
		}

		#endregion

	}
}

