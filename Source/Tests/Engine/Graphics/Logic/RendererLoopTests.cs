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
        private AGSBindingList<IArea> _areas;
        private AGSConcurrentHashSet<IObject> _roomObjects, _uiObjects;
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
        }

        [Test]
        public void RoomProperlyRendered_Test()
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
                var roomObj = _mocks.Object(true);
                var uiObj = _mocks.Object(true);
                roomObj.Setup(c => c.ID).Returns("roomObj");
                uiObj.Setup(c => c.ID).Returns("uiObj");
                _roomObjects.Clear(); _roomObjects.Add(roomObj.Object);
                _uiObjects.Clear(); _uiObjects.Add(uiObj.Object);

                var displayList = getDisplayList();
                var pipeline = getPipeline(displayList);

                var roomRenderer = new Mock<IRenderer>();
                var uiRenderer = new Mock<IRenderer>();
                var roomInsturction = new Mock<IRenderInstruction>();
                var uiInstruction = new Mock<IRenderInstruction>();
                roomRenderer.Setup(r => r.GetNextInstruction(_mocks.GameState().Object.Viewport)).Returns(roomInsturction.Object);
                uiRenderer.Setup(r => r.GetNextInstruction(_mocks.GameState().Object.Viewport)).Returns(uiInstruction.Object);
                pipeline.Subscribe("roomObj", roomRenderer.Object);
                pipeline.Subscribe("uiObj", uiRenderer.Object);

                IRendererLoop loop = getLoop(displayList, pipeline);
                Assert.IsTrue(loop.Tick()); //First tick just to tell the display list about our viewport, the second tick will have the objects to render
                displayList.GetDisplayList(_mocks.GameState().Object.Viewport);
                displayList.Update();
                pipeline.Update();
                Assert.IsTrue(loop.Tick());
                roomInsturction.Verify(r => r.Render(), Times.Once);
                uiInstruction.Verify(r => r.Render(), Times.Once);
            }
            finally 
            {
                AGSGame.UIThreadID = threadID;
            }
        }

        private IDisplayList getDisplayList()
        {
            return new AGSDisplayList(_mocks.GameState().Object, _mocks.Input().Object,
                new Mock<IMatrixUpdater>().Object, new Mock<IAGSRoomTransitions>().Object);
        }

        private IAGSRenderPipeline getPipeline(IDisplayList displayList)
        {
            return new AGSRenderPipeline(_mocks.GameState().Object, displayList, _mocks.Game().Object,
                                           new AGSEvent<DisplayListEventArgs>(), _mocks.RoomTransitions().Object);
        }

        private IRendererLoop getLoop(IDisplayList displayList = null, IAGSRenderPipeline pipeline = null)
        { 
            displayList = displayList ?? getDisplayList();
            pipeline = pipeline ?? getPipeline(displayList);
            return new AGSRendererLoop(_resolver, _mocks.Game().Object,
                                       _transitions.Object, new Mock<IGLUtils>().Object, new Mock<IWindowInfo>().Object, pipeline,
                                       displayList, new Mock<IInput>().Object, new Mock<IMatrixUpdater>().Object);
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