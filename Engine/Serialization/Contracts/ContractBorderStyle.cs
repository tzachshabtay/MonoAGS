using System;
using ProtoBuf;
using AGS.API;
using System.Drawing;

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
		public Color Color { get; set; }

		#region IContract implementation

		public IBorderStyle ToItem(AGSSerializationContext context)
		{
			AGSBorderStyle style = new AGSBorderStyle ();
			style.Color = (AGSColor)Color;
			style.LineWidth = LineWidth;

			return style;
		}

		public void FromItem(AGSSerializationContext context, IBorderStyle item)
		{
			Color = Color.FromArgb(item.Color.A, item.Color.R, item.Color.G, item.Color.B);
			LineWidth = item.LineWidth;
		}

		#endregion
	}
}

