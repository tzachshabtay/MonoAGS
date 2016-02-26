using System;
using ProtoBuf;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimationContainer : IContract<IAnimationContainer>
	{
		public ContractAnimationContainer()
		{
		}

		[ProtoMember(1)]
		public IContract<IAnimation> Animation { get; set; }

		[ProtoMember(2)]
		public bool DebugDrawAnchor { get; set; }

		[ProtoMember(3)]
		public IContract<IBorderStyle> Border { get; set; }

		[ProtoMember(4)]
		public Contract<ISprite> Sprite { get; set; }

		#region IContract implementation

		public IAnimationContainer ToItem(AGSSerializationContext context)
		{
			ISprite sprite = Sprite.ToItem(context);
			float scaleX = sprite.ScaleX;
			float scaleY = sprite.ScaleY;
			IPoint anchor = sprite.Anchor;
			Color tint = sprite.Tint;
			AGSAnimationContainer container = new AGSAnimationContainer (sprite,
				context.Factory.Graphics);
			container.Tint = tint;
			container.Anchor = anchor;
			container.DebugDrawAnchor = DebugDrawAnchor;
			container.Border = Border.ToItem(context);
			container.PixelPerfect(sprite.PixelPerfectHitTestArea != null);
			IAnimation animation = Animation.ToItem(context);
			if (animation != null)
			{
				container.StartAnimation(animation);
				if (animation.Frames.Count > 0)
					container.ScaleBy(scaleX, scaleY);
			}

			return container;
		}

		public void FromItem(AGSSerializationContext context, IAnimationContainer item)
		{
			Sprite = new Contract<ISprite> ();
			Sprite.FromItem(context, item);

			Animation = context.GetContract(item.Animation);
			Border = context.GetContract(item.Border);

			DebugDrawAnchor = item.DebugDrawAnchor;
		}

		#endregion
	}
}

