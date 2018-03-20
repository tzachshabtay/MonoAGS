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
    public class RendererLoopTests
    {
        private Mocks _mocks;
        private Mock<IAGSRoomTransitions> _transitions;
        private Mock<IImageRenderer> _renderer;
        private AGSBindingList<IArea> _areas;
        private AGSConcurrentHashSet<IObject> _roomObjects, _uiObjects;
        private IGameEvents _events;
        private Resolver _resolver;

        [SetUp]
        public void Init()
        {
            _mocks = Mocks.Init();
            _transitions = new Mock<IAGSRoomTransitions>();
            _resolver = Mocks.GetResolver();
            _resolver.Build();

            _areas = new AGSBindingList<IArea>(1);
            _roomObjects = new AGSConcurrentHashSet<IObject>();
            _uiObjects = new AGSConcurrentHashSet<IObject>();
            _events = _resolver.Container.Resolve<IGameEvents>(); //new AGSGameEvents(null, new AGSEvent(), null, null, null, null, _resolver);

            var room = _mocks.Room();
            room.Setup(m => m.Objects).Returns(_roomObjects);
            room.Setup(m => m.Areas).Returns(_areas);
            room.Setup(m => m.Background).Returns(() => null);
            _mocks.GameState().Setup(s => s.UI).Returns(_uiObjects);
        }

        [TearDown]
        public void Teardown()
        {
            _mocks.Dispose();
        }

        [Test]
        public void EmptyRoom_NotError_Test()
        {
            _mocks.Room().Setup(m => m.ShowPlayer).Returns(false);
            _mocks.GameState().Setup(m => m.GetSortedViewports()).Returns(new List<IViewport>{_mocks.GameState().Object.Viewport});
            _areas.Clear();
            _roomObjects.Clear();
            _uiObjects.Clear();

            IRendererLoop loop = getLoop();
            Assert.IsTrue(loop.Tick());
            _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>()), Times.Never);
        }

        [Test]
        public async Task RoomProperlyRendered_Test()
        {
            int threadID = AGSGame.UIThreadID;
            AGSGame.UIThreadID = Environment.CurrentManagedThreadId;
            try
            {
                _mocks.Room().Setup(m => m.ShowPlayer).Returns(false);
                AGSViewport viewport = new AGSViewport(new AGSDisplayListSettings(), new AGSCamera(), Mocks.GetResolver());
                viewport.RoomProvider = new AGSSingleRoomProvider(_mocks.Room().Object);
                _mocks.GameState().Setup(m => m.Viewport).Returns(viewport);
                    _mocks.GameState().Setup(m => m.GetSortedViewports()).Returns(new List<IViewport> { _mocks.GameState().Object.Viewport });
                _areas.Clear(); _areas.Add(getArea());
                _roomObjects.Clear(); _roomObjects.Add(_mocks.Object(true).Object);
                _uiObjects.Clear(); _uiObjects.Add(_mocks.Object(true).Object);

                IRendererLoop loop = getLoop();
                Assert.IsTrue(loop.Tick()); //First tick just to tell the display list about our viewport, the second tick will have the objects to render
                await _events.OnRepeatedlyExecute.InvokeAsync(new RepeatedlyExecuteEventArgs());
                Assert.IsTrue(loop.Tick());
                _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>()), Times.Exactly(2));
            }
            finally 
            {
                AGSGame.UIThreadID = threadID;
            }
        }

        private IRendererLoop getLoop()
        { 
            _renderer = new Mock<IImageRenderer>();
            AGSDisplayList displayList = new AGSDisplayList(_mocks.GameState().Object, _mocks.Input().Object,
                                                            new AGSWalkBehindsMap(null), _renderer.Object, _events, new Mock<IAGSRoomTransitions>().Object);
            return new AGSRendererLoop(_resolver, _mocks.Game().Object, _renderer.Object,
                                       _transitions.Object, new Mock<IGLUtils>().Object, new Mock<IGameWindow>().Object,
                                       new AGSEvent<DisplayListEventArgs>(), displayList, new Mock<IInput>().Object, new Mock<IMatrixUpdater>().Object);
        }

        private IArea getArea()
        {
            var resolver = ObjectTests.GetResolver();
            resolver.Build();
            var area = new AGSArea("Area", resolver) { Mask = new AGSMask(new bool[][] { }, null) };
            AGSRoomFactory roomFactory = new AGSRoomFactory(resolver);
            roomFactory.CreateScaleArea(area, 1f, 1f);
            area.AddComponent<IWalkBehindArea>();
            return area;
        }
	}
}

