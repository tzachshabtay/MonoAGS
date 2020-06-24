using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;
using Moq;
using NUnit.Framework;

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
		public async Task ChangeRoom_WhenNotInRoom_Test()
		{
            Mock<IGameState> state = _mocks.GameState();
			AGSBindingList<IRoom> rooms = new AGSBindingList<IRoom> (10);
			state.Setup(s => s.Rooms).Returns(rooms);

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
				rooms.Clear();
				IRoom room = _mocks.Room(true).Object;
				rooms.Add(room);
				await obj.ChangeRoomAsync(room);
				Assert.AreEqual(room, obj.Room, $"Room not changed for {(obj.DisplayName ?? "null")}");
				Assert.IsNull(obj.PreviousRoom, $"Prev room not null for {(obj.DisplayName ?? "null")}");
			}
		}

		[Test]
		public async Task ChangeRoom_WhenInRoom_Test()
		{
			Mock<IGameState> state = _mocks.GameState();
			AGSBindingList<IRoom> rooms = new AGSBindingList<IRoom> (10);
			state.Setup(s => s.Rooms).Returns(rooms);
			state.Setup(s => s.Player).Returns(_mocks.Player().Object);

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				IRoom newRoom = _mocks.Room(true).Object;
				rooms.Add(oldRoom);
				rooms.Add(newRoom);
			    await testChangeRoom(obj, oldRoom, newRoom);
			}
		}

		[Test]
		public async Task ChangeRoom_ToSameRoom_Test()
		{
			Mock<IGameState> state = _mocks.GameState();
			AGSBindingList<IRoom> rooms = new AGSBindingList<IRoom> (10);
			state.Setup(s => s.Rooms).Returns(rooms);
			state.Setup(s => s.Player).Returns(_mocks.Player().Object);

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				IRoom newRoom = oldRoom;
				rooms.Add(oldRoom);
				rooms.Add(newRoom);
			    await testChangeRoom(obj, oldRoom, newRoom);
			}
		}

		[Test]
		public async Task ChangeRoom_ToNullRoom_Test()
		{
			Mock<IGameState> state = _mocks.GameState();
			AGSBindingList<IRoom> rooms = new AGSBindingList<IRoom> (10);
			state.Setup(s => s.Rooms).Returns(rooms);
			state.Setup(s => s.Player).Returns(_mocks.Player().Object);

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
				rooms.Clear();
				IRoom oldRoom = _mocks.Room(true).Object;
				rooms.Add(oldRoom);
			    await testChangeRoom(obj, oldRoom, null);
			}
		}

	    private async Task testChangeRoom(IObject obj, IRoom oldRoom, IRoom newRoom)
	    {
	        await obj.ChangeRoomAsync(oldRoom);
	        await obj.ChangeRoomAsync(newRoom);
	        Assert.AreEqual(newRoom, obj.Room, $"Room not changed for {(obj.DisplayName ?? "null")}");
	        Assert.AreEqual(oldRoom, obj.PreviousRoom, $"Prev room incorrect for {(obj.DisplayName ?? "null")}");
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
			Mock<IGameState> state = _mocks.GameState();
			AGSBindingList<IRoom> rooms = new AGSBindingList<IRoom> (10);
			state.Setup(s => s.Rooms).Returns(rooms);

			Action<IObject, bool> setBool = (propertyName == "Visible") ? _setVisible : _setEnabled;

			Predicate<IObject> getBool = propertyName == "Visible" ? _getVisible : _getEnabled;

            AGSConcurrentHashSet<IObject> ui = new AGSConcurrentHashSet<IObject>();

			foreach (IObject obj in GetImplementors(_mocks, state))
			{
                state.Setup(s => s.UI).Returns(ui);
                ui.Add(obj);
				setBool(obj, isObj);
				if (!isParent.HasValue)
				{
					bool actual = getBool(obj);
					Assert.AreEqual(result, actual);
					continue;
				}
				foreach (IObject parent in GetImplementors(_mocks, state))
				{
                    state.Setup(s => s.UI).Returns(ui);
                    ui.Add(parent);
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
                        state.Setup(s => s.UI).Returns(ui);
                        ui.Add(grandparent);
						parent.TreeNode.SetParent(grandparent.TreeNode);
						setBool(grandparent, isGrandParent.Value);

						bool actual = getBool(obj);
						Assert.AreEqual(result, actual);
					}
				}
			}
		}

        public static Resolver GetResolver()
        { 
            return Mocks.GetResolver();
        }

        public static IEnumerable<IObject> GetImplementors(Mocks mocks, Mock<IGameState> stateMock, IGameState state = null)
        {
            if (state == null && stateMock != null) state = stateMock.Object;
            mocks.Game().Setup(g => g.State).Returns(state);
            stateMock?.Setup(s => s.UI).Returns(new AGSConcurrentHashSet<IObject>());
            Mock<IInput> input = new Mock<IInput>();
            Resolver resolver = GetResolver();
            input.Setup(i => i.KeyUp).Returns(new Mock<IEvent<KeyboardEventArgs>>().Object);
            input.Setup(i => i.KeyDown).Returns(new Mock<IEvent<KeyboardEventArgs>>().Object);
            input.Setup(i => i.MouseDown).Returns(new Mock<IEvent<MouseButtonEventArgs>>().Object);
            if (stateMock != null) stateMock.Setup(s => s.Cutscene).Returns(mocks.Cutscene().Object);

            Mock<IUIEvents> uiEvents = new Mock<IUIEvents>();
            Mock<IEvent<MouseButtonEventArgs>> buttonEvent = new Mock<IEvent<MouseButtonEventArgs>>();
            Mock<IEvent<MouseClickEventArgs>> buttonClickEvent = new Mock<IEvent<MouseClickEventArgs>>();
            Mock<IEvent<MousePositionEventArgs>> mouseEvent = new Mock<IEvent<MousePositionEventArgs>>();
            uiEvents.Setup(u => u.MouseClicked).Returns(buttonClickEvent.Object);
            uiEvents.Setup(u => u.MouseDown).Returns(buttonEvent.Object);
            uiEvents.Setup(u => u.MouseUp).Returns(buttonEvent.Object);
            uiEvents.Setup(u => u.LostFocus).Returns(buttonEvent.Object);
            uiEvents.Setup(u => u.MouseEnter).Returns(mouseEvent.Object);
            uiEvents.Setup(u => u.MouseLeave).Returns(mouseEvent.Object);
            uiEvents.Setup(u => u.MouseMove).Returns(mouseEvent.Object);

            Mock<IGraphicsFactory> graphicsFactory = new Mock<IGraphicsFactory>();
            Func<ISprite> getSprite = () => new AGSSprite(resolver, mocks.MaskLoader().Object);
            graphicsFactory.Setup(g => g.GetSprite()).Returns(() => getSprite());
            AGSAnimationComponent animationComponent = new AGSAnimationComponent();

            Mock<IImage> image = new Mock<IImage>();
            Mock<IButtonComponent> buttonComponent = new Mock<IButtonComponent>();
            var animation = new AGSAnimation(new AGSAnimationConfiguration { Loops = 1 }, new AGSAnimationState(), 1);
            animation.Frames.Add(new AGSAnimationFrame(getSprite()) { Delay = -1 });
            buttonComponent.Setup(b => b.HoverAnimation).Returns(new ButtonAnimation(animation));
            buttonComponent.Setup(b => b.IdleAnimation).Returns(new ButtonAnimation(animation));
            buttonComponent.Setup(b => b.PushedAnimation).Returns(new ButtonAnimation(animation));
            Mock<IALAudioSystem> audioSystem = new Mock<IALAudioSystem>();
            Mock<IRuntimeSettings> settings = mocks.Settings();
            Mock<IRenderThread> renderThread = new Mock<IRenderThread>();
            Mock<IUpdateThread> updateThread = new Mock<IUpdateThread>();
            renderThread.Setup(u => u.RunBlocking(It.IsAny<Action>())).Callback<Action>(a => a());
            updateThread.Setup(u => u.RunBlocking(It.IsAny<Action>())).Callback<Action>(a => a());

            var walkComponent = new Mock<IWalkComponent>();

            resolver.Builder.RegisterInstance(input.Object);
            resolver.Builder.RegisterInstance(state);
            resolver.Builder.RegisterInstance(uiEvents.Object);
            resolver.Builder.RegisterInstance(animationComponent).As<IAnimationComponent>();
            resolver.Builder.RegisterInstance(buttonComponent.Object);
            resolver.Builder.RegisterInstance(audioSystem.Object);
            resolver.Builder.RegisterInstance(new Mock<IRenderMessagePump>().Object);
            resolver.Builder.RegisterInstance(new Mock<IUpdateMessagePump>().Object);
            resolver.Builder.RegisterInstance(renderThread.Object);
            resolver.Builder.RegisterInstance(updateThread.Object);
            resolver.Builder.RegisterInstance(walkComponent.Object);
            resolver.Builder.RegisterInstance(new Mock<ITexture>().Object);
            resolver.Builder.RegisterInstance(mocks.MaskLoader().Object).As<IMaskLoader>();
            resolver.Builder.RegisterInstance(settings.Object).As<IGameSettings>();
            resolver.Builder.RegisterInstance(settings.Object).As<IRuntimeSettings>();
            resolver.Builder.RegisterInstance(mocks.Game().Object);
            resolver.Build();

            Func<IObject> baseObj = () => new AGSObject("Test" + Guid.NewGuid(), resolver);
            mocks.Game().Setup(g => g.Events).Returns(resolver.Container.Resolve<IGameEvents>());
            mocks.Game().Setup(g => g.Factory).Returns(resolver.Container.Resolve<IGameFactory>());

            Mock<IOutfit> outfit = new Mock<IOutfit>();

            Func<IPanel> basePanel = () => new AGSPanel("Panel" + Guid.NewGuid(), resolver) { Image = image.Object};
            Func<ILabel> baseLabel = () => new AGSLabel("Label" + Guid.NewGuid(), resolver) { LabelRenderSize = new SizeF(100f, 50f) };
            var button = new AGSButton("Button" + Guid.NewGuid(), resolver) { LabelRenderSize = new SizeF(100f, 50f) };

            List<IObject> implmentors = new List<IObject>
            {
                baseObj().Hotspot("Object"),
                new AGSCharacter("Character" + Guid.NewGuid(), resolver, outfit.Object).Hotspot("Character"),
                basePanel().Hotspot("Panel"),
                baseLabel().Hotspot("Label"),
                button.Hotspot("Button"),
                new AGSInventoryWindow("Inventory" + Guid.NewGuid(), resolver, image.Object).Hotspot("Inventory"),
                (new AGSSlider("Slider" + Guid.NewGuid(), resolver) { Image = image.Object}).Hotspot("Slider"),
                new AGSCheckBox("Checkbox" + Guid.NewGuid(), resolver),
                new AGSTextbox("Textbox" + Guid.NewGuid(), resolver),
                new AGSComboBox("Combobox" + Guid.NewGuid(), resolver)
			};

			return implmentors;
		}			
	}	

	public static class ObjectNames
	{
		public static IObject Hotspot(this IObject obj, string hotspot)
		{
			obj.DisplayName = hotspot;
			return obj;
		}
	}
}
