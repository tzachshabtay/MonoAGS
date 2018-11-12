using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimationConfiguration : IContract<IAnimationConfiguration>
	{
	    [ProtoMember(1)]
		public LoopingStyle Looping { get; set; }

		[ProtoMember(2)]
		public int Loops { get; set; }

		#region IContract implementation

		public IAnimationConfiguration ToItem(AGSSerializationContext context)
		{
			AGSAnimationConfiguration config = new AGSAnimationConfiguration ();
			config.Loops = Loops;
			config.Looping = Looping;
			return config;
		}

		public void FromItem(AGSSerializationContext context, IAnimationConfiguration item)
		{
			Loops = item.Loops;
			Looping = item.Looping;
		}

		#endregion
	}
}

