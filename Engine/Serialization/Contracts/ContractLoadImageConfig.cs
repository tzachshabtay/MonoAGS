using System;
using AGS.API;
using ProtoBuf;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractLoadImageConfig : IContract<ILoadImageConfig>
	{
		public ContractLoadImageConfig()
		{
		}

		[ProtoMember(1)]
		public float? TransparentColorSamplePointX { get; set; }

		[ProtoMember(2)]
		public float? TransparentColorSamplePointY { get; set; }

		#region IContract implementation

		public ILoadImageConfig ToItem(AGSSerializationContext context)
		{
			return new AGSLoadImageConfig { TransparentColorSamplePoint = TransparentColorSamplePointX == null ? (AGSPoint?)null :
					new AGSPoint (TransparentColorSamplePointX.Value, TransparentColorSamplePointY.Value)
			};
		}

		public void FromItem(AGSSerializationContext context, ILoadImageConfig item)
		{
			TransparentColorSamplePointX = item.TransparentColorSamplePoint == null ? (float?)null : item.TransparentColorSamplePoint.X;
			TransparentColorSamplePointY = item.TransparentColorSamplePoint == null ? (float?)null : item.TransparentColorSamplePoint.Y;
		}

		#endregion
	}
}

