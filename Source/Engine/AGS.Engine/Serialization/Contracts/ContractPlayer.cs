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
            AGSApproachStyle approach = new AGSApproachStyle();
            approach.CopyFrom(ApproachStyle.ToItem(context));
            AGSApproachComponent component = new AGSApproachComponent { ApproachStyle = approach };
            player.Character.AddComponent(component);

			return player;
		}

		public void FromItem(AGSSerializationContext context, IPlayer item)
		{
            if (item.Character == null) return;
            ApproachStyle = context.GetContract(item.Character.GetComponent<IApproachComponent>().ApproachStyle);
		}

		#endregion
	}
}

