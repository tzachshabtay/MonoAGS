using System.Collections.Generic;
using AGS.API;
using Newtonsoft.Json;

namespace AGS.Editor.Desktop
{
    public class DesktopSerialization : ISerialization
    {
        private readonly IGame _game;

        public DesktopSerialization(IGame game)
        {
            _game = game;
        }

        public static JsonSerializerSettings GetJsonSettings(IGame game)
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                Converters = game == null ? new List<JsonConverter>() : new List<JsonConverter> { new DesktopFontJsonConverter(game) }
            };
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, GetJsonSettings(_game));
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetJsonSettings(_game));
        }
    }
}