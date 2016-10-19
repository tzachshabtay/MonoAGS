using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace AGS.Engine
{
	public class AGSInteractionEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly IEvent<TEventArgs> _ev;
		private readonly List<IEvent<TEventArgs>> _events;
		private readonly string _verb;
		private readonly IObject _obj;
		private readonly IPlayer _player;

		public AGSInteractionEvent(List<IEvent<TEventArgs>> defaultEvents, string verb, IObject obj, IPlayer player)
		{
			_ev = new AGSEvent<TEventArgs>();
            _events = new List<IEvent<TEventArgs>> { _ev };
            _events.AddRange(defaultEvents);
            _verb = verb;
			_obj = obj;
			_player = player;
		}

		#region IInteractionEvent implementation

		public int SubscribersCount { get { return _ev.SubscribersCount; } }

		public void Subscribe(Action<object, TEventArgs> callback)
		{
			_ev.Subscribe(callback);
		}

		public void Unsubscribe(Action<object, TEventArgs> callback)
		{
			_ev.Unsubscribe(callback);
		}

		public void WaitUntil(Predicate<TEventArgs> condition)
		{
			Task.Run(async () => await WaitUntilAsync(condition)).Wait();
		}

		public void SubscribeToAsync(Func<object, TEventArgs, Task> callback)
		{
			_ev.SubscribeToAsync(callback);
		}

		public void UnsubscribeToAsync(Func<object, TEventArgs, Task> callback)
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

		public async Task InvokeAsync(object sender, TEventArgs args)
		{
            var ev = getEvent();
            if (ev == null) return;
            if (!await approachHotspot(ev)) return;
            await ev.InvokeAsync(sender, args);
		}

		public void Invoke(object sender, TEventArgs args)
		{
			Task.Run(async () => await InvokeAsync(sender, args)).Wait();
		}

        #endregion

        private IEvent<TEventArgs> getEvent()
        {
            return _events.FirstOrDefault(ev => ev.SubscribersCount > 0);
        }

        private async Task<bool> approachHotspot(IEvent<TEventArgs> ev)
		{
            if (ev != _ev && (_player == null || !_player.ApproachStyle.ApplyApproachStyleOnDefaults))
            {
                return true;
            }
			if (_obj == null) return true;
			ApproachHotspots approachStyle = getApproachStyle();
			switch (approachStyle)
			{
				case ApproachHotspots.NeverWalk:
                    break;
				case ApproachHotspots.FaceOnly:
					await _player.Character.FaceDirectionAsync(_obj);
					break;
				case ApproachHotspots.WalkIfHaveWalkPoint:
					if (_obj.WalkPoint == null) await _player.Character.FaceDirectionAsync(_obj);
					else
					{
                        if (!await _player.Character.WalkAsync(new AGSLocation(_obj.WalkPoint.Value))) return false;
						await _player.Character.FaceDirectionAsync(_obj);
					}
					break;
				case ApproachHotspots.AlwaysWalk:
					PointF? walkPoint = _obj.WalkPoint ?? _obj.CenterPoint ?? _obj.Location.XY;
                    if (!await _player.Character.WalkAsync(new AGSLocation(walkPoint.Value))) return false;
					await _player.Character.FaceDirectionAsync(_obj);
					break;
				default:
					throw new NotSupportedException ("Approach style is not supported: " + approachStyle.ToString());
			}
            return true;
		}

		private ApproachHotspots getApproachStyle()
		{
            return ((ConcurrentDictionary<string,ApproachHotspots>)_player.ApproachStyle.ApproachWhenVerb).GetOrAdd(_verb, _ => ApproachHotspots.NeverWalk);
		}
	}
}

