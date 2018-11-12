using System;
using AGS.API;
using ProtoBuf;


namespace AGS.Engine
{
	[ProtoContract]
	public class ContractLoadImageConfig : IContract<ILoadImageConfig>
	{
	    [ProtoMember(1)]
		public int? TransparentColorSamplePointX { get; set; }

		[ProtoMember(2)]
		public int? TransparentColorSamplePointY { get; set; }

		#region IContract implementation

		public ILoadImageConfig ToItem(AGSSerializationContext context)
		{
            return new AGSLoadImageConfig(TransparentColorSamplePointX == null ? (Point?)null :
                                          new Point(TransparentColorSamplePointX.Value, TransparentColorSamplePointY.Value));
		}

		public void FromItem(AGSSerializationContext context, ILoadImageConfig item)
		{
			TransparentColorSamplePointX = item.TransparentColorSamplePoint == null ? (int?)null : item.TransparentColorSamplePoint.Value.X;
			TransparentColorSamplePointY = item.TransparentColorSamplePoint == null ? (int?)null : item.TransparentColorSamplePoint.Value.Y;
		}

		#endregion
	}
}

