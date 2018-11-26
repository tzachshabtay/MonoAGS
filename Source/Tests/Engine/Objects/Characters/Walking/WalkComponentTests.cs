using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class WalkComponentTests
	{
        private AGSEvent<IRepeatedlyExecuteEventArgs> _onRepeatedlyExecute;
		private bool _testCompleted;

		[TestFixtureSetUp]
		public void Init()
		{
            _onRepeatedlyExecute = new AGSEvent<IRepeatedlyExecuteEventArgs>();
			startTicks();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_testCompleted = true;
		}

        [TestCase(0f, 0f, true, 5f, 5f, true, 3f, 3f, true)]
		[TestCase(5f, 5f, true, 0f, 0f, true, 3f, 3f, true)]
		[TestCase(0f, 0f, false, 5f, 5f, true, 3f, 3f, true)]
		[TestCase(5f, 5f, false, 0f, 0f, true, 3f, 3f, true)]
		[TestCase(0f, 0f, true, 5f, 5f, false, 3f, 3f, true)]
		[TestCase(5f, 5f, true, 0f, 0f, false, 3f, 3f, true)]
		[TestCase(0f, 0f, true, 5f, 5f, false, 3f, 3f, false)]
		[TestCase(5f, 5f, true, 0f, 0f, false, 3f, 3f, false)]
		public async Task WalkTest(float fromX, float fromY, bool fromWalkable, 
			                 float toX, float toY, bool toWalkable, 
			                 float closeToX, float closeToY, bool hasCloseToWalkable)
		{
            //Setup:
            Mock<IEntity> entity = new Mock<IEntity>();
            Mock<ITranslateComponent> translate = new Mock<ITranslateComponent>();
            Mock<IHasRoomComponent> hasRoom = new Mock<IHasRoomComponent>();
			Mock<IOutfitComponent> outfitHolder = new Mock<IOutfitComponent> ();
			Mock<IOutfit> outfit = new Mock<IOutfit> ();
            Mock<IDrawableInfoComponent> drawable = new Mock<IDrawableInfoComponent>();
			Mock<IPathFinder> pathFinder = new Mock<IPathFinder> ();
			Mock<IFaceDirectionComponent> faceDirection = new Mock<IFaceDirectionComponent> ();
			Mock<IObjectFactory> objFactory = new Mock<IObjectFactory> ();
			Mock<IRoom> room = new Mock<IRoom> ();
            Mock<IViewport> viewport = new Mock<IViewport>();
			Mock<IArea> area = new Mock<IArea> ();
            Mock<IWalkableArea> walkableArea = new Mock<IWalkableArea>();
			Mock<IMask> mask = new Mock<IMask> ();
			Mock<ICutscene> cutscene = new Mock<ICutscene> ();
			Mock<IGameState> gameState = new Mock<IGameState> ();
            Mock<IGame> game = new Mock<IGame>();
            Mock<IGameEvents> gameEvents = new Mock<IGameEvents>();

			gameEvents.Setup(g => g.OnRepeatedlyExecute).Returns(_onRepeatedlyExecute);
            game.Setup(g => g.State).Returns(gameState.Object);
            game.Setup(g => g.Events).Returns(gameEvents.Object);
			gameState.Setup(s => s.Cutscene).Returns(cutscene.Object);
            gameState.Setup(r => r.Viewport).Returns(viewport.Object);
            room.Setup(r => r.Areas).Returns(new AGSBindingList<IArea>(1) { area.Object });
            walkableArea.Setup(w => w.IsWalkable).Returns(true);
			area.Setup(a => a.Enabled).Returns(true);
		    // ReSharper disable CompareOfFloatsByEqualityOperator
		    area.Setup(a => a.IsInArea(It.Is<PointF>(p => p.X == fromX && p.Y == fromY))).Returns(fromWalkable);
			area.Setup(a => a.IsInArea(It.Is<PointF>(p => p.X == toX && p.Y == toY))).Returns(toWalkable);
            area.Setup(a => a.IsInArea(It.Is<PointF>(p => p.X == toX - 1 && p.Y == toY - 1))).Returns(toWalkable);
            area.Setup(a => a.IsInArea(It.Is<PointF>(p => p.X == toX + 1 && p.Y == toY + 1))).Returns(toWalkable);
			area.Setup(a => a.IsInArea(It.Is<PointF>(p => p.X == closeToX && p.Y == closeToY))).Returns(hasCloseToWalkable);
			area.Setup(a => a.Mask).Returns(mask.Object);
            area.Setup(a => a.GetComponent<IWalkableArea>()).Returns(walkableArea.Object);
			float distance;
			area.Setup(a => a.FindClosestPoint(It.Is<PointF>(p => p.X == toX && p.Y == toY), out distance)).Returns(new PointF (closeToX, closeToY));
		    // ReSharper restore CompareOfFloatsByEqualityOperator
			mask.Setup(m => m.Width).Returns(10);

			outfitHolder.Setup(o => o.Outfit).Returns(outfit.Object);

			float x = fromX;
			float y = fromY;
            hasRoom.Setup(o => o.Room).Returns(room.Object);
			translate.Setup(o => o.X).Returns(() => x);
			translate.Setup(o => o.Y).Returns(() => y);
            translate.Setup(o => o.Position).Returns(() => (x, y));
            translate.SetupSet(o => o.Position = It.IsAny<Position>()).Callback<Position>(f => (x, y, _) = f);
			translate.SetupSet(o => o.X = It.IsAny<float>()).Callback<float>(f => x = f);
			translate.SetupSet(o => o.Y = It.IsAny<float>()).Callback<float>(f => y = f);

            Mocks.Bind(entity, hasRoom);
            Mocks.Bind(entity, translate);
            Mocks.Bind(entity, faceDirection);
            Mocks.Bind(entity, outfitHolder);
            Mocks.Bind(entity, drawable);

            Position toLocation = (toX, toY);
            Position closeLocation = (closeToX, closeToY);

		    // ReSharper disable CompareOfFloatsByEqualityOperator
		    pathFinder.Setup(p => p.GetWalkPoints(It.Is<Position>(l => l.X == fromX && l.Y == fromY),
				It.Is<Position>(l => l.X == toX && l.Y == toY))).Returns(toWalkable ? new List<Position> {toLocation} : new List<Position>());

            pathFinder.Setup(p => p.GetWalkPoints(It.Is<Position>(l => l.X == fromX && l.Y == fromY),
				It.Is<Position>(l => l.X == closeToX && l.Y == closeToY))).Returns(hasCloseToWalkable ? new List<Position> {closeLocation} : new List<Position>());
		    // ReSharper restore CompareOfFloatsByEqualityOperator

			AGSWalkComponent walk = new AGSWalkComponent (pathFinder.Object, objFactory.Object, game.Object) 
			    { WalkStep = new PointF(4f, 4f), MovementLinkedToAnimation = false };

			bool walkShouldSucceed = fromWalkable && (toWalkable || hasCloseToWalkable);

            walk.Init(entity.Object, typeof(IWalkComponent));
            walk.AfterInit();

			//Act:
			bool walkSucceded = await walk.WalkAsync(toLocation);

			//Test:
			Assert.AreEqual(walkShouldSucceed, walkSucceded);

			if (walkShouldSucceed)
			{				
				Assert.AreEqual(toWalkable ? toX : closeToX, x, 0.1f);
				Assert.AreEqual(toWalkable ? toY : closeToY, y, 0.1f);
			}
		}

		private async void startTicks()
		{
			await tick();
		}

		private async Task tick()
		{
			if (_testCompleted) return;
			await Task.Delay(10);
            await _onRepeatedlyExecute.InvokeAsync(new RepeatedlyExecuteEventArgs());
			await tick();
		}
	}
}
