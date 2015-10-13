using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Moq;
using System.Collections.Generic;

namespace Tests
{
	[TestFixture]
	public class WalkBehaviorTests
	{
		[TestCase(0f, 0f, true, 5f, 5f, true, 3f, 3f, true)]
		[TestCase(5f, 5f, true, 0f, 0f, true, 3f, 3f, true)]
		[TestCase(0f, 0f, false, 5f, 5f, true, 3f, 3f, true)]
		[TestCase(5f, 5f, false, 0f, 0f, true, 3f, 3f, true)]
		[TestCase(0f, 0f, true, 5f, 5f, false, 3f, 3f, true)]
		[TestCase(5f, 5f, true, 0f, 0f, false, 3f, 3f, true)]
		[TestCase(0f, 0f, true, 5f, 5f, false, 3f, 3f, false)]
		[TestCase(5f, 5f, true, 0f, 0f, false, 3f, 3f, false)]
		public void WalkTest(float fromX, float fromY, bool fromWalkable, 
			                 float toX, float toY, bool toWalkable, 
			                 float closeToX, float closeToY, bool hasCloseToWalkable)
		{
			//Setup:
			Mock<IObject> obj = new Mock<IObject> ();
			Mock<IOutfitHolder> outfitHolder = new Mock<IOutfitHolder> ();
			Mock<IOutfit> outfit = new Mock<IOutfit> ();
			Mock<IPathFinder> pathFinder = new Mock<IPathFinder> ();
			Mock<IAGSFaceDirectionBehavior> faceDirection = new Mock<IAGSFaceDirectionBehavior> ();
			Mock<IObjectFactory> objFactory = new Mock<IObjectFactory> ();
			Mock<IRoom> room = new Mock<IRoom> ();
			Mock<IArea> area = new Mock<IArea> ();
			Mock<IMask> mask = new Mock<IMask> ();

			room.Setup(r => r.WalkableAreas).Returns(new List<IArea> { area.Object });
			area.Setup(a => a.Enabled).Returns(true);
			area.Setup(a => a.IsInArea(It.Is<IPoint>(p => p.X == fromX && p.Y == fromY))).Returns(fromWalkable);
			area.Setup(a => a.IsInArea(It.Is<IPoint>(p => p.X == toX && p.Y == toY))).Returns(toWalkable);
			area.Setup(a => a.IsInArea(It.Is<IPoint>(p => p.X == closeToX && p.Y == closeToY))).Returns(hasCloseToWalkable);
			area.Setup(a => a.Mask).Returns(mask.Object);
			float distance = 1f;
			area.Setup(a => a.FindClosestPoint(It.Is<IPoint>(p => p.X == toX && p.Y == toY), out distance)).Returns(new AGSPoint (closeToX, closeToY));
			mask.Setup(m => m.Width).Returns(10);

			outfitHolder.Setup(o => o.Outfit).Returns(outfit.Object);

			float x = fromX;
			float y = fromY;
			obj.Setup(o => o.Room).Returns(room.Object);
			obj.Setup(o => o.X).Returns(() => x);
			obj.Setup(o => o.Y).Returns(() => y);
			obj.Setup(o => o.Location).Returns(() => new AGSLocation (x, y));
			obj.SetupSet(o => o.X = It.IsAny<float>()).Callback<float>(f => x = f);
			obj.SetupSet(o => o.Y = It.IsAny<float>()).Callback<float>(f => y = f);

			ILocation toLocation = new AGSLocation (toX, toY);
			ILocation closeLocation = new AGSLocation (closeToX, closeToY);

			pathFinder.Setup(p => p.GetWalkPoints(It.Is<ILocation>(l => l.X == fromX && l.Y == fromY),
				It.Is<ILocation>(l => l.X == toX && l.Y == toY))).Returns(toWalkable ? new List<ILocation> {toLocation} : new List<ILocation>());

			pathFinder.Setup(p => p.GetWalkPoints(It.Is<ILocation>(l => l.X == fromX && l.Y == fromY),
				It.Is<ILocation>(l => l.X == closeToX && l.Y == closeToY))).Returns(hasCloseToWalkable ? new List<ILocation> {closeLocation} : new List<ILocation>());
			
			AGSWalkBehavior walk = new AGSWalkBehavior (obj.Object, pathFinder.Object, faceDirection.Object,
				                       outfitHolder.Object, objFactory.Object);

			bool walkShouldSucceed = fromWalkable && (toWalkable || hasCloseToWalkable);

			//Act:
			bool walkSucceded = walk.Walk(toLocation);

			//Test:
			Assert.AreEqual(walkShouldSucceed, walkSucceded);

			if (walkShouldSucceed)
			{				
				Assert.AreEqual(toWalkable ? toX : closeToX, x, 0.1f);
				Assert.AreEqual(toWalkable ? toY : closeToY, y, 0.1f);
			}				
		}
	}
}

