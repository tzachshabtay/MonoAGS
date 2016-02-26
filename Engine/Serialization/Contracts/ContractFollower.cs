using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractFollower : IContract<IFollower>
	{
		public ContractFollower()
		{
		}

		[ProtoMember(1)]
		public bool Enabled { get; set; }

		#region IContract implementation

		public IFollower ToItem(AGSSerializationContext context)
		{
			var follower = new AGSViewportFollower { Enabled = Enabled };
			context.Rewire(state => follower.Target = () => state.Player.Character);
			return follower;
		}

		public void FromItem(AGSSerializationContext context, IFollower item)
		{
			Enabled = item.Enabled;
		}

		#endregion
	}
}

