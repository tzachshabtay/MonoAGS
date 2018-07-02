using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
	public class AGSCursor : IAGSCursor
    {
        public IObject Cursor { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}