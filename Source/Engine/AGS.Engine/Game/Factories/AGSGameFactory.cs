using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		public AGSGameFactory(IGraphicsFactory graphics, IInventoryFactory inventory, IUIFactory ui,
			IRoomFactory room, IOutfitFactory outfit, IObjectFactory obj, IDialogFactory dialog, 
            IAudioFactory sound, IFontLoader fontFactory, IResourceLoader resources, IShaderFactory shaders, 
            Resolver resolver)
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
            Resources = resources;
            TypedParameter gameFactoryParam = new TypedParameter(typeof(IGameFactory), this);
            Masks = resolver.Container.Resolve<IMaskLoader>(gameFactoryParam);
            Shaders = shaders;
		}

		#region IGameFactory implementation

        public IResourceLoader Resources { get; private set; }

        public IMaskLoader Masks { get; private set; }

        public IShaderFactory Shaders { get; private set; }

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

