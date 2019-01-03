using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public interface IAGSCursor : INotifyPropertyChanged
    {
        IObject Cursor { get; set; }
    }
}