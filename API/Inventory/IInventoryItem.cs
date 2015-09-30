using System;

namespace AGS.API
{
	public interface IInventoryItem
	{
		IObject Graphics { get; set; }
		IObject CursorGraphics { get; set; }
		float Qty { get; set; }
	}
}

