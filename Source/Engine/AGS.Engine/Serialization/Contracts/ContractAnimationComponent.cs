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

		[ProtoMember(2)]
		public bool DebugDrawAnchor { get; set; }

		[ProtoMember(3)]
		public IContract<IBorderStyle> Border { get; set; }

		#region IContract implementation

		public IAnimationComponent ToItem(AGSSerializationContext context)
		{
            AGSAnimationComponent container = new AGSAnimationComponent();
			ToItem(context, container);
			return container;
		}

		public void ToItem(AGSSerializationContext context, IAnimationComponent container)
		{
			container.DebugDrawAnchor = DebugDrawAnchor;
			container.Border = Border.ToItem(context);
			IAnimation animation = Animation.ToItem(context);
			if (animation != null)
			{
				container.StartAnimation(animation);
			}
		}

		public void FromItem(AGSSerializationContext context, IAnimationComponent item)
		{
			Animation = context.GetContract(item.Animation);
			Border = context.GetContract(item.Border);

			DebugDrawAnchor = item.DebugDrawAnchor;			
		}

		#endregion
	}
}

