using System;
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

		public ContractPanel()
		{
		}

		[ProtoMember(1)]
		public ContractObject Object { get; set; }

		public IObject ToItem(AGSSerializationContext context)
		{
			IAnimationContainer container = Object.AnimationContainer.ToItem(context);
			var anchor = container.Anchor;
			IPanel panel = context.Factory.UI.GetPanel(Object.ID, container.Image, container.X, container.Y, false);
			Object.ToItem(context, panel);
			panel.Visible = Object.Visible;
			panel.Anchor = anchor;
			return panel;
		}

		public void FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem<IPanel>(context, (IPanel)item);
		}

		public void FromItem<TControl>(AGSSerializationContext context, IPanel<TControl> item) where TControl : IUIControl<TControl>
		{
			Object = new ContractObject ();
			Object.FromItem(context, item);
		}
	}
}

