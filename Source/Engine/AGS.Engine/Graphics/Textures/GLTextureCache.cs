using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class GLTextureCache : ITextureCache
    {
        private readonly Dictionary<string, ITexture> _textures;

        public GLTextureCache()
        {
            _textures = new Dictionary<string, ITexture>(1024);
        }

        public ITexture GetTexture(string id, Func<string, ITexture> factory)
        {
            return _textures.GetOrAdd(id, factory);
        }

        public bool TryGetTexture(string id, out ITexture texture)
        {
            return _textures.TryGetValue(id, out texture);
        }

        public void RemoveTexture(string id)
        {
            _textures.Remove(id);
        }
    }
}
