using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractButton : IContract<IObject>
	{
		static ContractButton()
		{
			ContractsFactory.RegisterFactory(typeof(IButton), () => new ContractButton ());
		}

		public ContractButton()
		{
		}

		[ProtoMember(1)]
		public ContractLabel Label { get; set; }

		[ProtoMember(2)]
		public IContract<IAnimation> IdleAnimation { get; set; }

		[ProtoMember(3)]
		public IContract<IAnimation> HoverAnimation { get; set; }

		[ProtoMember(4)]
		public IContract<IAnimation> PushedAnimation { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			ILabel label = (ILabel)Label.ToItem(context);
			var anchor = label.Anchor;
			IButton button = context.Factory.UI.GetButton(label, IdleAnimation.ToItem(context),
				                 HoverAnimation.ToItem(context), PushedAnimation.ToItem(context));
			button.Visible = label.UnderlyingVisible;
			button.Anchor = anchor;
			button.TreeNode.StealParent(label.TreeNode);
			return button;
		}

		public void FromItem(AGSSerializationContext context, IObject item)
		{
			IButton button = (IButton)item;

			Label = new ContractLabel ();
			Label.FromItem(context, button);

			IdleAnimation = context.GetContract(button.IdleAnimation);
			HoverAnimation = context.GetContract(button.HoverAnimation);
			PushedAnimation = context.GetContract(button.PushedAnimation);
		}

		#endregion
	}
}

