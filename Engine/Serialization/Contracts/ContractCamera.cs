using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractCamera : IContract<ICamera>
	{
		public ContractCamera()
		{
		}

		[ProtoMember(1)]
		public bool Enabled { get; set; }

		#region IContract implementation

		public ICamera ToItem(AGSSerializationContext context)
		{
			var camera = new AGSCamera { Enabled = Enabled };
			context.Rewire(state => camera.Target = () => state.Player.Character);
			return camera;
		}

		public void FromItem(AGSSerializationContext context, ICamera item)
		{
			Enabled = item.Enabled;
		}

		#endregion
	}
}

