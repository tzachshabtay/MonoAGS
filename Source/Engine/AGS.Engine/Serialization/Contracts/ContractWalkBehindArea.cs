using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractWalkBehindArea : IContract<IWalkBehindArea>
	{
	    [ProtoMember(1)]
		public float? BaseLine { get; set; }

		#region IContract implementation

		public IWalkBehindArea ToItem(AGSSerializationContext context)
		{
            IWalkBehindArea area = context.Resolver.Container.Resolve<IWalkBehindArea>();
			area.Baseline = BaseLine;
			return area;
		}

		public void FromItem(AGSSerializationContext context, IWalkBehindArea item)
		{
			BaseLine = item.Baseline;
		}

		#endregion
	}
}

