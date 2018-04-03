using System.Collections.Concurrent;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class InventorySubscriptions
    {
        private readonly ConcurrentDictionary<(IInventoryItem, IInventoryItem), IEvent<InventoryCombinationEventArgs>> _subscriptions;
        private readonly IDefaultInteractions _defaultInteractions;

        public InventorySubscriptions(IDefaultInteractions defaultInteractions)
        {
            _defaultInteractions = defaultInteractions;
            _subscriptions = new ConcurrentDictionary<(IInventoryItem, IInventoryItem), IEvent<InventoryCombinationEventArgs>>(2, 400);
        }

        public IEvent<InventoryCombinationEventArgs> Subscribe(IInventoryItem item1, IInventoryItem item2)
        {
            var tuple1 = (item1, item2);
            var tuple2 = (item2, item1);

            var combinationEvent = _subscriptions.GetOrAdd(tuple1, _ => new AGSInteractionEvent<InventoryCombinationEventArgs>
                                   (new List<IEvent<InventoryCombinationEventArgs>> { _defaultInteractions.OnInventoryCombination },
                                    AGSInteractions.INTERACT, null, null));
            _subscriptions[tuple2] = combinationEvent;

            return combinationEvent;
        }
    }
}