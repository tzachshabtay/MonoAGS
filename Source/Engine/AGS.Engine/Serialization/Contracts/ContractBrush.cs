using System;
using System.Linq;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractBrush : IContract<IBrush>
	{
        private IBrushLoader _brushes { get { return AGSGame.Device.BrushLoader; } }

		[ProtoMember(1)]
		public BrushType Type { get; set; }

		[ProtoMember(2)]
		public uint MainColor { get; set; }

		[ProtoMember(3)]
		public IContract<IBlend> Blend { get; set; }

		[ProtoMember(4)]
		public bool GammaCorrection { get; set; }

		[ProtoMember(5)]
		public IContract<IColorBlend> InterpolationColors { get; set; }

		[ProtoMember(6)]
		public uint[] LinearColors { get; set; }

		[ProtoMember(7)]
		public IContract<ITransformMatrix> Transform { get; set; }

		[ProtoMember(8)]
		public WrapMode WrapMode { get; set; }

		[ProtoMember(9)]
		public uint BackgroundColor { get; set; }

		[ProtoMember(10)]
		public HatchStyle HatchStyle { get; set; }

		[ProtoMember(11)]
		public IContract<PointF> CenterPoint { get; set; }

		[ProtoMember(12)]
		public IContract<PointF> FocusScales { get; set; }

		#region IContract implementation

		public IBrush ToItem(AGSSerializationContext context)
		{
			switch (Type)
			{
				case BrushType.Solid:
					return _brushes.LoadSolidBrush(Color.FromHexa(MainColor));
				case BrushType.Linear:
					return _brushes.LoadLinearBrush(LinearColors.Select(c => Color.FromHexa(c)).ToArray(), 
						Blend.ToItem(context), InterpolationColors.ToItem(context), Transform.ToItem(context), WrapMode, GammaCorrection);
				case BrushType.Hatch:
					return _brushes.LoadHatchBrush(HatchStyle, Color.FromHexa(MainColor), Color.FromHexa(BackgroundColor));
				case BrushType.Path:
					return _brushes.LoadPathsGradientBrush(Color.FromHexa(MainColor), CenterPoint.ToItem(context), Blend.ToItem(context), 
						FocusScales.ToItem(context), LinearColors.Select(c => Color.FromHexa(c)).ToArray(), 
						InterpolationColors.ToItem(context), Transform.ToItem(context), WrapMode);
				default:
					throw new NotSupportedException (Type + " brush is not supported!");
			}
		}

		public void FromItem(AGSSerializationContext context, IBrush item)
		{
			Type = item.Type;
			MainColor = item.Color.Value;
			Blend = context.GetContract(item.Blend);
			GammaCorrection = item.GammaCorrection;
			InterpolationColors = context.GetContract(item.InterpolationColors);
			LinearColors = item.LinearColors == null ? null : item.LinearColors.Select(c => c.Value).ToArray();
			Transform = context.GetContract(item.Transform);
			WrapMode = item.WrapMode;
			BackgroundColor = item.BackgroundColor.Value;
			HatchStyle = item.HatchStyle;
			CenterPoint = context.GetContract(item.CenterPoint);
			FocusScales = context.GetContract(item.FocusScales);
		}

		#endregion
	}
}

