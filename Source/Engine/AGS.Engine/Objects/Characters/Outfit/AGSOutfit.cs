using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSOutfit : IOutfit
    {
        private readonly ConcurrentDictionary<string, IDirectionalAnimation> _animations;

        public AGSOutfit()
        {
            _animations = new ConcurrentDictionary<string, IDirectionalAnimation>();
        }

        public const string Idle = "Idle";
        public const string Walk = "Walk";
        public const string Speak = "Speak";
        public const string SpeakAndWalk = "SpeakAndWalk";

        #region IOutfit implementation

        public IDirectionalAnimation this[string key] 
        {
            get 
            {
                _animations.TryGetValue(key, out var animation);
                return animation;
            }
            set { _animations[key] = value; }
        }

        public IDictionary<string, IDirectionalAnimation> ToDictionary()
        {
            return _animations.ToDictionary(k => k.Key, v => v.Value);
        }

		#endregion
	}
}