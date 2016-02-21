using System;
using Moq;
using AGS.API;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.ResolveAnything;
using System.Drawing;
using Autofac.Core;
using AGS.Engine;
using System.Threading.Tasks;

namespace Tests
{
	public class Mocks : IDisposable
	{
		Mock<IAnimationState> _animationState;
		Mock<IAnimation> _animation;
		Mock<IGameState> _gameState;
		Mock<IPlayer> _player;
		Mock<ICharacter> _character;
		Mock<IRoom> _room;
		Mock<IObject> _obj;
		Mock<IViewport> _viewport;
		Mock<ISprite> _sprite;
		Mock<IImage> _image;
		Mock<IMaskLoader> _maskLoader;
		Mock<IRoomEvents> _roomEvents;
		Mock<ICutscene> _cutscene;
		Mock<IInput> _input;

		IContainer container;

		public static Mocks Init()
		{
			ContainerBuilder builder = new ContainerBuilder ();
			Mocks mocks = new Mocks ();
			builder.RegisterInstance(mocks.Animation().Object);
			builder.RegisterInstance(mocks.AnimationState().Object);
			builder.RegisterInstance(mocks.GameState().Object);
			builder.RegisterInstance(mocks.Player().Object);
			builder.RegisterInstance(mocks.Character().Object);
			builder.RegisterInstance(mocks.Room().Object);
			builder.RegisterInstance(mocks.Object().Object);
			builder.RegisterInstance(mocks.Viewport().Object);
			builder.RegisterInstance(mocks.Sprite().Object);
			builder.RegisterInstance(mocks.Image().Object);
			builder.RegisterInstance(mocks.Input().Object);
			builder.RegisterInstance(mocks.Cutscene().Object);

			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

			mocks.container = builder.Build();

			return mocks;
		}

		public TItem Create<TItem>(params Parameter[] parameters)
		{
			return container.Resolve<TItem>(parameters);
		}

		public void Dispose()
		{
			container.Dispose();
		}

		public Mock<IAnimationState> AnimationState(bool newInstance = false)
		{
			if (_animationState == null || newInstance)
			{
				_animationState = new Mock<IAnimationState> ();
				_animationState.Setup(a => a.OnAnimationCompleted).Returns(new TaskCompletionSource<AnimationCompletedEventArgs> ());
			}
			return _animationState;
		}

		public Mock<IAnimation> Animation(bool newInstance = false)
		{
			if (_animation == null || newInstance)
			{
				_animation = new Mock<IAnimation> ();
				_animation.Setup(m => m.State).Returns(AnimationState().Object);
				_animation.Setup(m => m.Sprite).Returns(Sprite().Object);
				_animation.Setup(m => m.Frames).Returns(new List<IAnimationFrame>());
			}
			return _animation;
		}

		public Mock<IGameState> GameState()
		{
			if (_gameState == null)
			{
				_gameState = new Mock<IGameState> ();
				_gameState.Setup(m => m.Player).Returns(Player().Object);
				_gameState.Setup(m => m.Cutscene).Returns(Cutscene().Object);
			}
			return _gameState;
		}

		public Mock<IPlayer> Player()
		{
			if (_player == null)
			{
				_player = new Mock<IPlayer> ();
				_player.Setup(m => m.Character).Returns(Character().Object);
			}
			return _player;
		}

		public Mock<ICharacter> Character()
		{
			if (_character == null)
			{
				_character = new Mock<ICharacter> ();
				_character.Setup(m => m.Room).Returns(Room().Object);
			}
			return _character;
		}

		public Mock<IRoom> Room(bool newInstance = false)
		{			
			if (_room == null || newInstance)
			{
				List<IArea> walkables = new List<IArea> ();
				List<IWalkBehindArea> walkBehinds = new List<IWalkBehindArea> ();
				List<IScalingArea> scalingAreas = new List<IScalingArea> ();
				AGSConcurrentHashSet<IObject> roomObjects = new AGSConcurrentHashSet<IObject> ();
				_room = new Mock<IRoom> ();
				_room.Setup(m => m.Background).Returns(Object().Object);
				_room.Setup(m => m.Viewport).Returns(Viewport().Object);
				_room.Setup(m => m.Objects).Returns(roomObjects);
				_room.Setup(m => m.ShowPlayer).Returns(true);
				_room.Setup(m => m.Events).Returns(RoomEvents().Object);
				_room.Setup(m => m.WalkableAreas).Returns(walkables);
				_room.Setup(m => m.WalkBehindAreas).Returns(walkBehinds);
				_room.Setup(m => m.ScalingAreas).Returns(scalingAreas);
			}
			return _room;
		}

		public Mock<IRoomEvents> RoomEvents()
		{
			if (_roomEvents == null)
			{
				_roomEvents = new Mock<IRoomEvents> ();
				_roomEvents.Setup(r => r.OnAfterFadeIn).Returns(new Mock<IEvent<AGSEventArgs>> ().Object);
				_roomEvents.Setup(r => r.OnAfterFadeOut).Returns(new Mock<IEvent<AGSEventArgs>> ().Object);
				_roomEvents.Setup(r => r.OnBeforeFadeIn).Returns(new Mock<IEvent<AGSEventArgs>> ().Object);
				_roomEvents.Setup(r => r.OnBeforeFadeOut).Returns(new Mock<IEvent<AGSEventArgs>> ().Object);
			}
			return _roomEvents;
		}

		public Mock<IObject> Object()
		{
			if (_obj == null)
			{
				_obj = new Mock<IObject> ();
				_obj.Setup(m => m.Animation).Returns(Animation().Object);
				_obj.Setup(m => m.Image).Returns(Image().Object);
				_obj.Setup(m => m.Enabled).Returns(true);
				_obj.Setup(m => m.Visible).Returns(true);
				_obj.Setup(m => m.Anchor).Returns(new AGSPoint ());
			}
			return _obj;
		}

		public Mock<IViewport> Viewport()
		{
			if (_viewport == null)
			{
				_viewport = new Mock<IViewport> ();
			}
			return _viewport;
		}

		public Mock<ISprite> Sprite(bool newInstance = false)
		{
			if (_sprite == null || newInstance)
			{
				_sprite = new Mock<ISprite> ();
				_sprite.Setup(m => m.Image).Returns(Image().Object);
				_sprite.Setup(m => m.Anchor).Returns(new AGSPoint ());
			}
			return _sprite;
		}

		public Mock<IImage> Image()
		{
			if (_image == null)
			{
				_image = new Mock<IImage> ();
				_image.Setup(m => m.Width).Returns(100f);
				_image.Setup(m => m.Height).Returns(50f);
			}
			return _image;
		}

		public Mock<IMaskLoader> MaskLoader()
		{
			if (_maskLoader == null)
			{
				_maskLoader = new Mock<IMaskLoader> ();
			}
			return _maskLoader;
		}

		public Mock<ICutscene> Cutscene()
		{
			if (_cutscene == null)
			{
				_cutscene = new Mock<ICutscene> ();
			}
			return _cutscene;
		}

		public Mock<IInput> Input()
		{
			if (_input == null)
			{
				_input = new Mock<IInput> ();
			}
			return _input;
		}
	}
}

