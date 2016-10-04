using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSSerializationContext
	{
		private readonly ContractsFactory _contracts;
		private readonly List<Action<IGameState>> _rewireActions;

        public AGSSerializationContext(IGameFactory factory, IDictionary<string, ITexture> textures, 
			Resolver resolver)
		{
			Factory = factory;
			Textures = textures;
			_contracts = new ContractsFactory();
			Resolver = resolver;
			_rewireActions = new List<Action<IGameState>> ();
		}

		public IGameFactory Factory { get; private set; }

        public IDictionary<string, ITexture> Textures { get; private set; }

		public Resolver Resolver { get; private set; }

		//Hack to work around the player character reference not saved and thus the character being cloned twice.
		public ICharacter Player { get; set; }

		public void Rewire(Action<IGameState> onRewire)
		{
			_rewireActions.Add(onRewire);
		}

		public void Rewire(IGameState state)
		{
			foreach (var action in _rewireActions) action(state);
		}

		public IContract<TItem> GetContract<TItem>(TItem item)
		{
			IContract<TItem> contract = _contracts.GetContract(this, item);
			return contract;
		}
	}
}

