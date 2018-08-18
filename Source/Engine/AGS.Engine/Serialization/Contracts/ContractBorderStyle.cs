using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractBorderStyle : IContract<IBorderStyle>
	{
		public ContractBorderStyle()
		{
		}

		[ProtoMember(1)]
		public float LineWidth { get; set; }

		[ProtoMember(2)]
		public uint ColorValue { get; set; }

		[ProtoMember(3)]
		public bool BorderSupported { get; set; }

		#region IContract implementation

		public IBorderStyle ToItem(AGSSerializationContext context)
		{
			if (!BorderSupported) return null;
			var color = Color.FromHexa(ColorValue);
			var lineWidth = LineWidth;
            IBorderStyle style = context.Factory.Graphics.Borders.SolidColor(color, lineWidth);

			return style;
		}

		public void FromItem(AGSSerializationContext context, IBorderStyle item)
		{
			//todo: support all borders
			AGSColoredBorder border = item as AGSColoredBorder;
			if (border == null) return;
			BorderSupported = true;
			ColorValue = border.Color.BottomLeft.Value;
			LineWidth = border.LineWidth;
		}

		#endregion
	}
}

