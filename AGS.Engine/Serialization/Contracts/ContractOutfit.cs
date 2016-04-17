using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractOutfit : IContract<IOutfit>
	{
		public ContractOutfit()
		{
		}

		[ProtoMember(1)]
		public Contract<IDirectionalAnimation> WalkAnimation { get; set; }

		[ProtoMember(2)]
		public Contract<IDirectionalAnimation> IdleAnimation { get; set; }

		[ProtoMember(3)]
		public Contract<IDirectionalAnimation> SpeakAnimation { get; set; }

		#region IContract implementation

		public IOutfit ToItem(AGSSerializationContext context)
		{
			IOutfit item = new AGSOutfit ();
			item.WalkAnimation = WalkAnimation.ToItem(context);
			item.IdleAnimation = IdleAnimation.ToItem(context);
			item.SpeakAnimation = SpeakAnimation.ToItem(context);

			return item;
		}

		public void FromItem(AGSSerializationContext context, IOutfit item)
		{
			WalkAnimation = fromItem(context, item.WalkAnimation);
			IdleAnimation = fromItem(context, item.IdleAnimation);
			SpeakAnimation = fromItem(context, item.SpeakAnimation);
		}

		#endregion

		private Contract<IDirectionalAnimation> fromItem(AGSSerializationContext context, IDirectionalAnimation animation)
		{
			Contract<IDirectionalAnimation> item = new Contract<IDirectionalAnimation> ();
			item.FromItem(context, animation);
			return item;
		}
	}
}

