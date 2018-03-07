using NUnit.Framework;
using AGS.Engine;
using System.Threading.Tasks;
using AGS.API;

namespace Tests
{
	[TestFixture ()]
	public class EventTests
	{
		private int syncEvents, asyncEvents;
		const int x = 10;

		[SetUp]
		public void Init()
		{
			syncEvents = 0;
			asyncEvents = 0;
		}

		[Test ()]
		public void NoSubscribersTest ()
		{
			AGSEvent ev = new AGSEvent ();
			ev.Invoke();
			Assert.AreEqual (0, syncEvents);
			Assert.AreEqual (0, asyncEvents);
		}

		[Test ()]
		public async Task NoSubscribersAsyncTest ()
		{
			AGSEvent ev = new AGSEvent ();
			await ev.InvokeAsync ();
			Assert.AreEqual (0, syncEvents);
			Assert.AreEqual (0, asyncEvents);
		}

		[Test ()]
		public void SingleSubscriberTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe (onEvent);
			ev.Invoke (new MockEventArgs(x));
			Assert.AreEqual (1, syncEvents);
			Assert.AreEqual (0, asyncEvents);
		}

		[Test ()]
		public void SingleAsyncSubscriberTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
            ev.SubscribeToAsync (onEventAsyncWithBlockingInvoke);
			ev.Invoke (new MockEventArgs(x));
			Assert.AreEqual (0, syncEvents);
			Assert.AreEqual (1, asyncEvents);
		}

		[Test ()]
		public async Task SingleSubscriberAsyncTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe (onEvent);
			await ev.InvokeAsync (new MockEventArgs(x));
			Assert.AreEqual (1, syncEvents);
			Assert.AreEqual (0, asyncEvents);
		}

		[Test ()]
		public async Task SingleAsyncSubscriberAsyncTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.SubscribeToAsync (onEventAsync);
			await ev.InvokeAsync (new MockEventArgs(x));
			Assert.AreEqual (0, syncEvents);
			Assert.AreEqual (1, asyncEvents);
		}

		[Test ()]
		public void MultipleSubscribersTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe (onEvent);
			ev.Subscribe (e => onEvent(e));
            ev.SubscribeToAsync (onEventAsyncWithBlockingInvoke);
            ev.SubscribeToAsync (async e => await onEventAsyncWithBlockingInvoke(e));
			ev.Invoke (new MockEventArgs(x));
			Assert.AreEqual (2, syncEvents);
			Assert.AreEqual (2, asyncEvents);
		}

		[Test ()]
		public async Task MultipleSubscribersAsyncTest ()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe (onEvent);
			ev.Subscribe (e => onEvent(e));
			ev.SubscribeToAsync (onEventAsync);
			ev.SubscribeToAsync (async e => await onEventAsync(e));
			await ev.InvokeAsync (new MockEventArgs(x));
			Assert.AreEqual (2, syncEvents);
			Assert.AreEqual (2, asyncEvents);
		}

		[Test]
		public void SubscribeUnsubscribeTest()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe(onEvent);
			ev.Unsubscribe(onEvent);
			Assert.AreEqual(0, ev.SubscribersCount);
		}

		[Test]
		public void SubscribeUnsubscribeOnDifferentTargetTest()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.Subscribe(onEvent);
			EventTests target2 = new EventTests ();
			ev.Unsubscribe(target2.onEvent);
			Assert.AreEqual(1, ev.SubscribersCount);
		}

		[Test]
		public void SubscribeUnsubscribeAsyncTest()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.SubscribeToAsync(onEventAsync);
			ev.UnsubscribeToAsync(onEventAsync);
			Assert.AreEqual(0, ev.SubscribersCount);
		}

		[Test]
		public void SubscribeUnsubscribeOnDifferentTargetAsyncTest()
		{
			AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs> ();
			ev.SubscribeToAsync(onEventAsync);
			EventTests target2 = new EventTests ();
			ev.UnsubscribeToAsync(target2.onEventAsync);
			Assert.AreEqual(1, ev.SubscribersCount);
		}

        [Test]
        public void ClaimEventTest()
        {
            AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs>();
            ev.Subscribe(onEventClaim, CallbackPriority.High);
            EventTests target2 = new EventTests();
            ev.Subscribe(target2.onEventClaim);
            ev.Invoke(new MockEventArgs(x));
            Assert.AreEqual(2, ev.SubscribersCount);
            Assert.AreEqual(1, syncEvents);
            Assert.AreEqual(0, target2.syncEvents);
        }

        [Test]
        public void DontClaimEventTest()
        {
            AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs>();
            ev.Subscribe(onEventDontClaim);
            EventTests target2 = new EventTests();
            ev.Subscribe(target2.onEventDontClaim);
            ev.Invoke(new MockEventArgs(x));
            Assert.AreEqual(2, ev.SubscribersCount);
            Assert.AreEqual(1, syncEvents);
            Assert.AreEqual(1, target2.syncEvents);
        }

        [Test]
        public void DuplicateSubscribersTest()
        {
            AGSEvent<MockEventArgs> ev = new AGSEvent<MockEventArgs>();
            ev.Subscribe(onEvent);
            ev.Subscribe(onEvent);
            Assert.AreEqual(1, ev.SubscribersCount);
        }

		private void onEvent(MockEventArgs e)
		{
			Assert.AreEqual (x, e.X);
			syncEvents++;
		}

        private void onEventClaim(MockEventArgs e, ref ClaimEventToken token)
        {
            Assert.AreEqual(x, e.X);
            syncEvents++;
            token.Claimed = true;
        }

        private void onEventDontClaim(MockEventArgs e, ref ClaimEventToken token)
        {
            Assert.AreEqual(x, e.X);
            syncEvents++;
        }

		private async Task onEventAsync(MockEventArgs e)
		{
			await Task.Delay (1);
			Assert.AreEqual (x, e.X);
			asyncEvents++;
		}

        private Task onEventAsyncWithBlockingInvoke(MockEventArgs e)
        {
            Assert.AreEqual(x, e.X);
            asyncEvents++;
            return Task.FromResult(true);
        }

		private class MockEventArgs
		{
			public MockEventArgs(int x)
			{
				X = x;
			}

			public int X { get; private set; }
		}
	}
}

