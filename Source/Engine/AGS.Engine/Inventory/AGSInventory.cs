using System;
using AGS.API;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSInventory : IInventory
	{
		private ConcurrentDictionary<Tuple<IInventoryItem, IInventoryItem>, IEvent<InventoryCombinationEventArgs>> _eventsMap;

		public AGSInventory()
		{
			Items = new List<IInventoryItem> (20);
			_eventsMap = new ConcurrentDictionary<Tuple<IInventoryItem, IInventoryItem>, IEvent<InventoryCombinationEventArgs>> (2, 400);
			OnDefaultCombination = new AGSEvent<InventoryCombinationEventArgs> ();
		}

		#region IInventory implementation

		public IEvent<InventoryCombinationEventArgs> OnDefaultCombination { get; private set; }

		public IEvent<InventoryCombinationEventArgs> OnCombination(IInventoryItem item1, IInventoryItem item2)
		{
			var tuple1 = new Tuple<IInventoryItem, IInventoryItem> (item1, item2);
			var tuple2 = new Tuple<IInventoryItem, IInventoryItem> (item2, item1);

			var combinationEvent = _eventsMap.GetOrAdd(tuple1, _ => new AGSInteractionEvent<InventoryCombinationEventArgs>
                                   (new List<IEvent<InventoryCombinationEventArgs>> { OnDefaultCombination }, 
                                    AGSInteractions.INTERACT, null, null));
			_eventsMap[tuple2] = combinationEvent;

			return combinationEvent;
		}

		public IInventoryItem ActiveItem { get; set; }

		public IList<IInventoryItem> Items { get; private set; }

		#endregion
	}
}

