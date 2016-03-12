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
		private readonly Action<IObject, bool> _setVisible = (o, val) => o.Visible = val;
		private readonly Action<IObject, bool> _setEnabled = (o, val) => o.Enabled = val;
		private readonly Predicate<IObject> _getVisible = o => o.Visible;
		private readonly Predicate<IObject> _getEnabled = o => o.Enabled;

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

			foreach (IObject obj in GetImplementors(_mocks, state))
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

			foreach (IObject obj in GetImplementors(_mocks, state))
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

			foreach (IObject obj in GetImplementors(_mocks, state))
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

			foreach (IObject obj in GetImplementors(_mocks, state))
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

		[TestCase("Visible", false, null, null, false)]
		[TestCase("Visible", true, null, null, true)]
		[TestCase("Visible", false, false, null, false)]
		[TestCase("Visible", false, true, null, false)]
		[TestCase("Visible", true, false, null, false)]
		[TestCase("Visible", true, true, null, true)]
		[TestCase("Visible", false, false, false, false)]
		[TestCase("Visible", false, false, true, false)]
		[TestCase("Visible", false, true, false, false)]
		[TestCase("Visible", false, true, true, false)]
		[TestCase("Visible", true, false, false, false)]
		[TestCase("Visible", true, false, true, false)]
		[TestCase("Visible", true, true, false, false)]
		[TestCase("Visible", true, true, true, true)]

		[TestCase("Enabled", false, null, null, false)]
		[TestCase("Enabled", true, null, null, true)]
		[TestCase("Enabled", false, false, null, false)]
		[TestCase("Enabled", false, true, null, false)]
		[TestCase("Enabled", true, false, null, false)]
		[TestCase("Enabled", true, true, null, true)]
		[TestCase("Enabled", false, false, false, false)]
		[TestCase("Enabled", false, false, true, false)]
		[TestCase("Enabled", false, true, false, false)]
		[TestCase("Enabled", false, true, true, false)]
		[TestCase("Enabled", true, false, false, false)]
		[TestCase("Enabled", true, false, true, false)]
		[TestCase("Enabled", true, true, false, false)]
		[TestCase("Enabled", true, true, true, true)]
		public void ObjectBoolProperties_Test(string propertyName, bool isObj, bool? isParent, bool? isGrandParent, bool result)
		{
			Mock<IGameState> state = new Mock<IGameState> ();
			List<IRoom> rooms = new List<IRoom> ();
			state.Setup(s => s.Rooms).Returns(rooms);

			Action<IObject, bool> setBool = (propertyName == "Visible") ? _setVisible : _setEnabled;

			Predicate<IObject> getBool = propertyName == "Visible" ? _getVisible : _getEnabled;

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
				setBool(obj, isObj);
				if (!isParent.HasValue)
				{
					bool actual = getBool(obj);
					Assert.AreEqual(result, actual);
					continue;
				}
				foreach (IObject parent in GetImplementors(_mocks, state))
				{
					obj.TreeNode.SetParent(parent.TreeNode);
					setBool(parent, isParent.Value);
					if (!isGrandParent.HasValue)
					{
						bool actual = getBool(obj);
						Assert.AreEqual(result, actual);
						continue;
					}
					foreach (IObject grandparent in GetImplementors(_mocks, state))
					{
						parent.TreeNode.SetParent(grandparent.TreeNode);
						setBool(grandparent, isGrandParent.Value);

						bool actual = getBool(obj);
						Assert.AreEqual(result, actual);
					}
				}
			}
		}

		public static IEnumerable<IObject> GetImplementors(Mocks mocks, Mock<IGameState> stateMock, IGameState state = null)
		{
			if (state == null && stateMock != null) state = stateMock.Object;
			Resolver resolver = new Resolver ();
			Mock<IInput> input = new Mock<IInput> ();
			if (stateMock != null) stateMock.Setup(s => s.Cutscene).Returns(mocks.Cutscene().Object);

			resolver.Builder.RegisterInstance(input.Object);
			resolver.Builder.RegisterInstance(state);
			resolver.Build();

			Mock<IGraphicsFactory> graphicsFactory = new Mock<IGraphicsFactory> ();
			Func<ISprite> getSprite = () => new AGSSprite (mocks.MaskLoader().Object);
			graphicsFactory.Setup(g => g.GetSprite()).Returns(() => getSprite());
			AGSAnimationContainer animationContainer = new AGSAnimationContainer (getSprite(), graphicsFactory.Object);
			Mock<IGameEvents> gameEvents = new Mock<IGameEvents> ();
			Mock<IEvent<AGSEventArgs>> emptyEvent = new Mock<IEvent<AGSEventArgs>> ();
			gameEvents.Setup(ev => ev.OnRepeatedlyExecute).Returns(emptyEvent.Object);
			Func<IObject> baseObj = () => new AGSObject ("Test", animationContainer, 
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
				image.Object, gameEvents.Object, input.Object, state, resolver);
			Func<ILabel> baseLabel = () => new AGSLabel (basePanel(), uiEvents.Object, 
				image.Object, renderer.Object, new SizeF (100f, 50f), resolver);

			List<IObject> implmentors = new List<IObject>
			{
				baseObj().Hotspot("Object"),
				new AGSCharacter(baseObj(), outfit.Object, resolver, pathFinder.Object).Hotspot("Character"),
				basePanel().Hotspot("Panel"),
				baseLabel().Hotspot("Label"),
				new AGSButton(baseLabel(), resolver).Hotspot("Button"),
				new AGSInventoryWindow(basePanel(), gameEvents.Object, state, resolver).Hotspot("Inventory"),
				new AGSSlider(basePanel(), input.Object, gameEvents.Object, state, resolver).Hotspot("Slider"),
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

