using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractFont : IContract<IFont>
	{
		[ProtoMember(1)]
		public string FontFamily { get; set; }

		[ProtoMember(2)]
		public FontStyle Style { get; set; }

		[ProtoMember(3)]
		public float SizeInPoints { get; set; }

		#region IContract implementation

		public IFont ToItem(AGSSerializationContext context)
		{
			return Hooks.FontLoader.LoadFont(FontFamily, SizeInPoints, Style);
		}

		public void FromItem(AGSSerializationContext context, IFont item)
		{
			FontFamily = item.FontFamily;
			Style = item.Style;
			SizeInPoints = item.SizeInPoints;
		}

		#endregion
	}
}

