using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractPanel : IContract<IObject>
	{
		static ContractPanel()
		{
			ContractsFactory.RegisterFactory(typeof(IPanel), () => new ContractPanel ());
		}

	    [ProtoMember(1)]
		public ContractObject Object { get; set; }

		public IObject ToItem(AGSSerializationContext context)
		{
			IPanel panel = context.Factory.UI.GetPanel(Object.ID, new EmptyImage(1f,1f), 0f, 0f, null, false);
            Object.ToItem(context, panel);
			return panel;
		}

		public void FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem(context, (IPanel)item);
		}

		public void FromItem(AGSSerializationContext context, IPanel item)
		{
			Object = new ContractObject ();
			Object.FromItem(context, item);
		}
	}
}

