using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayListSettings : IDisplayListSettings
    {
        public AGSDisplayListSettings()
        {
            DisplayRoom = DisplayGUIs = true;
        }

        public bool DisplayRoom { get; set; }
        public bool DisplayGUIs { get; set; }
    }
}
