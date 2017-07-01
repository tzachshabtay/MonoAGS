using System;
using AGS.API;
using System.Collections.Concurrent;

namespace AGS.Engine
{
	public class RoomMusicCrossFading : ICrossFading
	{
		private ConcurrentDictionary<string, RoomCrossFader> _map;
		private IGame _game;
		private IGameState _state;

		public RoomMusicCrossFading(IGame game)
		{
			//Default: Linear 3 seconds fade out with no fade in (most music clips already have a 'fade-in' at the start)
			FadeOut = true;
			FadeOutSeconds = 3f;

			_map = new ConcurrentDictionary<string, RoomCrossFader> ();
			_game = game;
			_state = game.State;
			game.State.Rooms.OnListChanged.Subscribe(onRoomsChange);
			game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoad);
		}

		#region ICrossFading implementation

		public bool FadeOut { get; set; }

		public bool FadeIn { get; set; }

		public float FadeOutSeconds { get; set; }

		public float FadeInSeconds { get; set; }

		public Func<float, float> EaseFadeOut { get; set; }

		public Func<float, float> EaseFadeIn { get; set; }

		#endregion

		private void onRoomsChange(object sender, AGSListChangedEventArgs<IRoom> args)
		{
            foreach (var item in args.Items)
            {
                var fader = _map.GetOrAdd(item.Item.ID, _ => new RoomCrossFader(this, item.Item));
                if (args.ChangeType == ListChangeType.Remove)
                {
                    RoomCrossFader val;
                    _map.TryRemove(item.Item.ID, out val);
                    fader.Dispose();
                }
            }
		}

		private void onSavedGameLoad(object sender, AGSEventArgs args)
		{
			_state.Rooms.OnListChanged.Unsubscribe(onRoomsChange);
			foreach (var fader in _map.Values)
			{
				fader.Dispose();
			}
			foreach (var room in _state.Rooms)
			{
				_map.TryAdd(room.ID, new RoomCrossFader (this, room));
			}
			_state = _game.State;
			_state.Rooms.OnListChanged.Subscribe(onRoomsChange);
		}

		private class RoomCrossFader
		{
			private RoomMusicCrossFading _crossFading;
			private ISound _music;

			public RoomCrossFader(RoomMusicCrossFading crossFading, IRoom room)
			{
				_crossFading = crossFading;
				Room = room;

				room.Events.OnBeforeFadeOut.Subscribe(onBeforeFadeOut);
				room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);
			}

			public IRoom Room { get; private set; }

			public void Dispose()
			{
				var room = Room;
				if (room == null) return;
				var music = _music;
				if (music != null) music.Stop();
				room.Events.OnBeforeFadeOut.Unsubscribe(onBeforeFadeOut);
				room.Events.OnBeforeFadeIn.Unsubscribe(onBeforeFadeIn);
			}

			private void onBeforeFadeOut(object sender, AGSEventArgs args)
			{
				var music = _music;
				if (music == null || music.HasCompleted) return;
				if (!_crossFading.FadeOut)
				{
					music.Stop();
					return;
				}
				music.TweenVolume(0f, _crossFading.FadeOutSeconds, _crossFading.EaseFadeOut).Task.ContinueWith(_ =>
				{
					music.Stop();
				});
			}

			private void onBeforeFadeIn(object sender, AGSEventArgs args)
			{
				var room = Room;
				if (room == null) return;
				var clip = room.MusicOnLoad;
				if (clip == null) return;
				if (!_crossFading.FadeIn)
				{
					_music = clip.Play(shouldLoop: true);
					return;
				}
				float endVolume = clip.Volume;
				var music = clip.Play(0f, shouldLoop: true);
				_music = music;
				music.TweenVolume(endVolume, _crossFading.FadeInSeconds, _crossFading.EaseFadeIn);
			}
		}
	}
}

