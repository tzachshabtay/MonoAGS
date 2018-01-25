using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSSpriteRenderComponent : AGSComponent, ISpriteRenderComponent
    {
        private ISpriteProvider _provider;

        public AGSSpriteRenderComponent()
        {
        }

        public ISprite CurrentSprite { get => _provider?.Sprite; }

        public ISpriteProvider SpriteProvider
        {
            get => _provider;
            set
            {
                var previousProvider = _provider;
                if (previousProvider != null)
                    previousProvider.PropertyChanged -= OnProviderPropertyChanged;
                if (value != null)
                    value.PropertyChanged += OnProviderPropertyChanged;
                _provider = value;
                OnPropertyChanged(nameof(CurrentSprite));
            }
        }

        public bool DebugDrawPivot { get; set; }

        public IBorderStyle Border { get; set; }

        private void OnProviderPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ISpriteProvider.Sprite))
                return;
            // resend property changed event to notify that ISpriteRenderComponent.CurrentSprite has new value
            OnPropertyChanged(nameof(CurrentSprite));
        }
    }
}
