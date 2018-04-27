using System;
using AGS.API;
using AGS.Engine;
using Autofac;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class AGSEditor
    {
        public AGSEditor(Resolver editorResolver)
        {
            EditorResolver = editorResolver;
        }

        public IGame Editor { get; set; }
        public IGame Game { get; set; }

        public Resolver EditorResolver { get; }
        public Resolver GameResolver { get; set; }

        public UIEventsAggregator UIEventsAggregator { get; private set; }

        public void Init()
        {
            UIEventsAggregator = new UIEventsAggregator(Editor.Input, Game.HitTest, Editor.Events, Editor.State.FocusedUI);
        }

        public (float, float) ToGameResolution(float x, float y)
        {
            x = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Width, Game.Settings.VirtualResolution.Width, x);
            y = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Height, Game.Settings.VirtualResolution.Height, y);
            return (x, y);
        }

        public (float, float) ToEditorResolution(float x, float y)
        {
            x = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Width, Editor.Settings.VirtualResolution.Width, x);
            y = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Height, Editor.Settings.VirtualResolution.Height, y);
            return (x, y);
        }

        public static void SetupResolver()
        {
            Resolver.Override(resolver => resolver.Builder.RegisterType<KeyboardBindings>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ActionManager>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterAssemblyTypes(typeof(GameLoader).Assembly).
                              Except<InspectorTreeNodeProvider>().Except<EditorShouldBlockEngineInput>().AsImplementedInterfaces().ExternallyOwned());
            Resolver.Override(resolver => { var editor = new AGSEditor(resolver); resolver.Builder.RegisterInstance(editor); });
        }

        public static IEditorPlatform Platform { get; set; }
    }
}