using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayListSettings : IDisplayListSettings
    {
        public AGSDisplayListSettings()
        {
            DisplayRoom = DisplayGUIs = true;
            RestrictionList = new AGSRestrictionList();
        }

        public bool DisplayRoom { get; set; }
        public bool DisplayGUIs { get; set; }
        public IRestrictionList RestrictionList { get; private set; }
    }
}
