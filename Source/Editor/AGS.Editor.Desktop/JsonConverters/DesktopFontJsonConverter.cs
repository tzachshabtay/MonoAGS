using System;
using AGS.API;
using AGS.Engine;
using AGS.Engine.Desktop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGS.Editor.Desktop
{
    //todo: this should have been a json converter for DesktopFont, not for IFont.
    //We can't do this, though, because of a bug in json.net: https://github.com/JamesNK/Newtonsoft.Json/issues/1513
    //In this situation, we won't be able to have multiple converters to multiple implementations of IFont (or other interfaces).
    public class DesktopFontJsonConverter : JsonConverter<IFont>
    {
        private readonly DesktopFontLoader _fontLoader;

        public DesktopFontJsonConverter(IGame game)
        {
            _fontLoader = (game.Resolver.Resolve<IDevice>().FontLoader as DesktopFontLoader) ?? new DesktopFontLoader(new DesktopFontFamilyLoader());
        }

        public override IFont ReadJson(JsonReader reader, Type objectType, IFont existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            float sizeInPoints = obj[nameof(IFont.SizeInPoints)].Value<float>();
            FontStyle style = (FontStyle)obj[nameof(IFont.Style)].Value<int>();
            if ((obj[nameof(DesktopFont.Path)]?.Type ?? JTokenType.Null) == JTokenType.Null)
            {
                return (DesktopFont)_fontLoader.LoadFont(obj[nameof(IFont.FontFamily)].Value<string>(),
                                                         sizeInPoints, style);
            }
            return (DesktopFont)_fontLoader.LoadFontFromPath(obj[nameof(DesktopFont.Path)].Value<string>(), sizeInPoints, style);
        }

        public override void WriteJson(JsonWriter writer, IFont value, JsonSerializer serializer)
        {
            JObject.FromObject(value, JsonSerializer.CreateDefault(DesktopSerialization.GetJsonSettings(null))).WriteTo(writer);
        }
    }
}