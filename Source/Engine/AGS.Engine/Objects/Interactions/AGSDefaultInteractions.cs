using AGS.API;

namespace AGS.Engine
{
    public class AGSDefaultInteractions : AGSInteractions, IDefaultInteractions
    {
        public AGSDefaultInteractions(IGameState state, IEvent<InventoryCombinationEventArgs> onInventoryCombination) : base(null, null, state)
        {
            OnInventoryCombination = onInventoryCombination;
        }

        public IEvent<InventoryCombinationEventArgs> OnInventoryCombination { get; private set; }
    }
}