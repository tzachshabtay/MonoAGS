using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimation : IContract<IAnimation>
	{
		public ContractAnimation()
		{
		}

		[ProtoMember(1)]
		public IList<IContract<IAnimationFrame>> Frames { get; set; }
		 
		[ProtoMember(2)]
		public IContract<IAnimationConfiguration> Configuration { get; set; }
		 
		[ProtoMember(3)]
		public IContract<IAnimationState> State { get; set; }
		 
		#region IContract implementation

		public IAnimation ToItem(AGSSerializationContext context)
		{
			AGSAnimation animation = new AGSAnimation (Configuration.ToItem(context), State.ToItem(context),
				Frames == null ? 0 : Frames.Count);
			if (Frames != null)
			{
				foreach (var frame in Frames)
				{
					animation.Frames.Add(frame.ToItem(context));
				}
			}
			return animation;
		}

		public void FromItem(AGSSerializationContext context, IAnimation item)
		{
			Frames = new List<IContract<IAnimationFrame>> (item.Frames.Count);
			foreach (var frame in item.Frames)
			{
				Frames.Add(context.GetContract(frame));
			}

			Configuration = context.GetContract(item.Configuration);
			State = context.GetContract(item.State);
		}

		#endregion
	}
}

