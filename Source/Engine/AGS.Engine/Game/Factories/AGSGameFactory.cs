using AGS.API;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		public AGSGameFactory(IGraphicsFactory graphics, IInventoryFactory inventory, IUIFactory ui,
			IRoomFactory room, IOutfitFactory outfit, IObjectFactory obj, IDialogFactory dialog, 
                              IAudioFactory sound, IFontLoader fontFactory)
		{
			Graphics = graphics;
			Inventory = inventory;
			UI = ui;
			Room = room;
			Outfit = outfit;
			Object = obj;
			Dialog = dialog;
			Sound = sound;
            Fonts = fontFactory;
		}

		#region IGameFactory implementation

		public IInventoryFactory Inventory { get; private set; }

		public IUIFactory UI { get; private set; }

        public IFontLoader Fonts { get; private set; }

		public IObjectFactory Object { get; private set; }

		public IRoomFactory Room { get; private set; }

		public IOutfitFactory Outfit { get; private set; }

		public IDialogFactory Dialog { get; private set; }

		public IGraphicsFactory Graphics { get; private set; }

		public IAudioFactory Sound { get; private set; }

		#endregion


	}
}

