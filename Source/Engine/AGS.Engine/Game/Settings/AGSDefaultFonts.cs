using AGS.API;

namespace AGS.Engine
{
    public class AGSDefaultFonts : IDefaultFonts
    {
        public IFont Speech { get; set; } = AGSGame.Device.FontLoader.LoadFont(null, 10f);
        public IFont Text { get; set; } = AGSGame.Device.FontLoader.LoadFont(null, 14f);
        public IFont Dialogs { get; set; }
    }
}
