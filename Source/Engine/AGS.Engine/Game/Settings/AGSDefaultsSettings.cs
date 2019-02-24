using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSDefaultsSettings : IDefaultsSettings
    {
        public IFont SpeechFont { get; set; } = AGSGame.Device.FontLoader.LoadFont(null, 10f);

        public IFont TextFont { get; set; }  = AGSGame.Device.FontLoader.LoadFont(null, 14f);

        public ISkin Skin { get; set; }

        [Property(ForceReadonly = true)]
        public IMessageBoxSettings MessageBox { get; set; }
    }
}