using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using Moq;

namespace Tests
{
	[TestFixture]
	public class EdgesTests
	{
		private IAGSEdges _edges;
		private IEdge _left, _right, _top, _bottom;
		private Mocks _mocks;

		[SetUp]
		public void Init()
		{
			_left = new AGSEdge { Value = 100 };
			_right = new AGSEdge { Value = 200 };
			_top = new AGSEdge  { Value = 200 };
			_bottom = new AGSEdge { Value = 100 };
			_edges = new AGSEdges (_left, _right, _top, _bottom);
			_mocks = Mocks.Init();
		}

		[TearDown]
		public void Teardown()
		{
			_mocks.Dispose();
		}

		[TestCase("Left", 0f, 0f, Result = false)]
		[TestCase("Bottom", 0f, 0f, Result = false)]
		[TestCase("Top", 300f, 300f, Result = false)]
		[TestCase("Right", 300f, 300f, Result = false)]

		[TestCase("Left", 150f, 150f, Result = false)]
		[TestCase("Bottom", 150f, 150f, Result = false)]
		[TestCase("Top", 150f, 150f, Result = false)]
		[TestCase("Right", 150f, 150f, Result = false)]

		[TestCase("Left", 50f, 60f, Result = false)]
		[TestCase("Bottom", 50f, 60f, Result = false)]
		[TestCase("Top", 250f, 260f, Result = false)]
		[TestCase("Right", 250f, 260f, Result = false)]

		[TestCase("Left", 160f, 170f, Result = false)]
		[TestCase("Bottom", 160f, 170f, Result = false)]
		[TestCase("Top", 160f, 170f, Result = false)]
		[TestCase("Right", 160f, 170f, Result = false)]

		[TestCase("Left", 50f, 150f, Result = false)]
		[TestCase("Bottom", 50f, 150f, Result = false)]
		[TestCase("Top", 220f, 180f, Result = false)]
		[TestCase("Right", 220f, 180f, Result = false)]

		[TestCase("Left", 150f, 50f, Result = true)]
		[TestCase("Bottom", 150f, 50f, Result = true)]
		[TestCase("Top", 180f, 220f, Result = true)]
		[TestCase("Right", 180f, 220f, Result = true)]
		public bool EdgeCrossTest(string edgeName, float previous, float current)
		{
			IEdge edge = getEdge(edgeName);
			bool edgeCrossed = false;
			edge.OnEdgeCrossed.Subscribe((sender, e) => edgeCrossed = true);
			Mock<ICharacter> character = _mocks.Character();

			setPosition(character, previous, edgeName);
			_edges.OnRepeatedlyExecute(character.Object);
			Assert.IsFalse(edgeCrossed);

			setPosition(character, current, edgeName);
			_edges.OnRepeatedlyExecute(character.Object);
			return edgeCrossed;
		}

		private void setPosition(Mock<ICharacter> character, float value, string edgeName)
		{
			switch (edgeName)
			{
				case "Left":
				case "Right":
					character.Setup(c => c.X).Returns(value);
					break;
				default:
					character.Setup(c => c.Y).Returns(value);
					break;
			}
		}

		private IEdge getEdge(string edgeName)
		{
			switch (edgeName)
			{
				case "Left":
					return _left;
				case "Right":
					return _right;
				case "Top":
					return _top;
				default:
					return _bottom;
			}
		}
	}
}

