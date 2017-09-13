using System;
using ProtoBuf;
using Autofac;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
			return string.Format("[Contract<{0}>: Item={1}]", typeof(TItem).Name, Item == null ? 
				"null" : Item.ToString());
		}

		public TItem ToItem(AGSSerializationContext context)
		{
            if (_item != null)
            {
                if (!typeof(TItem).GetTypeInfo().IsValueType) return _item;
                if (!_item.Equals(default(TItem))) return _item;
            }
			if (Item == null) _item = default(TItem);
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

