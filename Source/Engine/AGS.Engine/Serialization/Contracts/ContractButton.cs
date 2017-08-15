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
		public ContractObject Object { get; set; }

		[ProtoMember(2)]
        public IContract<ButtonAnimation> IdleAnimation { get; set; }

		[ProtoMember(3)]
		public IContract<ButtonAnimation> HoverAnimation { get; set; }

		[ProtoMember(4)]
		public IContract<ButtonAnimation> PushedAnimation { get; set; }

        [ProtoMember(5)]
        public ContractTextComponent TextComponent { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
            var textComponent = TextComponent.ToItem(context);
			IButton button = context.Factory.UI.GetButton(Object.ID, IdleAnimation.ToItem(context), HoverAnimation.ToItem(context),
				PushedAnimation.ToItem(context), 0,0, null, "", null, false, textComponent.LabelRenderSize.Width, textComponent.LabelRenderSize.Height);
			Object.ToItem(context, button);
            button.TextConfig = textComponent.TextConfig;
			button.Text = textComponent.Text;
			button.Visible = Object.Visible;
			return button;
		}

		public void FromItem(AGSSerializationContext context, IObject item)
		{
			IButton button = (IButton)item;

			Object = new ContractObject ();
			Object.FromItem(context, button);

            TextComponent = new ContractTextComponent();
            TextComponent.FromItem(context, button);

			IdleAnimation = context.GetContract(button.IdleAnimation);
			HoverAnimation = context.GetContract(button.HoverAnimation);
			PushedAnimation = context.GetContract(button.PushedAnimation);
		}

		#endregion
	}
}

