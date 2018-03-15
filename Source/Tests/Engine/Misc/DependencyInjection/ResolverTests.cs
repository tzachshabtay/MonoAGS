using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ResolverTests
    {
        private class TestClass : IDisposable
        {
            public void Dispose()
            {
            }
        }

        [Test]
        public async Task ResolverMemoryLeakOutsideObjectTest()
        {
            await testAllocation<TestClass>();
        }

        [Test]
        public async Task ResolverMemoryLeakComponentInterfaceTest()
        {
            await testAllocation<ITranslateComponent>();
        }

        [Test]
        public async Task ResolverMemoryLeakComponentImplementationTest()
        {
            await testAllocation<AGSTranslateComponent>();
        }

        [Test]
        public async Task ResolverMemoryLeakInterfaceTest()
        {
            AGSGame.UIThreadID = Environment.CurrentManagedThreadId;
            await testAllocation<ITexture>();
        }

        [Test]
        public async Task ResolverMemoryLeakImplementationTest()
        {
            AGSGame.UIThreadID = Environment.CurrentManagedThreadId;
            await testAllocation<GLTexture>();
        }

        private async Task testAllocation<TObj>() where TObj : class
        {
            Mocks mocks = new Mocks();
            Resolver resolver = Mocks.GetResolver();
            resolver.Build();

            var myReference = resolver.Container.Resolve<TObj>();

            //https://stackoverflow.com/questions/11417283/strange-weakreference-behavior-on-mono
            var weakRef = alloc(myReference);
            await Task.Delay(5);

            myReference = null;

            GC.Collect(2, GCCollectionMode.Forced, true, true);

            Assert.IsFalse(weakRef.IsAlive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static WeakReference alloc(object o)
        {
            return new WeakReference(o);
        }
    }
}
