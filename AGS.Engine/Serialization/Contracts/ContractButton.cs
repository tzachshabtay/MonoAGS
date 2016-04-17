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
			IButton button = context.Factory.UI.GetButton(Label.Object.ID, IdleAnimation.ToItem(context), HoverAnimation.ToItem(context),
				PushedAnimation.ToItem(context), 0,0, "", null, false, Label.Width, Label.Height);
			Label.Object.ToItem(context, button);
			button.TextConfig = Label.TextConfig.ToItem(context);
			button.Text = Label.Text;
			button.Visible = Label.Object.Visible;
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

