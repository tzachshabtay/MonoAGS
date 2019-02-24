using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimationFrame : IContract<IAnimationFrame>
	{
	    [ProtoMember(1)]
		public Contract<ISprite> Sprite { get; set; }

		//[ProtoMember(2)]
		//public Contract<ISound> Sound { get; set; }

		[ProtoMember(3)]
		public int Delay { get; set; }

		[ProtoMember(4)]
		public int MinDelay { get; set; }

		[ProtoMember(5)]
		public int MaxDelay { get; set; }

		public IAnimationFrame ToItem(AGSSerializationContext context)
		{
			var frame = new AGSAnimationFrame (Sprite.ToItem(context));
			//frame.SoundEmitter = Sound.ToItem(context);
			frame.Delay = Delay;
			frame.MinDelay = MinDelay;
			frame.MaxDelay = MaxDelay;
			return frame;
		}

		public void FromItem(AGSSerializationContext context, IAnimationFrame frame)
		{
			Sprite = new Contract<ISprite> ();
			Sprite.FromItem(context, frame.Sprite);
			//Sound = new Contract<ISound> ();
			//Sound.FromItem(context, frame.SoundEmitter);
			Delay = frame.Delay;
			MaxDelay = frame.MaxDelay;
			MinDelay = frame.MinDelay;
		}
	}
}

