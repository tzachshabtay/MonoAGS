using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayListSettings : IDisplayListSettings
    {
        public AGSDisplayListSettings(IRestrictionList restrictionList = null, IDepthClipping depthClipping = null)
        {
            DisplayRoom = DisplayGUIs = true;
            RestrictionList = restrictionList ?? new AGSRestrictionList();
            DepthClipping = depthClipping ?? new AGSDepthClipping();
        }

        public bool DisplayRoom { get; set; }
        public bool DisplayGUIs { get; set; }
        public IRestrictionList RestrictionList { get; private set; }
        public IDepthClipping DepthClipping { get; private set; }
    }
}
