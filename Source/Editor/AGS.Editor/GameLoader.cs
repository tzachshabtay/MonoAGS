using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class GameLoader
    {
        private static Lazy<GameDebugView> _gameDebugView;

        private static string _currentFolder;

        static GameLoader()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += loadFromSameFolder;
        }

        public static void Load(string path)
        {
            var gameCreatorInterface = typeof(IGameCreator);
            FileInfo fileInfo = new FileInfo(path);
            _currentFolder = fileInfo.DirectoryName;
            var assembly = Assembly.LoadFrom(path);
            var types = assembly.GetTypes();
            var etypes = assembly.GetExportedTypes();
            var games = assembly.GetTypes().Where(type => gameCreatorInterface.IsAssignableFrom(type)).ToList();
            if (games.Count == 0)
            {
                throw new Exception($"Cannot load game: failed to find an instance of IGameCreator in {path}.");
            }
            if (games.Count > 1)
            {
                throw new Exception($"Cannot load game: found more than one instance of IGameCreator in {path}.");
            }
            var gameCreatorImplementation = games[0];
            var gameCreator = (IGameCreator)Activator.CreateInstance(gameCreatorImplementation);
            var game = gameCreator.CreateGame();

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

        static Assembly loadFromSameFolder(object sender, ResolveEventArgs args)
        {
            if (_currentFolder == null) return null;
            string assemblyPath = Path.Combine(_currentFolder, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
