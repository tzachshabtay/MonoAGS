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

        public void Subscribe(Action<TEventArgs> callback)
        {
            _ev.Subscribe(callback);
        }

        public void Unsubscribe(Action<TEventArgs> callback)
        {
            _ev.Unsubscribe(callback);
        }

        public void WaitUntil(Predicate<TEventArgs> condition)
        {
            Task.Run(async () => await WaitUntilAsync(condition)).Wait();
        }

        public void SubscribeToAsync(Func<TEventArgs, Task> callback)
        {
            _ev.SubscribeToAsync(callback);
        }

        public void UnsubscribeToAsync(Func<TEventArgs, Task> callback)
        {
            _ev.UnsubscribeToAsync(callback);
        }

        public async Task WaitUntilAsync(Predicate<TEventArgs> condition)
        {
            var ev = getEvent();
            if (ev == null) return;
            if (!await approachHotspot(ev)) return;
            await ev.WaitUntilAsync(condition);
        }

        public async Task InvokeAsync(TEventArgs args)
        {
            var ev = getEvent();
            if (ev == null) return;
            if (!await approachHotspot(ev)) return;
            await ev.InvokeAsync(args);
        }

        public void Invoke(TEventArgs args)
        {
            Task.Run(async () => await InvokeAsync(args)).Wait();
        }

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

