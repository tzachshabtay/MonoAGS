using System.Collections.Generic;
using AGS.API;
using AGS.Engine;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TreeNodeTests
    {
        [Test]
        public void TreeNode_AddChildren_Test()
        {
            Mocks mocks = new Mocks();
            var parent = mocks.Object(true);

            var child1 = mocks.Object(true);
            var child2 = mocks.Object(true);
            var childrenToAdd = new List<IObject> { child1.Object, child2.Object };

            AGSTreeNode<IObject> parentNode = new AGSTreeNode<IObject>(parent.Object);
            AGSTreeNode<IObject> child1Node = new AGSTreeNode<IObject>(child1.Object);
            AGSTreeNode<IObject> child2Node = new AGSTreeNode<IObject>(child2.Object);

            child1.Setup(c => c.TreeNode).Returns(child1Node);
            child2.Setup(c => c.TreeNode).Returns(child2Node);

            parentNode.AddChildren(childrenToAdd);

            Assert.AreEqual(parent.Object, child1Node.Parent);
            Assert.AreEqual(parent.Object, child2Node.Parent);
            Assert.AreEqual(child1.Object, parentNode.Children[0]);
            Assert.AreEqual(child2.Object, parentNode.Children[1]);
            Assert.IsTrue(parentNode.HasChild(child1.Object));
            Assert.IsTrue(parentNode.HasChild(child2.Object));
        }
    }
}
