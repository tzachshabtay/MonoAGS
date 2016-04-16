using System;
using ProtoBuf;
using AGS.API;


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

		#region IContract implementation

		public IBorderStyle ToItem(AGSSerializationContext context)
		{
			AGSBorderStyle style = new AGSBorderStyle ();
			style.Color = Color.FromHexa(ColorValue);
			style.LineWidth = LineWidth;

			return style;
		}

		public void FromItem(AGSSerializationContext context, IBorderStyle item)
		{
			ColorValue = item.Color.Value;
			LineWidth = item.LineWidth;
		}

		#endregion
	}
}

