using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractInventoryWindow : IContract<IInventoryWindow>
	{
		public ContractInventoryWindow()
		{
		}

		[ProtoMember(1)]
		public ContractObject Object { get; set; }

		[ProtoMember(2)]
		public float ItemWidth { get; set; }

		[ProtoMember(3)]
		public float ItemHeight { get; set; }

		[ProtoMember(4)]
		public Contract<ICharacter> CharacterToUse { get; set; }

		[ProtoMember(5)]
		public int TopItem { get; set; }

		#region IContract implementation

		public IInventoryWindow ToItem(AGSSerializationContext context)
		{
			IObject obj = Object.ToItem(context);
			IPanel panel = context.Factory.UI.GetPanel(obj, new EmptyImage (obj.Width, obj.Height), obj.X, obj.Y);
			var invWindow = context.Factory.Inventory.GetInventoryWindow(panel, ItemWidth, ItemHeight,
				CharacterToUse.ToItem(context));
			invWindow.Visible = Object.AnimationContainer.Visible;
			invWindow.TreeNode.StealParent(obj.TreeNode);
			return invWindow;
		}

		public void FromItem(AGSSerializationContext context, IInventoryWindow item)
		{
			Object = new ContractObject ();
			Object.FromItem(context, item);

			CharacterToUse = new Contract<ICharacter> ();
			CharacterToUse.FromItem(context, item.CharacterToUse);

			ItemWidth = item.ItemSize.Width;
			ItemHeight = item.ItemSize.Height;
		}

		#endregion
	}
}

