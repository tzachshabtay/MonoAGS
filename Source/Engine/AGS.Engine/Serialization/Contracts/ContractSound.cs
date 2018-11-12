using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractSound : IContract<ISound>
	{
	    public ISound ToItem(AGSSerializationContext context)
		{
			return null;
		}

		public void FromItem(AGSSerializationContext context, ISound sound)
		{
			
		}
	}
}

