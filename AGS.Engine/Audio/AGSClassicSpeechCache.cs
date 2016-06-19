using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSClassicSpeechCache : ISpeechCache
    {
        private IAudioFactory _factory;
        private ConcurrentDictionary<string, IAudioClip> _speechCache;
        private const string _baseFolder = "../../Assets/Speech/English/";

        public AGSClassicSpeechCache(IAudioFactory factory)
        {
            _factory = factory;
            _speechCache = new ConcurrentDictionary<string, IAudioClip>();
        }

        public async Task<ISpeechLine> GetSpeechLineAsync(string characterName, string text)
        {
            string filename = getFileName(characterName, ref text);
            if (filename == null) return new AGSSpeechLine(null, text);
            IAudioClip audioClip;
            if (!_speechCache.TryGetValue(filename, out audioClip))
            {
                audioClip = await _factory.LoadAudioClipAsync(_baseFolder + filename);
                _speechCache.TryAdd(filename, audioClip);
            }
            return new AGSSpeechLine(audioClip, text);
        }

        private string getFileName(string characterName, ref string text)
        {
            if (characterName == null || text == null || !text.StartsWith("&", StringComparison.Ordinal)) return null;
            int index = 1;
            StringBuilder sb = new StringBuilder(characterName.Substring(0, Math.Min(characterName.Length, 4)));
            while (index < text.Length && char.IsDigit(text[index]))
            {
                sb.Append(text[index]);
                index++;
            }
            if (text.Length > index && text[index] == ' ') index++;
            text = text.Substring(index);

            return sb.ToString();
        }
    }
}

