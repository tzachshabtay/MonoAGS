using System.Collections.Generic;
using System.Reflection;

namespace AGS.API
{
    public static class NamedColorsMap
    {
        public static Dictionary<string, uint> NamedColors = new Dictionary<string, uint>(140);
        public static Dictionary<uint, string> NamedColorsReversed = new Dictionary<uint, string>(140);

        static NamedColorsMap()
        {
            foreach (var field in typeof(Colors).GetTypeInfo().DeclaredFields)
            {
                Color color = (Color)field.GetValue(null);
                NamedColors[field.Name] = color.Value;
                NamedColorsReversed[color.Value] = field.Name;
            }
        }
    }
}
