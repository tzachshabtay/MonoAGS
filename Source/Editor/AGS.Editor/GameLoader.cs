using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public static class GameLoader
    {
        private static Lazy<GameDebugView> _gameDebugView;

        private static string _currentFolder;

        private static AppDomain _domain;

        static GameLoader()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += loadFromSameFolder;
        }

        public static async Task<(List<Type> games, Assembly assembly)> GetGames(AGSProject agsProj)
        {
            string path = await getOutputPath(agsProj);
            if (path == null)
            {
                return (new List<Type>(), null);
            }
            if (_domain != null)
            {
                _domain.AssemblyResolve -= loadFromSameFolder;
                AppDomain.Unload(_domain);
            }
            var gameCreatorInterface = typeof(IGameStarter);
            FileInfo fileInfo = new FileInfo(path);
            _currentFolder = fileInfo.DirectoryName;
            _domain = AppDomain.CreateDomain("LoadingGameDomain");
            _domain.AssemblyResolve += loadFromSameFolder;
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.CodeBase = path;
            var assembly = _domain.Load(assemblyName);
            try
            {
                var games = assembly.GetTypes().Where(type => gameCreatorInterface.IsAssignableFrom(type)
                                                      && gameCreatorInterface != type).ToList();
                return (games, assembly);
            }
            catch (ReflectionTypeLoadException e)
            {
                Debug.WriteLine($"Game Loader: Can't load types from {path}. Exception: {e.ToString()}");
                foreach (var loaderException in e.LoaderExceptions)
                {
                    Debug.WriteLine(loaderException.ToString());
                }
                return (new List<Type>(), assembly);
            }
        }

        public static void Load(IRenderMessagePump messagePump, AGSProject agsProj, AGSEditor editor)
        {
            messagePump.Post(async _ => await load(agsProj, editor), null);
        }

        private static async Task<string> getOutputPath(AGSProject agsProj)
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(agsProj.AGSProjectPath));
                await AGSEditor.Platform.DotnetProject.Load(agsProj.DotnetProjectPath);
                Directory.SetCurrentDirectory(currentDir);
                if (!File.Exists(AGSEditor.Platform.DotnetProject.OutputFilePath))
                {
                    string errors = await AGSEditor.Platform.DotnetProject.Compile();
                    if (errors != null)
                    {
                        Debug.WriteLine($"Can't compile dotnet project: {agsProj.DotnetProjectPath}, referenced in AGS project: {agsProj.AGSProjectPath}");
                        Debug.WriteLine(errors);
                        return null;
                    }
                    if (!File.Exists(AGSEditor.Platform.DotnetProject.OutputFilePath))
                    {
                        Debug.WriteLine($"Project was compiled but output not found for dotnet project: {agsProj.DotnetProjectPath}, referenced in AGS project: {agsProj.AGSProjectPath}");
                        return null;
                    }
                }
                return AGSEditor.Platform.DotnetProject.OutputFilePath;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
        }

        private static async Task load(AGSProject agsProj, AGSEditor editor)
        {
            var (games, assembly) = await GetGames(agsProj);
            if (games.Count == 0)
            {
                throw new Exception($"Cannot load game: failed to find an instance of IGameCreator in {agsProj.AGSProjectPath}.");
            }
            if (games.Count > 1)
            {
                throw new Exception($"Cannot load game: found more than one instance of IGameCreator in {agsProj.AGSProjectPath}.");
            }
            var gameCreatorImplementation = games[0];
            var gameCreator = (IGameStarter)Activator.CreateInstance(gameCreatorImplementation);

            var editorResolver = editor.EditorResolver;
            var updatePump = editorResolver.Container.Resolve<IUpdateMessagePump>();

            var resolver = new Resolver(AGSGame.Device);
            resolver.Builder.RegisterType<EditorShouldBlockEngineInput>().SingleInstance().As<IShouldBlockInput>().As<EditorShouldBlockEngineInput>();
            resolver.Builder.RegisterInstance(updatePump).As<IUpdateMessagePump>().As<IUpdateThread>();
            var game = AGSGame.CreateEmpty(resolver);
            editor.Game = game;
            editor.GameResolver = resolver;
            gameCreator.StartGame(game);

            var keyboardBindings = new KeyboardBindings(editor.Editor.Input);
            var actions = editorResolver.Container.Resolve<ActionManager>();
            var resourceLoader = resolver.Container.Resolve<IResourceLoader>();
            resourceLoader.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(assembly), 2));

            _gameDebugView = new Lazy<GameDebugView>(() =>
            {
                var gameDebugView = new GameDebugView(editor, keyboardBindings, actions);
                gameDebugView.Load();
                return gameDebugView;
            });

            EditorShouldBlockEngineInput blocker = resolver.Container.Resolve<EditorShouldBlockEngineInput>();

            var toolbar = new GameToolbar(blocker, editor.Editor.Input);
            toolbar.Init(editor.Editor.Factory);

            game.Events.OnLoad.Subscribe(() =>
            {
                editor.Init();
                toolbar.SetGame(game);
            });

            game.Start(new AGSGameSettings("Demo Game", new AGS.API.Size(320, 200),
               windowSize: new AGS.API.Size(640, 400), windowState: WindowState.Normal, preserveAspectRatio: false));

            keyboardBindings.OnKeyboardShortcutPressed.Subscribe(async action =>
            {
                if (action == KeyboardBindings.GameView)
                {
                    var gameDebug = _gameDebugView.Value;
                    if (gameDebug.Visible) gameDebug.Hide();
                    else await gameDebug.Show();
                }
            });
        }

        private static Assembly loadFromSameFolder(object sender, ResolveEventArgs args)
        {
            if (_currentFolder == null) return null;
            string assemblyPath = Path.Combine(_currentFolder, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                assemblyPath = assemblyPath.Replace(".dll", ".exe");
                if (!File.Exists(assemblyPath))
                {
                    return null;
                }
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}