using System;
using ProtoBuf;
using AGS.API;


namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimationComponent : IContract<IAnimationComponent>
	{
		public ContractAnimationComponent()
		{
		}

		[ProtoMember(1)]
		public IContract<IAnimation> Animation { get; set; }

		#region IContract implementation

		public IAnimationComponent ToItem(AGSSerializationContext context)
		{
            AGSAnimationComponent container = new AGSAnimationComponent();
			ToItem(context, container);
			return container;
		}

		public void ToItem(AGSSerializationContext context, IAnimationComponent container)
		{
			IAnimation animation = Animation.ToItem(context);
			if (animation != null)
			{
				container.StartAnimation(animation);
			}
		}

		public void FromItem(AGSSerializationContext context, IAnimationComponent item)
		{
			Animation = context.GetContract(item.Animation);
		}

		#endregion
	}
}

