using System;
using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractCutscene : IContract<ICutscene>
	{
		public ContractCutscene()
		{
		}

		[ProtoMember(1)]
		public bool IsSkipping { get; set; }

		[ProtoMember(2)]
		public bool IsRunning { get; set; }

		#region IContract implementation

		public ICutscene ToItem(AGSSerializationContext context)
		{
			ICutscene cutscene = context.Resolver.Container.Resolve<ICutscene>();
			if (IsRunning) cutscene.Start();

			return cutscene;
		}

		public void FromItem(AGSSerializationContext context, ICutscene item)
		{
			IsSkipping = item.IsSkipping;
			IsRunning = item.IsRunning;
		}

		#endregion
	}
}

