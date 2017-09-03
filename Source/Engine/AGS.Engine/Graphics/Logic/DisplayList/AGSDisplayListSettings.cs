using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayListSettings : IDisplayListSettings
    {
        public AGSDisplayListSettings()
        {
            DisplayRoom = DisplayGUIs = true;
            RestrictionList = new AGSRestrictionList();
            DepthClipping = new AGSDepthClipping();
        }

        public bool DisplayRoom { get; set; }
        public bool DisplayGUIs { get; set; }
        public IRestrictionList RestrictionList { get; private set; }
        public IDepthClipping DepthClipping { get; private set; }
    }
}
