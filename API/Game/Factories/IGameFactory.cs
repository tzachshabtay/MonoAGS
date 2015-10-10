using System;
using System.Drawing;

namespace AGS.API
{
	public interface IGameFactory
	{
		IGraphicsFactory Graphics { get; }
		ISoundFactory Sound { get; }
		IInventoryFactory Inventory { get; }
		IUIFactory UI { get; }
		IObjectFactory Object { get; }
		IRoomFactory Room { get; }
		IOutfitFactory Outfit { get; }
		IDialogFactory Dialog { get; }

		int GetInt(string name, int defaultValue = 0);
		float GetFloat(string name, float defaultValue = 0f);
		string GetString(string name, string defaultValue = null);

		void RegisterCustomData(ICustomSerializable customData);
	}
}

