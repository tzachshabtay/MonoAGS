using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        public AGSHasImage()
        {
            Anchor = new PointF(0.5f, 0f);
            Tint = Colors.White;
        }

        public byte Opacity
        {
            get { return Tint.A; }
            set { Tint = Color.FromArgb(value, Tint.R, Tint.G, Tint.B); }
        }

        public Color Tint { get; set; }

        public PointF Anchor { get; set; }

        public IImage Image { get; set; }

        public IImageRenderer CustomRenderer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Image == null ? base.ToString() : Image.ToString();
        }
    }
}
