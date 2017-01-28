using System.Collections.Concurrent;
using AGS.API;

namespace AGS.Engine
{
    public interface IModalWindows
    {
        ConcurrentStack<IEntity> ModalWindows { get; }
    }
}
