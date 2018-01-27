using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using System.Reflection;

namespace AGS.Engine
{
	public class ContractsFactory
	{
		private readonly Dictionary<object, object> _contracts;
		private readonly TypeComparer _typeComparer;
		private static readonly Dictionary<Type, Func<object>> _factories;

		public static int RunningID = 100;

		static ContractsFactory()
		{
			_factories = new Dictionary<Type, Func<object>> ();
		}

		public ContractsFactory()
		{
			_contracts = new Dictionary<object, object> (500);
			_typeComparer = new TypeComparer ();
		}

		public IContract<TItem> GetContract<TItem>(AGSSerializationContext context, TItem item)
		{
			if (item == null)
			{
				return new Contract<TItem> ();
			}

			var contract = _contracts.GetOrAdd(item, () => 
			{
				Type itemType = item.GetType();
				var result = getContract(context, item, itemType);
				if (result != null) return result;
				var interfaces = itemType.GetInterfaces().ToList();
				interfaces.Sort(_typeComparer);

				foreach (Type type in interfaces)
				{
					result = getContract(context, item, type);
					if (result != null) return result;
				}

				var defaultContract = new Contract<TItem>();
				defaultContract.FromItem(context, item);
				return defaultContract;
			});
			try
			{
				return castContract<TItem>(contract);
			}
			catch (InvalidCastException e)
			{
				throw new InvalidCastException("Cannot create contract from " + item.GetType().Name, e);
			}
		}

		public static void RegisterFactory(Type typeForContract, Func<object> factory)
		{
			_factories[typeForContract] = factory;
		}

		public static void RegisterSubtype(Type type, Type subType)
		{
			RuntimeTypeModel.Default.Add(type, true).AddSubType(RunningID++, subType);
		}

		private IContract<TItem> castContract<TItem>(object obj)
		{
			IContract<TItem> contract = obj as IContract<TItem>;
			if (contract != null) return contract;
			IInnerContract inner = (IInnerContract)obj;
			return inner.GetInnerContract<TItem>();
		}

		private IContract<TItem> getContract<TItem>(AGSSerializationContext context, TItem item, Type type)
		{
			if (!_factories.TryGetValue(type, out var factory)) return null;
			return createContract(context, item, factory);
		}

		private IContract<TItem> createContract<TItem>(AGSSerializationContext context, TItem item, Func<object> factory)
		{
			IContract<TItem> insideContract = factory() as IContract<TItem>;
			if (insideContract == null) return null;
			insideContract.FromItem(context, item);
			return new Contract<TItem> { Item = insideContract };
		}

		private class TypeComparer : IComparer<Type>
		{
			#region IComparer implementation
			public int Compare(Type x, Type y)
			{
				if (x.GetTypeInfo().IsAssignableFrom(y.GetTypeInfo()))
					return 1;
                else if (y.GetTypeInfo().IsAssignableFrom(x.GetTypeInfo()))
				    return -1;
                return 0;
			}
			#endregion

		}
	}
}

