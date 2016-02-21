using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSSerializationContext
	{
		private readonly ContractsFactory _contracts;

		public AGSSerializationContext(IGameFactory factory, IDictionary<string, GLImage> textures, 
			Resolver resolver)
		{
			Factory = factory;
			Textures = textures;
			_contracts = new ContractsFactory();
			Resolver = resolver;
		}

		public IGameFactory Factory { get; private set; }

		public IDictionary<string, GLImage> Textures { get; private set; }

		public Resolver Resolver { get; private set; }

		//Hack to work around the player character reference not saved and thus the character being cloned twice.
		public ICharacter Player { get; set; }

		public IContract<TItem> GetContract<TItem>(TItem item)
		{
			IContract<TItem> contract = _contracts.GetContract(this, item);
			return contract;
		}
	}
}

