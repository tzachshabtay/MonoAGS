using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSCloner<TItem>
    {
        private readonly IContract<TItem> _contract;
        private readonly AGSSerializationContext _context;
        private readonly IGameState _state;

        public AGSCloner(TItem item, IContract<TItem> contract, Resolver resolver, IGameFactory factory,
                         IDictionary<string, ITexture> textures, IGLUtils glUtils, IGameState state)
        {
            _contract = contract;
            _state = state;
            _context = new AGSSerializationContext(factory, textures, resolver, glUtils);
            contract.FromItem(_context, item);
        }

        public TItem Clone()
        {
            TItem item = _contract.ToItem(_context);
            _context.Rewire(_state);
            return item;
        }
    }
}
