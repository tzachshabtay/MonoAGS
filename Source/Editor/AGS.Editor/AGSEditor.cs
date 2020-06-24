﻿using AGS.API;
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

        public AGSProject Project { get; set; }

        public CanvasHitTest CanvasHitTest { get; set; }

        public void Init()
        {
            UIEventsAggregator = new UIEventsAggregator(Editor.Input, Game.HitTest, Editor.Events, Editor.State.FocusedUI);
        }

        public (float x, float y) ToGameResolution(float x, float y, IDrawableInfoComponent drawable)
        {
            var viewport = Game.State.Viewport;
            x = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Width, Game.Settings.WindowSize.Width, x);
            y = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Height, Game.Settings.WindowSize.Height, y);
            x -= viewport.ScreenArea.X;
            y -= viewport.ScreenArea.Y;
            x = MathUtils.Lerp(0f, 0f, viewport.ScreenArea.Width, Game.Settings.VirtualResolution.Width, x);
            y = MathUtils.Lerp(0f, 0f, viewport.ScreenArea.Height, Game.Settings.VirtualResolution.Height, y);
            if (drawable != null && !drawable.IgnoreViewport)
            {
                var matrix = viewport.GetMatrix(drawable.RenderLayer).Inverted();
                var vec = Vector3.Transform(new Vector3(x, y, 0f), matrix);
                x = vec.X;
                y = vec.Y;
            }
            return (x, y);
        }

        public (float x, float y) ToEditorResolution(float x, float y, IDrawableInfoComponent drawable)
        {
            var viewport = Game.State.Viewport;
            if (drawable != null && !drawable.IgnoreViewport)
            {
                var matrix = viewport.GetMatrix(drawable.RenderLayer);
                var vec = Vector3.Transform(new Vector3(x, y, 0f), matrix);
                x = vec.X;
                y = vec.Y;
            }
            x = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Width, viewport.ScreenArea.Width, x);
            y = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Height, viewport.ScreenArea.Height, y);
            x += viewport.ScreenArea.X;
            y += viewport.ScreenArea.Y;
            x = MathUtils.Lerp(0f, 0f, Game.Settings.WindowSize.Width, Editor.Settings.VirtualResolution.Width, x);
            y = MathUtils.Lerp(0f, 0f, Game.Settings.WindowSize.Height, Editor.Settings.VirtualResolution.Height, y);
            return (x, y);
        }

        public (float, float) ToGameSize(float width, float height)
        {
            width = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Width, Game.Settings.VirtualResolution.Width, width);
            height = MathUtils.Lerp(0f, 0f, Editor.Settings.VirtualResolution.Height, Game.Settings.VirtualResolution.Height, height);
            return (width, height);
        }

        public (float, float) ToEditorSize(float width, float height)
        {
            width = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Width, Editor.Settings.VirtualResolution.Width, width);
            height = MathUtils.Lerp(0f, 0f, Game.Settings.VirtualResolution.Height, Editor.Settings.VirtualResolution.Height, height);
            return (width, height);
        }

        public bool IsEditorPositionInGameWindow(float x, float y)
        {
            (x, y) = ToGameResolution(x, y, null);
            if (x < 0 || x > Game.Settings.VirtualResolution.Width) return false;
            if (y < 0 || y > Game.Settings.VirtualResolution.Height) return false;
            return true;
        }

        public static void SetupResolver()
        {
            Resolver.Override(resolver => resolver.Builder.RegisterType<KeyboardBindings>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ActionManager>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterAssemblyTypes(typeof(GameLoader).Assembly).
                              Except<EditorShouldBlockEngineInput>().Except<EditorUIEvents>().AsImplementedInterfaces().ExternallyOwned());
            Resolver.Override(resolver => { var editor = new AGSEditor(resolver); resolver.Builder.RegisterInstance(editor); });
            Resolver.Override(resolver => resolver.RegisterType<AGSTreeNodeViewProvider, ITreeNodeViewProvider>());
        }

        public static IEditorPlatform Platform { get; set; }
    }
}