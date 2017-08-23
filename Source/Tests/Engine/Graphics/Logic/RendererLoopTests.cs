using System.Collections.Generic;
using AGS.API;
using AGS.Engine;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class RendererLoopTests
    {
        private Mocks _mocks;
        private Mock<IAGSRoomTransitions> _transitions;
        private Mock<IImageRenderer> _renderer;
        private List<IArea> _areas;
        private AGSConcurrentHashSet<IObject> _roomObjects, _uiObjects;
        private Resolver _resolver;

        [SetUp]
        public void Init()
        {
            _mocks = Mocks.Init();
            _transitions = new Mock<IAGSRoomTransitions>();
            _resolver = Mocks.GetResolver();

            _areas = new List<IArea>();
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
            _areas.Clear();
            _roomObjects.Clear();
            _uiObjects.Clear();

            IRendererLoop loop = getLoop();
            Assert.IsTrue(loop.Tick());
            _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>()), Times.Never);
        }

        [Test]
        public void RoomProperlyRendered_Test()
        { 
            _mocks.Room().Setup(m => m.ShowPlayer).Returns(false);
            _areas.Clear(); _areas.Add(getArea());
            _roomObjects.Clear(); _roomObjects.Add(_mocks.Object(true).Object);
            _uiObjects.Clear(); _uiObjects.Add(_mocks.Object(true).Object);

            IRendererLoop loop = getLoop();
            Assert.IsTrue(loop.Tick());
            _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>()), Times.Exactly(2));
        }

        private IRendererLoop getLoop()
        { 
            _renderer = new Mock<IImageRenderer>();
            AGSDisplayList displayList = new AGSDisplayList(_mocks.GameState().Object, _mocks.Input().Object,
                                                            new AGSWalkBehindsMap(null), _renderer.Object);
            return new AGSRendererLoop(_resolver, _mocks.Game().Object, _renderer.Object,
                                       _transitions.Object, new Mock<IGLUtils>().Object, 
                                       new AGSEvent<DisplayListEventArgs>(), displayList);
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

