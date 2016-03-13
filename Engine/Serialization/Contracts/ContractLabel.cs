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
		public IContract<ITextConfig> TextConfig { get; set; }

		[ProtoMember(3)]
		public string Text { get; set; }

		[ProtoMember(4)]
		public float Width { get; set; }

		[ProtoMember(5)]
		public float Height { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			if (Object == null) return null;
			IObject obj = Object.ToItem(context);
			var anchor = obj.Anchor;
			var tint = obj.Tint;
			IPanel panel = context.Factory.UI.GetPanel(obj, new EmptyImage (obj.Width/obj.ScaleX, obj.Height/obj.ScaleY), 
				obj.X, obj.Y);
			ILabel label = context.Factory.UI.GetLabel(panel, Text, Width, Height, obj.X, obj.Y, 
				TextConfig.ToItem(context));
			label.Visible = Object.Visible;
			label.Anchor = anchor;
			label.Tint = tint;
			label.TreeNode.StealParent(obj.TreeNode);
			return label;
		}
			
		public void FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem<ILabel>(context, (ILabel)item);
		}

		public void FromItem<TControl>(AGSSerializationContext context, ILabel<TControl> item) where TControl : IUIControl<TControl>
		{
			Object = new ContractObject ();
			Object.FromItem(context, item);

			TextConfig = context.GetContract(item.TextConfig);
			Text = item.Text;
			Width = item.Width;
			Height = item.Height;
		}

		#endregion
	}
}

