using System;
using ProtoBuf;
using Autofac;
using System.Diagnostics;
using System.Linq;

namespace AGS.Engine
{
	public interface IInnerContract
	{
		IContract<T> GetInnerContract<T>();
	}

	[ProtoContract(AsReferenceDefault = true)]
	public class Contract<TItem> : IContract<TItem>, IInnerContract
	{
		//Uncomment for debugging the contracts:
		//private static int runningId; 
		//private int _id;
		private TItem _item;

		public Contract()
		{
			//_id = runningId;
			//runningId++;
		}

		[ProtoMember(1)]
		public IContract<TItem> Item { get; set; }

		public IContract<T> GetInnerContract<T>()
		{
			return Item as IContract<T>;
		}

		public override string ToString()
		{
			return $"[Contract<{typeof(TItem).Name}>: Item={(Item == null ? "null" : Item.ToString())}]";
		}

		public TItem ToItem(AGSSerializationContext context)
		{
			if (_item != null) return _item;
			if (Item == null) _item = default;
			else _item = Item.ToItem(context);
			return _item;
		}

		public void FromItem(AGSSerializationContext context, TItem item)
		{
			if (item == null) return;
			try
			{
				Item = context.Resolver.Container.Resolve<IContract<TItem>>();
				Item.FromItem(context, item);
			}
			catch (Exception e)
			{
				throw new ArgumentException ("Failed to create a contract from " + item, e);
			}
		}
	}
}

