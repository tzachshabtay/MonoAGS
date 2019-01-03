using System;
using System.Collections.Concurrent;
using AGS.API;

namespace AGS.Engine
{
    public class GLTextureCache : ITextureCache
    {
        private readonly ConcurrentDictionary<string, ITexture> _textures;

        public GLTextureCache()
        {
            _textures = new ConcurrentDictionary<string, ITexture>();
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
            _textures.TryRemove(id, out _);
        }
    }
}
