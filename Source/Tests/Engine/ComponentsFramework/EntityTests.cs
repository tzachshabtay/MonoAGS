using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EntityTests
    {
        public interface IOnlyOneComponent : IComponent 
        {
            void AssertOne();
        }

        private class OnlyOneComponent : AGSComponent, IOnlyOneComponent
        {
            public static int LastID;
            private int _id;

            public OnlyOneComponent()
            {
                _id = LastID++;
            }

            public void AssertOne()
            {
                Assert.AreEqual(LastID, 1);
            }
        }

        [Test]
        public async Task CanOnlyAddOneComponentTest()
        {
            Resolver resolver = Mocks.GetResolver();
            resolver.Builder.RegisterType<OnlyOneComponent>().As<IOnlyOneComponent>();
            resolver.Build();
            for (int i = 0; i < 100; i++)
            {
                OnlyOneComponent.LastID = 0;
                AGSEmptyEntity entity = new AGSEmptyEntity("Test" + Guid.NewGuid(), resolver);

                List<Task> tasks = new List<Task>();
                for (int j = 0; j < 10; j++)
                {
                    tasks.Add(Task.Run(() => entity.AddComponent<IOnlyOneComponent>()));
                }
                await Task.WhenAll(tasks);
                var component = entity.AddComponent<IOnlyOneComponent>();

                component.AssertOne();
            }
        }
    }
}
