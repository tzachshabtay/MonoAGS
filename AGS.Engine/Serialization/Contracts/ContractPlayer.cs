using Autofac;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractPlayer : IContract<IPlayer>
	{
		public ContractPlayer()
		{
		}

		[ProtoMember(1)]
		public IContract<IApproachStyle> ApproachStyle { get; set; }

		#region IContract implementation

		public IPlayer ToItem(AGSSerializationContext context)
		{
            IPlayer player = context.Resolver.Container.Resolve<IPlayer>();             
			player.Character = context.Player;
			player.ApproachStyle.CopyFrom(ApproachStyle.ToItem(context));

			return player;
		}

		public void FromItem(AGSSerializationContext context, IPlayer item)
		{
			ApproachStyle = context.GetContract(item.ApproachStyle);
		}

		#endregion
	}
}

