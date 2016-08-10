using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractLabel : IContract<IObject>
	{
		static ContractLabel()
		{
			ContractsFactory.RegisterFactory(typeof(ILabel), () => new ContractLabel ());
		}

		public ContractLabel()
		{
		}

		[ProtoMember(1)]
		public ContractObject Object { get; set; }

		[ProtoMember(2)]
        public IContract<ITextComponent> TextComponent { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			if (Object == null) return null;
            ITextComponent textComponent = TextComponent.ToItem(context);
			ILabel label = context.Factory.UI.GetLabel(Object.ID, textComponent.Text, 
                textComponent.LabelRenderSize.Width, textComponent.LabelRenderSize.Height, 0, 0, 
				textComponent.TextConfig, false);
			Object.ToItem(context, label);
			label.Visible = Object.Visible;
			return label;
		}
			
		public void FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem(context, (ILabel)item);
		}

		public void FromItem(AGSSerializationContext context, ILabel item)
		{
			Object = new ContractObject ();
			Object.FromItem(context, (IObject)item);

            TextComponent = context.GetContract(item as ITextComponent);            
		}

		#endregion
	}
}

