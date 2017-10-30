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
            entity.AddComponent(crop);
            Assert.IsTrue(bindingCalled);
        }
    }
}
