using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSInventoryItem : IInventoryItem
	{
		public AGSInventoryItem()
		{
			Qty = 1f;
		}

		#region IInventoryItem implementation

		public IObject Graphics { get; set; }

		public IObject CursorGraphics { get; set; }

		public float Qty { get; set; }

		public bool ShouldInteract { get; set; }

		#endregion

		public override string ToString()
		{
			return string.Format("Inventory Item: {0}", Graphics.ID ?? Graphics.Hotspot ?? Graphics.ToString());
		}
	}
}

