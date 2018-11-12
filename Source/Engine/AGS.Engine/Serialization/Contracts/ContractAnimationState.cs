using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractAnimationState : IContract<IAnimationState>
	{
	    [ProtoMember(1)]
		public bool RunningBackwards { get; set; }

		[ProtoMember(2)]
		public int CurrentFrame { get; set; }

		[ProtoMember(3)]
		public int CurrentLoop { get; set; }

		[ProtoMember(4)]
		public int TimeToNextFrame { get; set; }

		[ProtoMember(5)]
		public bool IsPaused { get; set; }

		#region IContract implementation

		public IAnimationState ToItem(AGSSerializationContext context)
		{
			AGSAnimationState state = new AGSAnimationState ();
			state.RunningBackwards = RunningBackwards;
			state.CurrentFrame = CurrentFrame;
			state.CurrentLoop = CurrentLoop;
			state.TimeToNextFrame = TimeToNextFrame;
			state.IsPaused = IsPaused;

			return state;
		}

		public void FromItem(AGSSerializationContext context, IAnimationState item)
		{
			RunningBackwards = item.RunningBackwards;
			CurrentFrame = item.CurrentFrame;
			CurrentLoop = item.CurrentLoop;
			TimeToNextFrame = item.TimeToNextFrame;
			IsPaused = item.IsPaused;
		}

		#endregion
	}
}

