using NUnit.Framework;
using AGS.API;
using Moq;
using AGS.Engine;

namespace Tests
{
	[TestFixture]
	public class RenderOrderSelectorTests
	{
		//Objects in different layers
		[TestCase(0,null,null, 1,null,null, 1f,null,null, 0f,null,null, 0f,null,null, 0f,null,null, Result=true)]
		[TestCase(0,null,null, 1,null,null, 0f,null,null, 1f,null,null, 0f,null,null, 0f,null,null, Result=true)]
		[TestCase(1,null,null, 0,null,null, 0f,null,null, 1f,null,null, 0f,null,null, 0f,null,null, Result=false)]
		[TestCase(1,null,null, 0,null,null, 1f,null,null, 0f,null,null, 0f,null,null, 0f,null,null, Result=false)]

		//Objects in same layer, different Zs
		[TestCase(0,null,null, 0,null,null, 0f,null,null, 1f,null,null, 0f,null,null, 0f,null,null, Result=true)]
		[TestCase(0,null,null, 0,null,null, 1f,null,null, 0f,null,null, 0f,null,null, 0f,null,null, Result=false)]

		//Objects in same layer, different Sprite Zs
		[TestCase(0,null,null, 0,null,null, 0f,null,null, 0f,null,null, 0f,null,null, 1f,null,null, Result=true)]
		[TestCase(0,null,null, 0,null,null, 0f,null,null, 0f,null,null, 1f,null,null, 0f,null,null, Result=false)]

		//Objects in same layer, different Zs and Sprite Zs
		[TestCase(0,null,null, 0,null,null, 0f,null,null, 1f,null,null, 0f,null,null, 1f,null,null, Result=true)]
		[TestCase(0,null,null, 0,null,null, 1f,null,null, 0f,null,null, 1f,null,null, 0f,null,null, Result=false)]

		[TestCase(0,null,null, 0,null,null, 0f,null,null, 2f,null,null, 1f,null,null, 0f,null,null, Result=true)]
		[TestCase(0,null,null, 0,null,null, 2f,null,null, 0f,null,null, 0f,null,null, 1f,null,null, Result=false)]

		[TestCase(0,null,null, 0,null,null, 1f,null,null, 0f,null,null, 0f,null,null, 2f,null,null, Result=true)]
		[TestCase(0,null,null, 0,null,null, 0f,null,null, 1f,null,null, 2f,null,null, 0f,null,null, Result=false)]

		//Now, let's add parents
		//Objects in different layers, parents should not matter
		[TestCase(0,1,null, 1,0,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, 0f,0f,null, Result=true)]
		[TestCase(0,1,null, 1,0,null, 0f,0f,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, Result=true)]
		[TestCase(1,0,null, 0,1,null, 0f,0f,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, Result=false)]
		[TestCase(1,0,null, 0,1,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, 0f,0f,null, Result=false)]

		//Objects have no layer, parent in different layer
		[TestCase(null,0,null, null,1,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, 0f,0f,null, Result=true)]
		[TestCase(null,0,null, null,1,null, 0f,0f,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, Result=true)]
		[TestCase(null,1,null, null,0,null, 0f,0f,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, Result=false)]
		[TestCase(null,1,null, null,0,null, 1f,1f,null, 0f,0f,null, 0f,0f,null, 0f,0f,null, Result=false)]

		//Objects in same layer with different Zs/SpriteZs, parents have the same Zs/SpriteZs
		[TestCase(0,null,null, 0,0,null,    0f,0f,null, 1f,0f,null, 0f,0f,null, 0f,0f,null, Result=true)]
		[TestCase(0,1,null,    0,1,null,    1f,1f,null, 0f,1f,null, 0f,0f,null, 0f,0f,null, Result=false)]

		[TestCase(0,1,null,    0,1,null,    0f,0f,null, 0f,0f,null, 0f,1f,null, 1f,1f,null, Result=true)]
		[TestCase(0,null,null, 0,0,null,    0f,0f,null, 0f,1f,null, 1f,1f,null, 0f,0f,null, Result=false)]

		[TestCase(0,0,null,    0,null,null, 0f,1f,null, 1f,0f,null, 0f,0f,null, 1f,1f,null, Result=true)]
		[TestCase(0,1,null,    0,1,null,    1f,5f,null, 0f,2f,null, 1f,0f,null, 0f,3f,null, Result=false)]

		[TestCase(0,1,null,    0,1,null,    0f,2f,null, 2f,5f,null, 1f,3f,null, 0f,0f,null, Result=true)]
		[TestCase(0,0,null,    0,null,null, 2f,1f,null, 0f,1f,null, 0f,1f,null, 1f,1f,null, Result=false)]

		[TestCase(0,1,null,    0,1,null,    1f,2f,null, 0f,2f,null, 0f,2f,null, 2f,2f,null, Result=true)]
		[TestCase(0,1,null,    0,1,null,    0f,3f,null, 1f,3f,null, 2f,3f,null, 0f,3f,null, Result=false)]

		//Objects in same layer, parents have different Zs/SpriteZs (parents should win regardless of objects Zs/SpriteZs)
		[TestCase(0,null,null, 0,0,null,    0f,1f,null, 1f,0f,null, 0f,0f,null, 0f,0f,null, Result=false)]
		[TestCase(0,0,null,    0,1,null,    1f,0f,null, 0f,1f,null, 0f,0f,null, 0f,0f,null, Result=true)]

		[TestCase(0,1,null,    0,null,null, 0f,0f,null, 0f,0f,null, 0f,1f,null, 1f,0f,null, Result=false)]
		[TestCase(0,null,null, 0,0,null,    0f,1f,null, 0f,1f,null, 1f,0f,null, 0f,1f,null, Result=true)]

		[TestCase(0,0,null,    0,null,null, 0f,1f,null, 1f,0f,null, 0f,1f,null, 1f,0f,null, Result=false)]
		[TestCase(0,1,null,    0,1,null,    1f,2f,null, 0f,5f,null, 1f,5f,null, 0f,3f,null, Result=true)]

		//And finally, a small taste of grandparents
		//Objects in different layers, grandparents should not matter
		[TestCase(0,1,1, 1,0,0, 1f,1f,0f, 0f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=true)]
		[TestCase(0,0,1, 1,1,0, 0f,0f,1f, 1f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=true)]
		[TestCase(1,1,0, 0,0,1, 0f,0f,1f, 1f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(1,0,0, 0,1,1, 1f,1f,0f, 0f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=false)]

		//Objects have no layer, parent in different layer
		[TestCase(null,0,1, null,1,1, 1f,1f,0f, 0f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=true)]
		[TestCase(null,0,1, null,1,1, 0f,0f,1f, 1f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(null,1,0, null,0,0, 0f,0f,1f, 1f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(null,1,0, null,0,0, 1f,1f,0f, 0f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=true)]

		//Objects and parents have no layer, grandparents in different layer
		[TestCase(null,null,0, null,null,1, 1f,0f,1f, 0f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=true)]
		[TestCase(null,null,0, null,null,1, 0f,1f,0f, 1f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=true)]
		[TestCase(null,null,1, null,null,0, 0f,1f,0f, 1f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(null,null,1, null,null,0, 1f,0f,1f, 0f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]

		//Objects in same layer, parents have different Zs/SpriteZs, grandparents have the same Zs
		[TestCase(0,null,1,    0,0,null, 0f,1f,0f, 1f,0f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(0,0,null,    0,1,0,    1f,0f,1f, 0f,1f,1f, 0f,0f,1f, 0f,0f,1f, Result=true)]

		[TestCase(0,1,0,       0,1,0,    0f,0f,0f, 0f,0f,1f, 0f,1f,1f, 1f,0f,0f, Result=false)]
		[TestCase(0,null,null, 0,0,null, 0f,1f,1f, 0f,1f,0f, 1f,0f,0f, 0f,1f,1f, Result=true)]

		[TestCase(0,0,1,       0,null,1, 0f,1f,5f, 1f,0f,2f, 0f,1f,0f, 1f,0f,3f, Result=false)]
		[TestCase(0,1,0,       0,1,0,    1f,2f,2f, 0f,5f,0f, 1f,5f,3f, 0f,3f,5f, Result=true)]

		//Objects in same layer, grandparents have different Zs/SpriteZs (grandparents should win regardless of objects/parents Zs/SpriteZs)
		[TestCase(0,null,1,    0,0,null, 0f,0f,1f, 1f,1f,0f, 0f,0f,0f, 0f,0f,0f, Result=false)]
		[TestCase(0,0,null,    0,1,0,    1f,1f,0f, 0f,0f,1f, 0f,0f,0f, 0f,0f,0f, Result=true)]

		[TestCase(0,1,0,       0,1,0,    0f,0f,0f, 0f,0f,0f, 0f,1f,1f, 1f,0f,0f, Result=false)]
		[TestCase(0,null,null, 0,0,null, 0f,1f,1f, 0f,0f,1f, 1f,0f,0f, 0f,1f,1f, Result=true)]

		[TestCase(0,0,1,       0,null,1, 0f,2f,1f, 1f,1f,0f, 0f,1f,1f, 1f,0f,0f, Result=false)]
		[TestCase(0,1,0,       0,1,0,    1f,1f,2f, 0f,0f,5f, 1f,5f,5f, 0f,3f,3f, Result=true)]

        //Test for bug: https://github.com/tzachshabtay/MonoAGS/issues/269
        //The result here is less important (both objects has the same Z across the board so it's undefined), just making sure it doesn't crash.
        [TestCase(0,0,null,    0,null,null, 0f,0f,null, 0f,null,null, 0f,0f,null, 0f,null,null, Result=false)]
		public bool IsObjectInFrontTest(
			int? o1RenderLayerZ, int? o1ParentRenderLayerZ, int? o1GrandParentRenderLayerZ,
			int? o2RenderLayerZ, int? o2ParentRenderLayerZ, int? o2GrandParentRenderLayerZ,
			float o1Z, float? o1ParentZ, float? o1GrandParentZ,
			float o2Z, float? o2ParentZ, float? o2GrandParentZ,
			float o1SpriteZ, float? o1ParentSpriteZ, float? o1GrandParentSpriteZ,
			float o2SpriteZ, float? o2ParentSpriteZ, float? o2GrandParentSpriteZ)
		{
			IObject grandParent1 = getObject(o1GrandParentRenderLayerZ, o1GrandParentZ, o1GrandParentSpriteZ, null);
			IObject grandParent2 = getObject(o2GrandParentRenderLayerZ, o2GrandParentZ, o2GrandParentSpriteZ, null);

			IObject parent1 = getObject(o1ParentRenderLayerZ, o1ParentZ, o1ParentSpriteZ, grandParent1);
			IObject parent2 = getObject(o2ParentRenderLayerZ, o2ParentZ, o2ParentSpriteZ, grandParent2);

			IObject o1 = getObject(o1RenderLayerZ, o1Z, o1SpriteZ, parent1);
			IObject o2 = getObject(o2RenderLayerZ, o2Z, o2SpriteZ, parent2);

			RenderOrderSelector selector = new RenderOrderSelector ();
			int order1 = selector.Compare(o1, o2);
			return order1 > 0;
		}

		private bool objExists(float? z, float? spriteZ)
		{
			if (z.HasValue && spriteZ.HasValue) return true;
			if (!z.HasValue && !spriteZ.HasValue) return false;
			Assert.Fail("Bad input for test");
			return false;
		}

		private IObject getObject(int? renderLayerZ, float? z, float? spriteZ, IObject parent)
		{
			if (!objExists(z, spriteZ)) return null;
		    Assert.IsNotNull(z);
		    Assert.IsNotNull(spriteZ);
			return getObject(renderLayerZ, z.Value, spriteZ.Value, parent);
		}

		private IObject getObject(int? renderLayerZ, float z, float spriteZ, IObject parent)
		{
			Mock<ISprite> sprite = new Mock<ISprite> ();
			sprite.Setup(s => s.Z).Returns(spriteZ);

			Mock<ITreeNode<IObject>> tree = new Mock<ITreeNode<IObject>> ();
			tree.Setup(t => t.Parent).Returns(parent);

			IRenderLayer layer = null;
			if (renderLayerZ.HasValue)
			{
				Mock<IRenderLayer> layerMock = new Mock<IRenderLayer> ();
				layerMock.Setup(l => l.Z).Returns(renderLayerZ.Value);
				layer = layerMock.Object;
			}

			Mock<IObject> obj = new Mock<IObject> ();
			obj.Setup(o => o.Z).Returns(z);
			obj.Setup(o => o.CurrentSprite).Returns(sprite.Object);
			obj.Setup(o => o.TreeNode).Returns(tree.Object);
			obj.Setup(o => o.RenderLayer).Returns(layer);
            obj.Setup(o => o.Properties).Returns(new AGSCustomProperties());

			tree.Setup(t => t.Node).Returns(obj.Object);

			return obj.Object;
		}
	}
}

