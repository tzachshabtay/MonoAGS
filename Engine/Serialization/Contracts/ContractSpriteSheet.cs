using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractSpriteSheet : IContract<ISpriteSheet>
	{
		public ContractSpriteSheet()
		{
		}

		[ProtoMember(1)]
		public int CellWidth { get; set; }

		[ProtoMember(2)]
		public int CellHeight { get; set; }

		[ProtoMember(3)]
		public SpriteSheetOrder Order { get; set; }

		[ProtoMember(4)]
		public int StartFromCell { get; set; }

		[ProtoMember(5)]
		public int CellsToGrab { get; set; }

		[ProtoMember(6)]
		public string Path { get; set; }

		#region IContract implementation

		public ISpriteSheet ToItem(AGSSerializationContext context)
		{
			AGSSpriteSheet sheet = new AGSSpriteSheet (Path, CellWidth, CellHeight, StartFromCell, CellsToGrab, Order);
			return sheet;
		}

		public void FromItem(AGSSerializationContext context, ISpriteSheet item)
		{
			CellWidth = item.CellWidth;
			CellHeight = item.CellHeight;
			Order = item.Order;
			StartFromCell = item.StartFromCell;
			CellsToGrab = item.CellsToGrab;
			Path = item.Path;
		}

		#endregion
	}
}

