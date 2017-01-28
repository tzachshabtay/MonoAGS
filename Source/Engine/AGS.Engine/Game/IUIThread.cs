using System;

namespace AGS.Engine
{
    public interface IUIThread
    {
        void RunBlocking(Action action);
    }
}
