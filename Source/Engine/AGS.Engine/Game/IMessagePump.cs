using System;
using System.Threading;

namespace AGS.Engine
{
	public interface IMessagePump
	{
		void PumpMessages();
		void SetSyncContext();
        void Post(SendOrPostCallback d, object state);
	}
}

