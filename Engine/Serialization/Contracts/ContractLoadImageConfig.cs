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
		public Point? TransparentColorSamplePoint { get; set; }

		#region IContract implementation

		public ILoadImageConfig ToItem(AGSSerializationContext context)
		{
			return new AGSLoadImageConfig { TransparentColorSamplePoint = TransparentColorSamplePoint };
		}

		public void FromItem(AGSSerializationContext context, ILoadImageConfig item)
		{
			TransparentColorSamplePoint = item.TransparentColorSamplePoint;
		}

		#endregion
	}
}

