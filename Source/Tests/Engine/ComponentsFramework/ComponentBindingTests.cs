using System;
using AGS.API;
using AGS.Engine;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ComponentBindingTests
    {
        [Test]
        public void BindingCalledForExplicitType()
        {
            AGSEmptyEntity entity = new AGSEmptyEntity("test", Mocks.GetResolver());
            AGSCropSelfComponent crop = new AGSCropSelfComponent();
            bool bindingCalled = false;
            entity.Bind<ICropSelfComponent>(c =>
            {
                Assert.AreSame(crop, c);
                bindingCalled = true;
            }, _ => Assert.Fail("Component was somehow removed"));
            Assert.IsFalse(bindingCalled);
            entity.AddComponent<ICropSelfComponent>(crop);
            Assert.IsTrue(bindingCalled);
        }

        [Test]
        public void BindingCalledForExplicitTypeAlreadyAdded()
        {
            AGSEmptyEntity entity = new AGSEmptyEntity("test" + Guid.NewGuid(), Mocks.GetResolver());
            AGSCropSelfComponent crop = new AGSCropSelfComponent();
            entity.AddComponent<ICropSelfComponent>(crop);
            bool bindingCalled = false;
            entity.Bind<ICropSelfComponent>(c =>
            {
                Assert.AreSame(crop, c);
                bindingCalled = true;
            }, _ => Assert.Fail("Component was somehow removed"));
            Assert.IsTrue(bindingCalled);
        }
    }
}
