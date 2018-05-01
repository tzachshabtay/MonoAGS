using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public interface IHostingWindow : INotifyPropertyChanged
    {
        Rectangle HostingWindow { get; }
    }
}