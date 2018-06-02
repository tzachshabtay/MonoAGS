using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
    public class AGSInteractionEvent<TEventArgs> : IEvent<TEventArgs>
    {
        private readonly IEvent<TEventArgs> _ev;
        private readonly List<IEvent<TEventArgs>> _events;
        private readonly string _verb;
        private readonly IObject _obj;
        private readonly IGameState _state;

        public AGSInteractionEvent(List<IEvent<TEventArgs>> defaultEvents, string verb, IObject obj, IGameState state)
        {
            _ev = new AGSEvent<TEventArgs>();
            _events = new List<IEvent<TEventArgs>> { _ev };
            _events.AddRange(defaultEvents);
            _verb = verb;
            _obj = obj;
            _state = state;
        }

        #region IInteractionEvent implementation

        public int SubscribersCount => _ev.SubscribersCount;

        public void Subscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.Subscribe(callback, priority);
        }

        public void Unsubscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.Unsubscribe(callback, priority);
        }

        public void Subscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.Subscribe(callback, priority);
        }

        public void Unsubscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.Unsubscribe(callback, priority);
        }

        public void SubscribeToAsync(Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.SubscribeToAsync(callback, priority);
        }

        public void UnsubscribeToAsync(Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.UnsubscribeToAsync(callback, priority);
        }

        public void SubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.SubscribeToAsync(callback, priority);
        }

        public void UnsubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal)
        {
            _ev.UnsubscribeToAsync(callback, priority);
        }

        public async Task WaitUntilAsync(Predicate<TEventArgs> condition, CallbackPriority priority = CallbackPriority.Normal)
        {
            var ev = getEvent();
            if (ev == null) return;
            if (!await approachHotspot(ev)) return;
            await ev.WaitUntilAsync(condition, priority);
        }

        public async Task InvokeAsync(TEventArgs args)
        {
            var ev = getEvent();
            if (ev == null) return;
            if (!await approachHotspot(ev)) return;
            await ev.InvokeAsync(args);
        }

        public void Dispose() => _ev?.Dispose();

        #endregion

        private IEvent<TEventArgs> getEvent() => _events.FirstOrDefault(ev => ev.SubscribersCount > 0);

        private async Task<bool> approachHotspot(IEvent<TEventArgs> ev)
        {
            if (_obj == null) return true;
            var approach = _state.Player.GetComponent<IApproachComponent>();
            if (approach == null) return true;
            if (ev != _ev && !approach.ApproachStyle.ApplyApproachStyleOnDefaults)
            {
                return true;
            }
            return await approach.ApproachAsync(_verb, _obj);
        }
    }
}