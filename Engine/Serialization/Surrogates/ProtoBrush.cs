using System;
using ProtoBuf;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AGS.Engine
{
	public enum BrushType
	{
		Solid,
		Linear,
		Hatch,
		Path,
		Texture,
	};

	[ProtoContract]
	public class ProtoBrush
	{
		[ProtoMember(1)]
		public BrushType Type { get; set; }

		[ProtoMember(2)]
		public Color Color { get; set; }

		[ProtoMember(3)]
		public Blend Blend { get; set; }

		[ProtoMember(4)]
		public bool GammaCorrection { get; set; }

		[ProtoMember(5)]
		public ColorBlend InterpolationColors { get; set; }

		[ProtoMember(6)]
		public Color[] LinearColors { get; set; }

		[ProtoMember(7)]
		public Matrix Transform { get; set; }

		[ProtoMember(8)]
		public WrapMode WrapMode { get; set; }

		[ProtoMember(9)]
		public Color BackgroundColor { get; set; }

		[ProtoMember(10)]
		public HatchStyle HatchStyle { get; set; }

		[ProtoMember(11)]
		public PointF CenterPoint { get; set; }

		[ProtoMember(12)]
		public PointF FocusScales { get; set; }

		public static implicit operator Brush(ProtoBrush c)
		{
			switch (c.Type)
			{
				case BrushType.Solid:
					return loadSolid(c);
				case BrushType.Linear:
					return loadLinear(c);
				case BrushType.Path:
					return loadPathGradient(c);
				case BrushType.Hatch:
					return loadHatch(c);
				case BrushType.Texture:
					return loadTexture(c);
				default:
					throw new NotSupportedException (c.Type.ToString());
			}
		}

		public static implicit operator ProtoBrush(Brush b)
		{ 
			return saveBrush<SolidBrush>(b as SolidBrush, saveSolid) ??
				saveBrush<LinearGradientBrush>(b as LinearGradientBrush, saveLinear) ??
				saveBrush<HatchBrush>(b as HatchBrush, saveHatch) ??
				saveBrush<PathGradientBrush>(b as PathGradientBrush, savePathGradient) ??
				saveBrush<TextureBrush>(b as TextureBrush, saveTexture);
		}	

		private static ProtoBrush saveBrush<TBrush>(TBrush brush, Func<TBrush, ProtoBrush> saveBrush)
		{
			if (brush == null) return null;
			return saveBrush(brush);
		}

		private static ProtoBrush saveSolid(SolidBrush brush)
		{
			return new ProtoBrush 
			{ 
				Type = BrushType.Solid,
				Color = brush.Color,
			};
		}

		private static Brush loadSolid(ProtoBrush brush)
		{
			return new SolidBrush (brush.Color);
		}

		private static ProtoBrush saveLinear(LinearGradientBrush brush)
		{
			return new ProtoBrush 
			{ 
				Type = BrushType.Linear,
				Blend = brush.Blend, 
				GammaCorrection = brush.GammaCorrection,
				InterpolationColors = brush.InterpolationColors,
				LinearColors = brush.LinearColors,
				Transform = brush.Transform,
				WrapMode = brush.WrapMode,
			};
		}

		private static Brush loadLinear(ProtoBrush brush)
		{
			LinearGradientBrush g = new LinearGradientBrush (new Point (), new Point (), Color.White, Color.White);
			g.Blend = brush.Blend;
			g.GammaCorrection = brush.GammaCorrection;
			g.InterpolationColors = brush.InterpolationColors;
			g.LinearColors = brush.LinearColors;
			g.Transform = brush.Transform;
			g.WrapMode = brush.WrapMode;
			return g;
		}

		private static ProtoBrush saveHatch(HatchBrush brush)
		{
			return new ProtoBrush
			{
				Type = BrushType.Hatch,
				Color = brush.ForegroundColor,
				BackgroundColor = brush.BackgroundColor,
				HatchStyle = brush.HatchStyle,
			};
		}

		private static Brush loadHatch(ProtoBrush brush)
		{
			return new HatchBrush (brush.HatchStyle, brush.Color, brush.BackgroundColor);
		}

		private static ProtoBrush savePathGradient(PathGradientBrush brush)
		{
			return new ProtoBrush
			{
				Type = BrushType.Path,
				Blend = brush.Blend,
				Color = brush.CenterColor,
				CenterPoint = brush.CenterPoint,
				FocusScales = brush.FocusScales,
				LinearColors = brush.SurroundColors,
				InterpolationColors = brush.InterpolationColors,
				Transform = brush.Transform,
				WrapMode = brush.WrapMode,
			};
		}

		private static Brush loadPathGradient(ProtoBrush brush)
		{
			PathGradientBrush g = new PathGradientBrush (new Point[]{ });
			g.Blend = brush.Blend;
			g.CenterColor = brush.Color;
			g.CenterPoint = brush.CenterPoint;
			g.FocusScales = brush.FocusScales;
			g.SurroundColors = brush.LinearColors;
			g.InterpolationColors = brush.InterpolationColors;
			g.Transform = brush.Transform;
			g.WrapMode = brush.WrapMode;
			return g;
		}


		private static ProtoBrush saveTexture(TextureBrush brush)
		{
			throw new NotSupportedException ("Texture brush is not supported!"); //todo: support texture brush

			/*return new ProtoBrush
			{
				Type = BrushType.Texture,
				Transform = brush.Transform,
				WrapMode = brush.WrapMode,
				Image = brush.Image,
			};*/
		}

		private static Brush loadTexture(ProtoBrush brush)
		{
			throw new NotSupportedException ("Texture brush is not supported!"); //todo: support texture brush

			/*TextureBrush t = new TextureBrush (brush.Image);
			t.Transform = brush.Transform;
			t.WrapMode = brush.WrapMode;
			return t;*/
		}
	}
}

