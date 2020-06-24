using AGS.API;

namespace AGS.Engine
{
    public class AGSSkinComponent : AGSComponent, ISkinComponent
    {
        public AGSSkinComponent(IGameSettings settings)
        {
            Skin = settings.Defaults.Skin;
            SkinTags = new AGSConcurrentHashSet<string>();
        }

        public ISkin Skin { get; set; }

        public IConcurrentHashSet<string> SkinTags { get; private set; }

        public override void Init()
        {
            base.Init();
            Entity.OnComponentsInitialized.Subscribe(onComponentsInitialized);
        }

        private void onComponentsInitialized()
        {
            var skin = Skin;
            if (skin == null) return;
            skin.Apply(Entity);
        }
    }
}
