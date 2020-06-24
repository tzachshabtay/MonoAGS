using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSDefaultsSettings : IDefaultsSettings
    {
        public AGSDefaultsSettings(IDefaultFonts fonts, IDialogSettings dialog)
        {
            Fonts = fonts;
            Dialog = dialog;
        }

        [Property(ForceReadonly = true)]
        public IDefaultFonts Fonts { get; set; }

        public ISkin Skin { get; set; }

        [Property(ForceReadonly = true)]
        public IMessageBoxSettings MessageBox { get; set; }

        [Property(ForceReadonly = true)]
        public IDialogSettings Dialog { get; set; }
    }
}
