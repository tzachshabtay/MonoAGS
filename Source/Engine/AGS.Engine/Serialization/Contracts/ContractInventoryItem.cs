using System;
using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractInventoryItem : IContract<IInventoryItem>
	{
	    [ProtoMember(1, AsReference = true)]
		public IContract<IObject> Graphics { get; set; }

		[ProtoMember(2, AsReference = true)]
		public IContract<IObject> CursorGraphics { get; set; }

		[ProtoMember(3)]
		public float Qty { get; set; }

		[ProtoMember(4)]
		public bool ShouldInteract { get; set; }

		#region IContract implementation

		public IInventoryItem ToItem(AGSSerializationContext context)
		{
            IInventoryItem item = context.Resolver.Container.Resolve<IInventoryItem>();
			item.Graphics = Graphics.ToItem(context);
			item.CursorGraphics = CursorGraphics.ToItem(context);
			item.Qty = Qty;
			item.ShouldInteract = ShouldInteract;

			return item;
		}

		public void FromItem(AGSSerializationContext context, IInventoryItem item)
		{
			Graphics = context.GetContract(item.Graphics);
			CursorGraphics = context.GetContract(item.CursorGraphics);
			Qty = item.Qty;
			ShouldInteract = item.ShouldInteract;
		}

		#endregion
	}
}

