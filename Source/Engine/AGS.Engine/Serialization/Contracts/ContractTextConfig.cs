using System;
using ProtoBuf;
using AGS.API;


namespace AGS.Engine
{
	[ProtoContract]
	public class ContractTextConfig : IContract<ITextConfig>
	{
		public ContractTextConfig()
		{
		}

		[ProtoMember(1)]
		public IContract<IBrush> Brush { get; set; }

		[ProtoMember(2)]
		public IContract<IFont> Font { get; set; }

		[ProtoMember(3)]
		public Alignment Alignment { get; set; }

		[ProtoMember(4)]
		public IContract<IBrush> OutlineBrush { get; set; }

		[ProtoMember(5)]
		public float OutlineWidth { get; set; }

		[ProtoMember(6)]
		public IContract<IBrush> ShadowBrush  { get; set; }

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
            var config = context.Factory.Fonts.GetTextConfig(Brush.ToItem(context), Font.ToItem(context), OutlineBrush.ToItem(context), OutlineWidth,
				                       ShadowBrush.ToItem(context), ShadowOffsetX, ShadowOffsetY, Alignment, AutoFit, PaddingLeft,
				                       PaddingRight, PaddingTop, PaddingBottom);
			return config;
		}

		public void FromItem(AGSSerializationContext context, ITextConfig item)
		{
			Brush = context.GetContract(item.Brush);
			Font = context.GetContract(item.Font);
			OutlineBrush = context.GetContract(item.OutlineBrush);
			OutlineWidth = item.OutlineWidth;
			ShadowBrush = context.GetContract(item.ShadowBrush);
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

