using System;
using AGS.API;
using Autofac;
using System.Drawing;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		public AGSGameFactory(IGraphicsFactory graphics, IInventoryFactory inventory, IUIFactory ui,
			IRoomFactory room, IOutfitFactory outfit, IObjectFactory obj, IDialogFactory dialog)
		{
			Graphics = graphics;
			Inventory = inventory;
			UI = ui;
			Room = room;
			Outfit = outfit;
			Object = obj;
			Dialog = dialog;
		}

		#region IGameFactory implementation

		public int GetInt(string name, int defaultValue = 0)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			throw new NotImplementedException();
		}

		public string GetString(string name, string defaultValue = null)
		{
			throw new NotImplementedException();
		}

		public void RegisterCustomData(ICustomSerializable customData)
		{
			throw new NotImplementedException();
		}

		public IInventoryFactory Inventory { get; private set; }

		public IUIFactory UI { get; private set; }

		public IObjectFactory Object { get; private set; }

		public IRoomFactory Room { get; private set; }

		public IOutfitFactory Outfit { get; private set; }

		public IDialogFactory Dialog { get; private set; }

		public IGraphicsFactory Graphics { get; private set; }

		public ISoundFactory Sound
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion


	}
}

