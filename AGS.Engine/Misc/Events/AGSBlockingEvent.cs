using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSBlockingEvent<TEventArgs> : IBlockingEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly Guid _id;

		public AGSBlockingEvent()
		{
			_id = Guid.NewGuid();
		}

		public event EventHandler<TEventArgs> InnerEvent;

		#region IBlockingEvent implementation

		public void Subscribe(Action<object, TEventArgs> callback)
		{
			InnerEvent += (sender, e) => callback(sender, e);
		}

		public void Unsubscribe(Action<object, TEventArgs> callback)
		{
			InnerEvent -= (sender, e) => callback(sender, e);
		}

		public void Invoke(object sender, TEventArgs args)
		{
			var innerEvent = InnerEvent;
			if (innerEvent == null) return;
			if (args != null)
				args.TimesInvoked = Repeat.Do(_id.ToString());
			innerEvent(sender, args);
		}

		public int SubscribersCount
		{
			get
			{
				return InnerEvent.GetInvocationList().Length;
			}
		}

		#endregion


	}
}

