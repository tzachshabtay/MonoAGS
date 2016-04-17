using System;
using ProtoBuf;

namespace AGS.Engine
{
	public interface IContract<TItem>
	{
		TItem ToItem(AGSSerializationContext context);
		void FromItem(AGSSerializationContext context, TItem item);
	}
}

