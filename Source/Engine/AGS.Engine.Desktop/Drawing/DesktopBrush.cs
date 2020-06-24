using AGS.API;
using System.Drawing;

namespace AGS.Engine.Desktop
{
    [PropertyFolder]
    [ConcreteImplementation(Browsable = false)]
	public class DesktopBrush : IBrush
	{
		public DesktopBrush(Brush brush)
		{
			InnerBrush = brush;
		}

        [Property(Browsable = false)]
		public Brush InnerBrush { get; private set; }

		public static DesktopBrush Solid(API.Color color)
		{
			DesktopBrush brush = new DesktopBrush (new SolidBrush (color.Convert()));
			brush.Type = BrushType.Solid;
			brush.Color = color;
			return brush;
		}

		public static DesktopBrush Solid(System.Drawing.Color color)
		{
			DesktopBrush brush = new DesktopBrush (new SolidBrush (color));
			brush.Type = BrushType.Solid;
			brush.Color = color.Convert();
			return brush;
		}

		#region IBrush implementation

		public BrushType Type { get; private set; }

		public API.Color Color { get; private set; }

		public IBlend Blend => default;

	    public bool GammaCorrection => default;

	    public IColorBlend InterpolationColors => default;

	    public API.Color[] LinearColors => default;

	    public ITransformMatrix Transform => default;

	    public WrapMode WrapMode => default;

	    public API.Color BackgroundColor => default;

	    public HatchStyle HatchStyle => default;

	    public API.PointF CenterPoint => default;

		public API.PointF FocusScales => default;

        #endregion

        public override string ToString()
        {
            if (Type == BrushType.Solid) return Color.ToString();
            return Type.ToString();
        }
    }
}
