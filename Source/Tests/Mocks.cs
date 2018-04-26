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
        Mock<ICharacter> _player;
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
		Mock<IALAudioSystem> _audioSystem;
        Mock<IGame> _game;
        Mock<IRuntimeSettings> _settings;

		IContainer container;

		public static Mocks Init()
		{
			ContainerBuilder builder = new ContainerBuilder ();
			Mocks mocks = new Mocks ();
			builder.RegisterInstance(mocks.Animation().Object);
			builder.RegisterInstance(mocks.AnimationState().Object);
			builder.RegisterInstance(mocks.GameState().Object);
            builder.RegisterInstance(mocks.Game().Object);
			builder.RegisterInstance(mocks.Player().Object);
			builder.RegisterInstance(mocks.Character().Object);
			builder.RegisterInstance(mocks.Room().Object);
			builder.RegisterInstance(mocks.Object().Object);
			builder.RegisterInstance(mocks.Viewport().Object);
			builder.RegisterInstance(mocks.Sprite().Object);
			builder.RegisterInstance(mocks.Image().Object);
			builder.RegisterInstance(mocks.Input().Object);
			builder.RegisterInstance(mocks.Cutscene().Object);
			builder.RegisterInstance(mocks.RoomTransitions().Object);
			builder.RegisterInstance(mocks.AudioSystem().Object);
			builder.RegisterInstance(new Mock<IRenderMessagePump> ().Object);
            builder.RegisterInstance(new Mock<IUpdateMessagePump>().Object);
            builder.RegisterInstance(new Mock<IGameEvents>().Object);
            builder.RegisterInstance(new Mock<IDisplayList>().Object);
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

			mocks.container = builder.Build();

			return mocks;
		}

        public static void Bind<T>(Mock<IEntity> entity, Mock<T> mock) where T:class, IComponent
        {
            entity.Setup(e => e.Bind<T>(It.IsAny<Action<T>>(), It.IsAny<Action<T>>())).Callback<Action<T>, Action<T>>((a, b) => a(mock.Object));
        }

        public static Resolver GetResolver()
        {
            Mock<IDevice> device = new Mock<IDevice>();
            Mock<IEngineConfigFile> configFile = new Mock<IEngineConfigFile>();
            device.Setup(d => d.ConfigFile).Returns(configFile.Object);

            Mock<IBitmapLoader> bitmapLoader = new Mock<IBitmapLoader>();
            bitmapLoader.Setup(loader => loader.Load(It.IsAny<int>(), It.IsAny<int>())).Returns(new Mock<IBitmap>().Object);
            device.Setup(d => d.BitmapLoader).Returns(bitmapLoader.Object);

            Mock<IAssemblies> assemblies = new Mock<IAssemblies>();
            device.Setup(d => d.Assemblies).Returns(assemblies.Object);

            var mocks = new Mocks();
            device.Setup(d => d.FileSystem).Returns(mocks.FileSystem().Object);

            Mock<IBrushLoader> brushes = new Mock<IBrushLoader>();
            device.Setup(d => d.BrushLoader).Returns(brushes.Object);

            Mock<IGraphicsBackend> graphics = new Mock<IGraphicsBackend>();
            device.Setup(d => d.GraphicsBackend).Returns(graphics.Object);

            Mock<IFontLoader> fonts = new Mock<IFontLoader>();
            device.Setup(d => d.FontLoader).Returns(fonts.Object);

            Mock<IKeyboardState> keyboard = new Mock<IKeyboardState>();
            device.Setup(d => d.KeyboardState).Returns(keyboard.Object);

            var resolver = new Resolver(device.Object);

            Mock<IAudioBackend> audio = new Mock<IAudioBackend>();
            resolver.Builder.RegisterInstance(audio.Object);
            return resolver;
        }

		public TItem Create<TItem>(params Parameter[] parameters)
		{
			return container.Resolve<TItem>(parameters);
		}

		public void Dispose()
		{
			container.Dispose();
		}

        public Mock<IFileSystem> FileSystem()
        {
            Mock<IFileSystem> files = new Mock<IFileSystem>();
            files.Setup(f => f.StorageFolder).Returns("Test");
            return files;
        }

		public Mock<IALAudioSystem> AudioSystem(bool newInstance = false)
		{
			if (_audioSystem == null || newInstance)
			{
                _audioSystem = new Mock<IALAudioSystem> ();
			}
			return _audioSystem;
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

        public Mock<IGame> Game()
        {
            if (_game == null)
            {
                _game = new Mock<IGame>();
                _game.Setup(g => g.State).Returns(GameState().Object);
                _game.Setup(g => g.Settings).Returns(Settings().Object);
            }
            return _game;
        }

        public Mock<IRuntimeSettings> Settings()
        {
            if (_settings == null)
            {
                _settings = new Mock<IRuntimeSettings>();
                _settings.Setup(g => g.VirtualResolution).Returns(new AGS.API.Size(640, 480));
            }
            return _settings;
        }

		public Mock<IGameState> GameState()
		{
			if (_gameState == null)
			{
				_gameState = new Mock<IGameState> ();
				_gameState.Setup(m => m.Player).Returns(Player().Object);
                _gameState.Setup(m => m.Room).Returns(() => Player().Object.Room);
                _gameState.Setup(m => m.Rooms).Returns(() => { var rooms = new AGSBindingList<IRoom>(1); rooms.Add(Player().Object.Room); return rooms; });
                _gameState.Setup(m => m.Viewport).Returns(Viewport().Object);
                _gameState.Setup(m => m.SecondaryViewports).Returns(new AGSBindingList<IViewport>(0));
                Viewport().Setup(v => v.RoomProvider).Returns(_gameState.Object);
				_gameState.Setup(m => m.Cutscene).Returns(Cutscene().Object);
			}
			return _gameState;
		}

        public Mock<ICharacter> Player()
		{
			if (_player == null)
			{
				_player = new Mock<ICharacter> ();
				_player.Setup(m => m.Room).Returns(Room().Object);
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
                AGSBindingList<IArea> areas = new AGSBindingList<IArea>(5);
				AGSConcurrentHashSet<IObject> roomObjects = new AGSConcurrentHashSet<IObject> ();
				_room = new Mock<IRoom> ();
				_room.Setup(m => m.Background).Returns(Object().Object);
				_room.Setup(m => m.Objects).Returns(roomObjects);
				_room.Setup(m => m.ShowPlayer).Returns(true);
				_room.Setup(m => m.Events).Returns(RoomEvents().Object);
				_room.Setup(m => m.Areas).Returns(areas);
                _room.Setup(m => m.ID).Returns(new Guid().ToString());
			}
			return _room;
		}

		public Mock<IRoomEvents> RoomEvents()
		{
			if (_roomEvents == null)
			{
				_roomEvents = new Mock<IRoomEvents> ();
				_roomEvents.Setup(r => r.OnAfterFadeIn).Returns(new Mock<IEvent> ().Object);
				_roomEvents.Setup(r => r.OnAfterFadeOut).Returns(new Mock<IBlockingEvent> ().Object);
				_roomEvents.Setup(r => r.OnBeforeFadeIn).Returns(new Mock<IBlockingEvent> ().Object);
				_roomEvents.Setup(r => r.OnBeforeFadeOut).Returns(new Mock<IBlockingEvent> ().Object);
			}
			return _roomEvents;
		}

		public Mock<IAGSRoomTransitions> RoomTransitions()
		{
			var transitions = new Mock<IAGSRoomTransitions> ();
			return transitions;
		}

		public Mock<IObject> Object(bool newInstance = false)
		{
            if (_obj == null || newInstance)
			{
				_obj = new Mock<IObject> ();
                _obj.Setup(m => m.ID).Returns(new Guid().ToString());
				_obj.Setup(m => m.Animation).Returns(Animation().Object);
				_obj.Setup(m => m.Image).Returns(Image().Object);
				_obj.Setup(m => m.Enabled).Returns(true);
				_obj.Setup(m => m.Visible).Returns(true);
				_obj.Setup(m => m.Pivot).Returns(new AGS.API.PointF ());
                _obj.Setup(m => m.TreeNode).Returns(new AGSTreeNode<IObject>());
                _obj.Setup(m => m.Location).Returns(new AGSLocation(0f, 0f));
                _obj.Setup(m => m.Properties).Returns(new AGSCustomProperties());
			}
			return _obj;
		}

		public Mock<IViewport> Viewport()
		{
			if (_viewport == null)
			{
				_viewport = new Mock<IViewport> ();
                _viewport.Setup(v => v.DisplayListSettings).Returns(new AGSDisplayListSettings());
			}
			return _viewport;
		}

		public Mock<ISprite> Sprite(bool newInstance = false)
		{
			if (_sprite == null || newInstance)
			{
				_sprite = new Mock<ISprite> ();
				_sprite.Setup(m => m.Image).Returns(Image().Object);
				_sprite.Setup(m => m.Pivot).Returns(new AGS.API.PointF ());
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

