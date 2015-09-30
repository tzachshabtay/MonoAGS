using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSInventoryItem : IInventoryItem
	{
		public AGSInventoryItem()
		{
			Qty = 1f;
		}

		public static IInventoryItem FromRoomObject(IObject obj)
		{
			obj.IgnoreViewport = false;
			obj.IgnoreScalingArea = false;
			obj.RenderLayer = AGSLayers.UI;
			obj.Anchor = new AGSPoint (0.5f, 0.5f);
			return new AGSInventoryItem { CursorGraphics = obj, Graphics = obj };
		}

		#region IInventoryItem implementation

		public IObject Graphics { get; set; }

		public IObject CursorGraphics { get; set; }

		public float Qty { get; set; }

		#endregion
	}
}

