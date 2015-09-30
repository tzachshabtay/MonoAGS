using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSInteractionEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly IEvent<TEventArgs> _ev;
		private readonly IEvent<TEventArgs> _defaultEvent;
		private readonly bool _isLookEvent;
		private readonly IObject _obj;
		private readonly IPlayer _player;

		public AGSInteractionEvent(IEvent<TEventArgs> defaultEvent, bool isLookEvent, IObject obj, IPlayer player)
		{
			_ev = new AGSEvent<TEventArgs>();
			_defaultEvent = defaultEvent;
			_isLookEvent = isLookEvent;
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
			if (await shouldDefault())
			{
				await _defaultEvent.WaitUntilAsync(condition);
			}
			else
			{
				await approachHotspot();
				await _ev.WaitUntilAsync(condition);
			}
		}

		public async Task InvokeAsync(object sender, TEventArgs args)
		{
			if (await shouldDefault())
			{
				await _defaultEvent.InvokeAsync(sender, args);
			}
			else
			{
				await approachHotspot();
				await _ev.InvokeAsync(sender, args);
			}
		}

		public void Invoke(object sender, TEventArgs args)
		{
			Task.Run(async () => await InvokeAsync(sender, args)).Wait();
		}

		#endregion

		private async Task<bool> shouldDefault()
		{
			if (_ev.SubscribersCount > 0 || _defaultEvent == null) return false;
			if (_player != null && _player.ApproachStyle.ApplyApproachStyleOnDefaults)
			{
				await approachHotspot();
			}
			return true;
		}

		private async Task approachHotspot()
		{
			if (_obj == null) return;
			ApproachHotspots approachStyle = getApproachStyle();
			switch (approachStyle)
			{
				case ApproachHotspots.NeverWalk:
					return;
				case ApproachHotspots.FaceOnly:
					await _player.Character.FaceDirectionAsync(_obj);
					break;
				case ApproachHotspots.WalkIfHaveWalkPoint:
					if (_obj.WalkPoint == null) await _player.Character.FaceDirectionAsync(_obj);
					else
					{
						await _player.Character.WalkAsync(new AGSLocation (_obj.WalkPoint));
						await _player.Character.FaceDirectionAsync(_obj);
					}
					break;
				case ApproachHotspots.AlwaysWalk:
					if (_obj.WalkPoint == null) await _player.Character.WalkAsync(new AGSLocation(_obj.CenterPoint));
					else await _player.Character.WalkAsync(new AGSLocation (_obj.WalkPoint));
					await _player.Character.FaceDirectionAsync(_obj);
					break;
				default:
					throw new NotSupportedException ("Approach style is not supported: " + approachStyle.ToString());
			}
		}

		private ApproachHotspots getApproachStyle()
		{
			return _isLookEvent ? _player.ApproachStyle.ApproachWhenLook : _player.ApproachStyle.ApproachWhenInteract;
		}
	}
}

