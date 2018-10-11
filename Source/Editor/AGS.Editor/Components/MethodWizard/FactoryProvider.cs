using System;
using AGS.API;

namespace AGS.Editor
{
    public static class FactoryProvider
    {
        public static object GetFactory(string factoryType, IGame game)
        {
            var factory = game.Factory;
            switch (factoryType)
            {
                case nameof(IBorderFactory): return factory.Graphics.Borders;
                case nameof(IBrushLoader): return factory.Graphics.Brushes;
                case nameof(IFontLoader): return factory.Fonts;
                case nameof(IInventoryFactory): return factory.Inventory;
                case nameof(IAudioFactory): return factory.Sound;
                case nameof(IFontFactory): return factory.Fonts;
                default:
                    throw new NotSupportedException($"Not supported factory type: {factoryType}");
            }
        }

        public static string GetFactoryScriptName(object factory, IGame game)
        {
            if (factory == null) return null; //constructor
            if (factory == game.Factory.UI) return "_factory.UI";
            if (factory == game.Factory.Object) return "_factory.Object";
            if (factory == game.Factory.Room) return "_factory.Room";
            if (factory == game.Factory.Graphics.Borders) return "_factory.Graphics.Borders";
            if (factory == game.Factory.Graphics.Brushes) return "_factory.Graphics.Brushes";
            if (factory == game.Factory.Fonts) return "_factory.Fonts";
            if (factory == game.Factory.Inventory) return "_factory.Inventory";
            if (factory == game.Factory.Sound) return "_factory.Sound";
            throw new NotSupportedException($"Unsupported factory of type {factory?.GetType().ToString() ?? "null"}");
        }
    }
}