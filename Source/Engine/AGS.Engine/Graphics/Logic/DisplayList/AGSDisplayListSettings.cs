using System.ComponentModel;
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

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
