using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractPointF : IContract<PointF>
	{
		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }

		#region IContract implementation

		public PointF ToItem(AGSSerializationContext context)
		{
			return new PointF (X, Y);
		}

		public void FromItem(AGSSerializationContext context, PointF item)
		{
			X = item.X;
			Y = item.Y;
		}

		#endregion
	}
}

