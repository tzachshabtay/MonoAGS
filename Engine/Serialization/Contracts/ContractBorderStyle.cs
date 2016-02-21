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
			style.Color = Color;
			style.LineWidth = LineWidth;

			return style;
		}

		public void FromItem(AGSSerializationContext context, IBorderStyle item)
		{
			Color = item.Color;
			LineWidth = item.LineWidth;
		}

		#endregion
	}
}

