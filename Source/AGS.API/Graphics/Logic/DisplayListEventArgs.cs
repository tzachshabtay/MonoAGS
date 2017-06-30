using System.Collections.Generic;

namespace AGS.API
{
    public class DisplayListEventArgs : AGSEventArgs
    {
        public DisplayListEventArgs(List<IObject> displayList)
        {
            DisplayList = displayList;
        }

        public List<IObject> DisplayList { get; set; }
    }
}
