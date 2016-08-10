using System;
namespace AGS.Engine
{
	public interface IMessagePump
	{
		void PumpMessages();
		void SetSyncContext();
	}
}

