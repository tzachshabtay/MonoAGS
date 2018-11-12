using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractDirectionalAnimation : IContract<IDirectionalAnimation>
	{
	    [ProtoMember(1)]
		public Contract<IAnimation> Left { get; set; }

		[ProtoMember(2)]
		public Contract<IAnimation> Right { get; set; }

		[ProtoMember(3)]
		public Contract<IAnimation> Up { get; set; }

		[ProtoMember(4)]
		public Contract<IAnimation> Down { get; set; }

		[ProtoMember(5)]
		public Contract<IAnimation> UpLeft { get; set; }

		[ProtoMember(6)]
		public Contract<IAnimation> UpRight { get; set; }

		[ProtoMember(7)]
		public Contract<IAnimation> DownLeft { get; set; }

		[ProtoMember(8)]
		public Contract<IAnimation> DownRight { get; set; }

		#region IContract implementation

		public IDirectionalAnimation ToItem(AGSSerializationContext context)
		{
			IDirectionalAnimation item = new AGSDirectionalAnimation ();
			item.Left = Left.ToItem(context);
			item.Right = Right.ToItem(context);
			item.Up = Up.ToItem(context);
			item.Down = Down.ToItem(context);
			item.UpLeft = UpLeft.ToItem(context);
			item.UpRight = UpRight.ToItem(context);
			item.DownLeft = DownLeft.ToItem(context);
			item.DownRight = DownRight.ToItem(context);

			return item;
		}

		public void FromItem(AGSSerializationContext context, IDirectionalAnimation item)
		{
			Left = fromItem(context, item.Left);
			Right = fromItem(context, item.Right);
			Up = fromItem(context, item.Up);
			Down = fromItem(context, item.Down);
			UpLeft = fromItem(context, item.UpLeft);
			UpRight = fromItem(context, item.UpRight);
			DownLeft = fromItem(context, item.DownLeft);
			DownRight = fromItem(context, item.DownRight);
		}

		#endregion

		private Contract<IAnimation> fromItem(AGSSerializationContext context, IAnimation animation)
		{
			Contract<IAnimation> contract = new Contract<IAnimation> ();
			contract.FromItem(context, animation);
			return contract;
		}
	}
}

