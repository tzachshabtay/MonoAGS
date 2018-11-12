using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractInventory : IContract<IInventory>
	{
	    [ProtoMember(1)]
		int ActiveItemIndex { get; set; }

		[ProtoMember(2)]
		IList<Contract<IInventoryItem>> Items { get; set; }

		#region IContract implementation

		public IInventory ToItem(AGSSerializationContext context)
		{
			AGSInventory inventory = new AGSInventory ();
			if (Items != null)
			{
				foreach (var item in Items)
				{
					inventory.Items.Add(item.ToItem(context));
				}
				if (ActiveItemIndex >= 0 && ActiveItemIndex < Items.Count)
				{
					inventory.ActiveItem = inventory.Items[ActiveItemIndex];
				}
			}

			return inventory;
		}

		public void FromItem(AGSSerializationContext context, IInventory item)
		{
			if (item.ActiveItem == null) ActiveItemIndex = -1;
			else ActiveItemIndex = item.Items.IndexOf(item.ActiveItem);

			Items = new List<Contract<IInventoryItem>> (item.Items.Count);
			foreach (var invItem in item.Items)
			{
				Contract<IInventoryItem> contract = new Contract<IInventoryItem> ();
				contract.FromItem(context, invItem);
				Items.Add(contract);
			}
		}

		#endregion
	}
}

