using System;
using NUnit.Framework;
using AGS.API;
using System.Collections.Generic;
using AGS.Engine;
using Moq;
using System.Drawing;
using Autofac;

namespace Tests
{
	[TestFixture]
	public class ObjectTests
	{
		private Mocks _mocks;

		[SetUp]
		public void Init()
		{
			_mocks = Mocks.Init();
		}

		[TearDown]
		public void Teardown()
		{
			_mocks.Dispose();
		}



		[Test]
		public void ChangeRoom_WhenNotInRoom_Test()
		{
			Mock<IGameState> state = new Mock<IGameState> ();
			List<IRoom> rooms = new List<IRoom> ();
			state.Setup(s => s.Rooms).Returns(rooms);

			foreach (IObject obj in GetImplementors(_mocks, state.Object))
			{
				rooms.Clear();
				IRoom room = _mocks.Room(true).Object;
				rooms.Add(room);
				obj.ChangeRoom(room);
				Assert.AreEqual(room, obj.Room, "Room not changed for " + obj.Hotspot ?? "null");
				Assert.IsNull(obj.PreviousRoom, "Prev room not null for " + obj.Hotspot ?? "null");
			}
		}

		[Test]
		public void ChangeRoom_WhenInRoom_Test()
		{
			Mock<IGameState> state = new Mock<IGameState> ();
			List<IRoom> rooms = new List<IRoom> ();
			state.Setup(s => s.Rooms).Returns(rooms);

			foreach (IObject obj in GetImplementors(_mocks, state.Object))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				IRoom newRoom = _mocks.Room(true).Object;
				rooms.Add(oldRoom);
				rooms.Add(newRoom);
				obj.ChangeRoom(oldRoom);
				obj.ChangeRoom(newRoom);
				Assert.AreEqual(newRoom, obj.Room, "Room not changed for " + obj.Hotspot ?? "null");
				Assert.AreEqual(oldRoom, obj.PreviousRoom, "Prev room incorrect for " + obj.Hotspot ?? "null");
			}
		}

		[Test]
		public void ChangeRoom_ToSameRoom_Test()
		{
			Mock<IGameState> state = new Mock<IGameState> ();
			List<IRoom> rooms = new List<IRoom> ();
			state.Setup(s => s.Rooms).Returns(rooms);

			foreach (IObject obj in GetImplementors(_mocks, state.Object))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				IRoom newRoom = oldRoom;
				rooms.Add(oldRoom);
				rooms.Add(newRoom);
				obj.ChangeRoom(oldRoom);
				obj.ChangeRoom(newRoom);
				Assert.AreEqual(newRoom, obj.Room, "Room not changed for " + obj.Hotspot ?? "null");
				Assert.AreEqual(oldRoom, obj.PreviousRoom, "Prev room incorrect for " + obj.Hotspot ?? "null");
			}
		}

		[Test]
		public void ChangeRoom_ToNullRoom_Test()
		{
			Mock<IGameState> state = new Mock<IGameState> ();
			List<IRoom> rooms = new List<IRoom> ();
			state.Setup(s => s.Rooms).Returns(rooms);

			foreach (IObject obj in GetImplementors(_mocks, state.Object))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				IRoom newRoom = null;
				rooms.Add(oldRoom);
				obj.ChangeRoom(oldRoom);
				obj.ChangeRoom(newRoom);
				Assert.AreEqual(newRoom, obj.Room, "Room not changed for " + obj.Hotspot ?? "null");
				Assert.AreEqual(oldRoom, obj.PreviousRoom, "Prev room incorrect for " + obj.Hotspot ?? "null");
			}
		}

		public static IEnumerable<IObject> GetImplementors(Mocks mocks, IGameState state)
		{
			Resolver resolver = new Resolver ();
			Mock<IInput> input = new Mock<IInput> ();

			resolver.Builder.RegisterInstance(input.Object);
			resolver.Builder.RegisterInstance(state);
			resolver.Build();

			Mock<IGraphicsFactory> graphicsFactory = new Mock<IGraphicsFactory> ();
			Func<ISprite> getSprite = () => new AGSSprite (mocks.MaskLoader().Object);
			graphicsFactory.Setup(g => g.GetSprite()).Returns(() => getSprite());
			AGSAnimationContainer animationContainer = new AGSAnimationContainer (mocks.Sprite().Object, graphicsFactory.Object);
			Mock<IGameEvents> gameEvents = new Mock<IGameEvents> ();
			Mock<IEvent<AGSEventArgs>> emptyEvent = new Mock<IEvent<AGSEventArgs>> ();
			gameEvents.Setup(ev => ev.OnRepeatedlyExecute).Returns(emptyEvent.Object);
			Func<IObject> baseObj = () => new AGSObject (animationContainer, 
				gameEvents.Object, resolver);

			Mock<IOutfit> outfit = new Mock<IOutfit> ();
			Mock<IPathFinder> pathFinder = new Mock<IPathFinder> ();
			Mock<IUIEvents> uiEvents = new Mock<IUIEvents> ();
			Mock<IEvent<MouseButtonEventArgs>> buttonEvent = new Mock<IEvent<MouseButtonEventArgs>> ();
			Mock<IEvent<MousePositionEventArgs>> mouseEvent = new Mock<IEvent<MousePositionEventArgs>> ();
			uiEvents.Setup(u => u.MouseClicked).Returns(buttonEvent.Object);
			uiEvents.Setup(u => u.MouseDown).Returns(buttonEvent.Object);
			uiEvents.Setup(u => u.MouseUp).Returns(buttonEvent.Object);
			uiEvents.Setup(u => u.MouseEnter).Returns(mouseEvent.Object);
			uiEvents.Setup(u => u.MouseLeave).Returns(mouseEvent.Object);
			uiEvents.Setup(u => u.MouseMove).Returns(mouseEvent.Object);
			Mock<IImage> image = new Mock<IImage> ();
			Mock<ILabelRenderer> renderer = new Mock<ILabelRenderer> ();

			Func<IPanel> basePanel = () => new AGSPanel (baseObj(), uiEvents.Object,
				                         image.Object, gameEvents.Object, input.Object);
			Func<ILabel> baseLabel = () => new AGSLabel (basePanel(), uiEvents.Object, 
				                         image.Object, renderer.Object, new SizeF (100f, 50f));

			List<IObject> implmentors = new List<IObject>
			{
				baseObj().Hotspot("Object"),
				new AGSCharacter(baseObj(), outfit.Object, resolver, pathFinder.Object).Hotspot("Character"),
				basePanel().Hotspot("Panel"),
				baseLabel().Hotspot("Label"),
				new AGSButton(baseLabel()).Hotspot("Button"),
				new AGSInventoryWindow(basePanel(), gameEvents.Object, state).Hotspot("Inventory"),
				new AGSSlider(basePanel(), input.Object, gameEvents.Object, state).Hotspot("Slider"),
			};

			return implmentors;
		}			
	}	

	public static class ObjectNames
	{
		public static IObject Hotspot(this IObject obj, string hotspot)
		{
			obj.Hotspot = hotspot;
			return obj;
		}
	}
}

