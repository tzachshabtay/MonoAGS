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
			IObject obj = Object.ToItem(context);
			var anchor = obj.Anchor;
			IPanel panel = context.Factory.UI.GetPanel(obj, obj.Image, obj.X, obj.Y);
			panel.Visible = Object.Visible;
			panel.Anchor = anchor;
			panel.TreeNode.StealParent(obj.TreeNode);
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

