using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
	public class AGSInteractions : IInteractions
	{
        private ConcurrentDictionary<string, IEvent<ObjectEventArgs>> _events;
        private ConcurrentDictionary<string, IEvent<InventoryInteractEventArgs>> _inventoryEvents;

        private Func<string, IEvent<ObjectEventArgs>> _factory;
        private Func<string, IEvent<InventoryInteractEventArgs>> _inventoryFactory;

        public AGSInteractions (IInteractions defaultInteractions, IObject obj, IGameState state)
		{
            _events = new ConcurrentDictionary<string, IEvent<ObjectEventArgs>>();
            _inventoryEvents = new ConcurrentDictionary<string, IEvent<InventoryInteractEventArgs>>();
            var defaultInteractionEvent = new AGSInteractionEvent<ObjectEventArgs>(
                new List<IEvent<ObjectEventArgs>>(), DEFAULT, obj, state);
            var defaultInventoryEvent = new AGSInteractionEvent<InventoryInteractEventArgs>(
                new List<IEvent<InventoryInteractEventArgs>>(), DEFAULT, obj, state);
            _events.TryAdd(DEFAULT, defaultInteractionEvent);
            _inventoryEvents.TryAdd(DEFAULT, defaultInventoryEvent);

            _factory = verb =>
            {
                List<IEvent<ObjectEventArgs>> interactions = new List<IEvent<ObjectEventArgs>> { defaultInteractionEvent };
                if (defaultInteractions != null)
                {
                    interactions.Add(defaultInteractions.OnInteract(verb));
                    if (verb != DEFAULT) interactions.Add(defaultInteractions.OnInteract(DEFAULT));
                }
                return new AGSInteractionEvent<ObjectEventArgs>(interactions, verb, obj, state);
            };
            _inventoryFactory = verb =>
            {
                List<IEvent<InventoryInteractEventArgs>> interactions = new List<IEvent<InventoryInteractEventArgs>> { defaultInventoryEvent };
                if (defaultInteractions != null)
                {
                    interactions.Add(defaultInteractions.OnInventoryInteract(verb));
                    if (verb != DEFAULT) interactions.Add(defaultInteractions.OnInventoryInteract(DEFAULT));
                }
                return new AGSInteractionEvent<InventoryInteractEventArgs>(interactions, verb, obj, state);
            };
		}

        public const string LOOK = "Look";
        public const string INTERACT = "Interact";
        public const string DEFAULT = "Default";

        #region IInteractions implementation

        public IEvent<ObjectEventArgs> OnInteract(string verb)
        {
            return _events.GetOrAdd(verb, _factory);
        }

        public IEvent<InventoryInteractEventArgs> OnInventoryInteract(string verb)
        {
            return _inventoryEvents.GetOrAdd(verb, _inventoryFactory);
        }

		#endregion
	}
}

