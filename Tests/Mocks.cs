using System;
using Moq;
using API;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.ResolveAnything;
using System.Drawing;
using Autofac.Core;

namespace Tests
{
	public class Mocks : IDisposable
	{
		Mock<IAnimationState> animationState;
		Mock<IAnimation> animation;
		Mock<IGameState> gameState;
		Mock<IPlayer> player;
		Mock<ICharacter> character;
		Mock<IRoom> room;
		Mock<IObject> obj;
		Mock<IViewport> viewport;
		Mock<ISprite> sprite;
		Mock<IImage> image;

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

		public Mock<IAnimationState> AnimationState()
		{
			if (animationState == null)
			{
				animationState = new Mock<IAnimationState> ();
			}
			return animationState;
		}

		public Mock<IAnimation> Animation()
		{
			if (animation == null)
			{
				animation = new Mock<IAnimation> ();
				animation.Setup(m => m.State).Returns(AnimationState().Object);
				animation.Setup(m => m.Sprite).Returns(Sprite().Object);
			}
			return animation;
		}

		public Mock<IGameState> GameState()
		{
			if (gameState == null)
			{
				gameState = new Mock<IGameState> ();
				gameState.Setup(m => m.Player).Returns(Player().Object);
			}
			return gameState;
		}

		public Mock<IPlayer> Player()
		{
			if (player == null)
			{
				player = new Mock<IPlayer> ();
				player.Setup(m => m.Character).Returns(Character().Object);
			}
			return player;
		}

		public Mock<ICharacter> Character()
		{
			if (character == null)
			{
				character = new Mock<ICharacter> ();
				character.Setup(m => m.Room).Returns(Room().Object);
			}
			return character;
		}

		public Mock<IRoom> Room()
		{
			if (room == null)
			{
				room = new Mock<IRoom> ();
				room.Setup(m => m.Background).Returns(Object().Object);
				room.Setup(m => m.Viewport).Returns(Viewport().Object);
				room.Setup(m => m.Objects).Returns(new List<IObject> ());
				room.Setup(m => m.ShowPlayer).Returns(true);
			}
			return room;
		}

		public Mock<IObject> Object()
		{
			if (obj == null)
			{
				obj = new Mock<IObject> ();
				obj.Setup(m => m.Animation).Returns(Animation().Object);
				obj.Setup(m => m.Image).Returns(Image().Object);
				obj.Setup(m => m.Enabled).Returns(true);
				obj.Setup(m => m.Visible).Returns(true);
			}
			return obj;
		}

		public Mock<IViewport> Viewport()
		{
			if (viewport == null)
			{
				viewport = new Mock<IViewport> ();
			}
			return viewport;
		}

		public Mock<ISprite> Sprite()
		{
			if (sprite == null)
			{
				sprite = new Mock<ISprite> ();
				sprite.Setup(m => m.Image).Returns(Image().Object);
			}
			return sprite;
		}

		public Mock<IImage> Image()
		{
			if (image == null)
			{
				image = new Mock<IImage> ();
			}
			return image;
		}
	}
}

