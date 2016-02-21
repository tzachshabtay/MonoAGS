using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractViewport : IContract<IViewport>
	{
		public ContractViewport()
		{
		}

		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }
		 
		[ProtoMember(3)]
		public float ScaleX { get; set; }

		[ProtoMember(4)]
		public float ScaleY { get; set; }
		 
		[ProtoMember(5)]
		public Contract<IFollower> Follower { get; set; }

		#region IContract implementation

		public IViewport ToItem(AGSSerializationContext context)
		{
			AGSViewport viewport = new AGSViewport ();
			viewport.X = X;
			viewport.Y = Y;
			viewport.ScaleX = ScaleX;
			viewport.ScaleY = ScaleY;
		
			viewport.Follower = Follower.ToItem(context);

			return viewport;
		}

		public void FromItem(AGSSerializationContext context, IViewport item)
		{
			Follower = new Contract<IFollower> ();
			Follower.FromItem(context, item.Follower);

			X = item.X;
			Y = item.Y;
			ScaleX = item.ScaleX;
			ScaleY = item.ScaleY;
		}

		#endregion
	}
}

