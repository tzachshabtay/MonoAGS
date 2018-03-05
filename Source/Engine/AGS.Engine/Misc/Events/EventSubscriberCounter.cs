using System;
namespace AGS.Engine
{
    public class EventSubscriberCounter
    {
        private const int MAX_SUBSCRIPTIONS = 10000;

        public int Count { get; private set; }

        public void Add()
        {
            Count++;
            if (Count > MAX_SUBSCRIPTIONS)
            {
                throw new OverflowException("Event Subscription Overflow");
            }
        }

        public void Remove()
        {
            Count--;
        }
    }
}
