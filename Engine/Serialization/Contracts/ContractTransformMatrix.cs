using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractTransformMatrix : IContract<ITransformMatrix>
	{
		[ProtoMember(1)]
		public float[] Elements { get; set; }

		#region IContract implementation

		public ITransformMatrix ToItem(AGSSerializationContext context)
		{
			return new AGSTransformMatrix (Elements);
		}

		public void FromItem(AGSSerializationContext context, ITransformMatrix item)
		{
			Elements = item.Elements;
		}

		#endregion
	}
}

