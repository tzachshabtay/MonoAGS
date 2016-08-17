using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractInventoryWindow : IContract<IObject>
	{
		static ContractInventoryWindow()
		{
			ContractsFactory.RegisterFactory(typeof(IInventoryWindow), () => new ContractInventoryWindow ());
		}

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
		public string CharacterID { get; set; }

		[ProtoMember(5)]
		public int TopItem { get; set; }

		#region IContract implementation

		public IInventoryWindow ToItem(AGSSerializationContext context)
		{
			var invWindow = context.Factory.Inventory.GetInventoryWindow(Object.ID, new EmptyImage(1f, 1f), 
				ItemWidth, ItemHeight, null);
			Object.ToItem(context, invWindow);
			context.Rewire(state => invWindow.CharacterToUse = CharacterID == null ? null :  state.Find<ICharacter>(CharacterID));
			return invWindow;
		}

		IObject IContract<IObject>.ToItem(AGSSerializationContext context)
		{
			return ToItem(context);
		}

		public void FromItem(AGSSerializationContext context, IInventoryWindow item)
		{
			Object = new ContractObject ();
			Object.FromItem(context, (IObject)item);

			CharacterID = item.CharacterToUse == null ? null : item.CharacterToUse.ID;

			ItemWidth = item.ItemSize.Width;
			ItemHeight = item.ItemSize.Height;
		}

		public void FromItem(AGSSerializationContext context, IObject item)
		{
			FromItem(context, (IInventoryWindow)item);
		}

		#endregion
	}
}

