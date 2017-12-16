using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSHasImage : IHasImage
    {
        public AGSHasImage()
        {
            Pivot = new PointF(0.5f, 0f);
            Tint = Colors.White;
        }

        public byte Opacity
        {
            get => Tint.A;
            set => Tint = Color.FromArgb(value, Tint.R, Tint.G, Tint.B);
        }

        public Color Tint { get; set; }

        public PointF Pivot { get; set; }

        public IImage Image { get; set; }

        public IImageRenderer CustomRenderer { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public override string ToString() => Image == null ? base.ToString() : Image.ToString();
    }
}
