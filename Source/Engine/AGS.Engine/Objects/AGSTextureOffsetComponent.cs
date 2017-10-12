using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTextureOffsetComponent : AGSComponent, ITextureOffsetComponent
    {
        private PointF _textureOffset;

        public AGSTextureOffsetComponent(IEvent onTextureOffsetChanged)
        {
            OnTextureOffsetChanged = onTextureOffsetChanged;
        }

        public PointF TextureOffset 
        { 
            get { return _textureOffset; } 
            set 
            {
                if (_textureOffset.Equals(value)) return;
                _textureOffset = value;
                OnTextureOffsetChanged.Invoke();
            }
        }

        public IEvent OnTextureOffsetChanged { get; private set; }
    }
}
