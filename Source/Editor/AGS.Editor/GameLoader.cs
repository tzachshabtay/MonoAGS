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
using Newtonsoft.Json;

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

        public static void SetupResolver()
        {
            Resolver.Override(resolver => resolver.Builder.RegisterType<KeyboardBindings>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterType<ActionManager>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterAssemblyTypes(typeof(GameLoader).Assembly).
                              Except<InspectorTreeNodeProvider>().AsImplementedInterfaces().ExternallyOwned());
        }

        public static IEditorPlatform Platform { get; set; }

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

        public static void Load(IRenderMessagePump messagePump, AGSProject agsProj, IGameFactory factory)
        {
            messagePump.Post(async _ => await load(agsProj, factory), null);
        }

        private static async Task<string> getOutputPath(AGSProject agsProj)
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(agsProj.AGSProjectPath));
                await Platform.DotnetProject.Load(agsProj.DotnetProjectPath);
                Directory.SetCurrentDirectory(currentDir);
                if (!File.Exists(Platform.DotnetProject.OutputFilePath))
                {
                    string errors = await Platform.DotnetProject.Compile();
                    if (errors != null)
                    {
                        Debug.WriteLine($"Can't compile dotnet project: {agsProj.DotnetProjectPath}, referenced in AGS project: {agsProj.AGSProjectPath}");
                        Debug.WriteLine(errors);
                        return null;
                    }
                    if (!File.Exists(Platform.DotnetProject.OutputFilePath))
                    {
                        Debug.WriteLine($"Project was compiled but output not found for dotnet project: {agsProj.DotnetProjectPath}, referenced in AGS project: {agsProj.AGSProjectPath}");
                        return null;
                    }
                }
                return Platform.DotnetProject.OutputFilePath;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
        }

        private static async Task load(AGSProject agsProj, IGameFactory factory)
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
            var game = AGSGame.CreateEmpty();
            gameCreator.StartGame(game);

            KeyboardBindings keyboardBindings = null;
            ActionManager actions = null;
            if (game is AGSGame agsGame) //todo: find a solution for any IGame implementation
            {
                Resolver resolver = agsGame.GetResolver();
                keyboardBindings = resolver.Container.Resolve<KeyboardBindings>();
                actions = resolver.Container.Resolve<ActionManager>();
                var resourceLoader = resolver.Container.Resolve<IResourceLoader>();
                resourceLoader.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(assembly), 2));
            }

            _gameDebugView = new Lazy<GameDebugView>(() =>
            {
                var gameDebugView = new GameDebugView(game, keyboardBindings, actions);
                gameDebugView.Load();
                return gameDebugView;
            });

            var toolbar = new GameToolbar();
            toolbar.Init(factory);

            game.Events.OnLoad.Subscribe(() =>
            {
                toolbar.SetGame(game);
            });

            game.Start(new AGSGameSettings("Demo Game", new AGS.API.Size(320, 200),
               windowSize: new AGS.API.Size(640, 400), windowState: WindowState.Normal));

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