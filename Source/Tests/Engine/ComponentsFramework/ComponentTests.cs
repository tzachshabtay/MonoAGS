using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ComponentTests
    {
        [Test]
        public void ComponentPropertyChangedSubscription()
        {
            AGSSprite component = new AGSSprite(Mocks.GetResolver(), new Mock<IMaskLoader>().Object);
            Assert.AreEqual(0, component.GetPropertyChangedSubscriberCount());

            component.PropertyChanged += onComponentPropertyChanged;
            Assert.AreEqual(1, component.GetPropertyChangedSubscriberCount());

            component.PropertyChanged -= onComponentPropertyChanged;
            Assert.AreEqual(0, component.GetPropertyChangedSubscriberCount());
        }

        private void onComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}
