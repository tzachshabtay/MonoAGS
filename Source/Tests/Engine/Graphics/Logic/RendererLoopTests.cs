using System;
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
        private List<IArea> _walkables;
        private List<IWalkBehindArea> _walkBehinds;
        private List<IScalingArea> _scalingAreas;
        private AGSConcurrentHashSet<IObject> _roomObjects, _uiObjects;
        private Resolver _resolver;

        [SetUp]
        public void Init()
        {
            _mocks = Mocks.Init();
            _transitions = new Mock<IAGSRoomTransitions>();
            Mock<IEngineConfigFile> configFile = new Mock<IEngineConfigFile>();
            _resolver = new Resolver(configFile.Object);

            _walkables = new List<IArea>();
            _walkBehinds = new List<IWalkBehindArea>();
            _scalingAreas = new List<IScalingArea>();
            _roomObjects = new AGSConcurrentHashSet<IObject>();
            _uiObjects = new AGSConcurrentHashSet<IObject>();

            var room = _mocks.Room();
            room.Setup(m => m.Objects).Returns(_roomObjects);
            room.Setup(m => m.WalkableAreas).Returns(_walkables);
            room.Setup(m => m.WalkBehindAreas).Returns(_walkBehinds);
            room.Setup(m => m.ScalingAreas).Returns(_scalingAreas);
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
            _walkables.Clear();
            _walkBehinds.Clear();
            _scalingAreas.Clear();
            _roomObjects.Clear();
            _uiObjects.Clear();

            IRendererLoop loop = getLoop();
            Assert.IsTrue(loop.Tick());
            _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>(), It.IsAny<PointF>()), Times.Never);
        }

        [Test]
        public void RoomProperlyRendered_Test()
        { 
            _mocks.Room().Setup(m => m.ShowPlayer).Returns(false);
            _walkables.Clear(); _walkables.Add(getArea());
            _walkBehinds.Clear(); _walkBehinds.Add(new AGSWalkBehindArea(getArea()));
            _scalingAreas.Clear(); _scalingAreas.Add(new AGSScalingArea(getArea()));
            _roomObjects.Clear(); _roomObjects.Add(_mocks.Object(true).Object);
            _uiObjects.Clear(); _uiObjects.Add(_mocks.Object(true).Object);

            IRendererLoop loop = getLoop();
            Assert.IsTrue(loop.Tick());
            _renderer.Verify(r => r.Render(It.IsAny<IObject>(), It.IsAny<IViewport>(), It.IsAny<PointF>()), Times.Exactly(2));
        }

        private IRendererLoop getLoop()
        { 
            _renderer = new Mock<IImageRenderer>();
            return new AGSRendererLoop(_resolver, _mocks.Game().Object, _renderer.Object,
                                        _mocks.Input().Object, new AGSWalkBehindsMap(null), _transitions.Object,
                                        new Mock<IGLUtils>().Object);
        }

        private IArea getArea()
        {
            return new AGSArea { Mask = new AGSMask(new bool[][] { }, null) };
        }
	}
}

