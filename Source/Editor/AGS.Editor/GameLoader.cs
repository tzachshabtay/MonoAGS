using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using Autofac;

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

        public static (List<Type> games, Assembly assembly) GetGames(string path)
        {
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

        public static void Load(IRenderMessagePump messagePump, string path, IGame editorGame)
        {
            messagePump.Post(_ => load(path, editorGame), null);
        }

        private static void load(string path, IGame editorGame)
        {
            var (games, assembly) = GetGames(path);
            if (games.Count == 0)
            {
                throw new Exception($"Cannot load game: failed to find an instance of IGameCreator in {path}.");
            }
            if (games.Count > 1)
            {
                throw new Exception($"Cannot load game: found more than one instance of IGameCreator in {path}.");
            }
            var gameCreatorImplementation = games[0];
            var gameCreator = (IGameStarter)Activator.CreateInstance(gameCreatorImplementation);
            var game = AGSGame.CreateEmpty();
            gameCreator.StartGame(game);

            if (game is AGSGame agsGame) //todo: find a solution for any IGame implementation
            {
                var resourceLoader = agsGame.GetResolver().Container.Resolve<IResourceLoader>();
                resourceLoader.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(assembly), 2));
            }

            _gameDebugView = new Lazy<GameDebugView>(() =>
            {
                var gameDebugView = new GameDebugView(game);
                gameDebugView.Load();
                return gameDebugView;
            });

            game.Start(new AGSGameSettings("Demo Game", new AGS.API.Size(320, 200),
               windowSize: new AGS.API.Size(640, 400), windowState: WindowState.Normal));

            game.Input.KeyDown.SubscribeToAsync(async args =>
            {
                if (args.Key == Key.G && (game.Input.IsKeyDown(Key.AltLeft) || game.Input.IsKeyDown(Key.AltRight)))
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
