using System;
using AGS.API;
using Autofac;

using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		public AGSGameFactory(IGraphicsFactory graphics, IInventoryFactory inventory, IUIFactory ui,
			IRoomFactory room, IOutfitFactory outfit, IObjectFactory obj, IDialogFactory dialog, IAudioFactory sound)
		{
			Graphics = graphics;
			Inventory = inventory;
			UI = ui;
			Room = room;
			Outfit = outfit;
			Object = obj;
			Dialog = dialog;
			Sound = sound;
		}

		#region IGameFactory implementation

		public IInventoryFactory Inventory { get; private set; }

		public IUIFactory UI { get; private set; }

		public IObjectFactory Object { get; private set; }

		public IRoomFactory Room { get; private set; }

		public IOutfitFactory Outfit { get; private set; }

		public IDialogFactory Dialog { get; private set; }

		public IGraphicsFactory Graphics { get; private set; }

		public IAudioFactory Sound { get; private set; }

		#endregion


	}
}

