using System;
using ProtoBuf;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractTextConfig : IContract<ITextConfig>
	{
		public ContractTextConfig()
		{
		}

		[ProtoMember(1)]
		public Brush Brush { get; set; }

		[ProtoMember(2)]
		public Font Font { get; set; }

		[ProtoMember(3)]
		public Alignment Alignment { get; set; }

		[ProtoMember(4)]
		public Brush OutlineBrush { get; set; }

		[ProtoMember(5)]
		public float OutlineWidth { get; set; }

		[ProtoMember(6)]
		public Brush ShadowBrush  { get; set; }

		[ProtoMember(7)]
		public float ShadowOffsetX { get; set; }

		[ProtoMember(8)]
		public float ShadowOffsetY { get; set; }

		[ProtoMember(9)]
		public AutoFit AutoFit { get; set; }

		[ProtoMember(10)]
		public float PaddingLeft { get; set; }

		[ProtoMember(11)]
		public float PaddingRight { get; set; }

		[ProtoMember(12)]
		public float PaddingTop { get; set; }

		[ProtoMember(13)]
		public float PaddingBottom { get; set; }

		#region IContract implementation

		public ITextConfig ToItem(AGSSerializationContext context)
		{
			AGSTextConfig config = new AGSTextConfig (new AGSBrush (Brush), new AGSFont (Font), new AGSBrush (OutlineBrush), OutlineWidth,
				                       new AGSBrush (ShadowBrush), ShadowOffsetX, ShadowOffsetY, Alignment, AutoFit, PaddingLeft,
				                       PaddingRight, PaddingTop, PaddingBottom);
			return config;
		}

		public void FromItem(AGSSerializationContext context, ITextConfig item)
		{
			Brush = ((AGSBrush)item.Brush).InnerBrush;
			Font = ((AGSFont)item.Font).InnerFont;
			OutlineBrush = item.OutlineBrush == null ? null : ((AGSBrush)item.OutlineBrush).InnerBrush;
			OutlineWidth = item.OutlineWidth;
			ShadowBrush = item.ShadowBrush == null ? null : ((AGSBrush)item.ShadowBrush).InnerBrush;
			ShadowOffsetX = item.ShadowOffsetX;
			ShadowOffsetY = item.ShadowOffsetY;
			Alignment = item.Alignment;
			AutoFit = item.AutoFit;
			PaddingLeft = item.PaddingLeft;
			PaddingRight = item.PaddingRight;
			PaddingTop = item.PaddingTop;
			PaddingBottom = item.PaddingBottom;
		}

		#endregion
	}
}

