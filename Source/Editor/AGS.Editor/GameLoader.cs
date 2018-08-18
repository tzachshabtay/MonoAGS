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
            editor.Project = agsProj;
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
                throw new Exception($"Cannot load game: failed to find an instance of {nameof(IGameStarter)} in {agsProj.AGSProjectPath}.");
            }
            if (games.Count > 1)
            {
                throw new Exception($"Cannot load game: found more than one instance of {nameof(IGameStarter)} in {agsProj.AGSProjectPath}.");
            }
            var gameCreatorImplementation = games[0];
            var gameCreator = (IGameStarter)Activator.CreateInstance(gameCreatorImplementation);

            var editorResolver = editor.EditorResolver;
            var updatePump = editorResolver.Container.Resolve<IUpdateMessagePump>();

            var gameSettings = gameCreator.Settings;
            var gameResolver = new Resolver(AGSGame.Device, gameSettings);
            gameResolver.Builder.RegisterType<EditorShouldBlockEngineInput>().SingleInstance().As<IShouldBlockInput>().As<EditorShouldBlockEngineInput>();
            gameResolver.Builder.RegisterInstance(updatePump).As<IUpdateMessagePump>().As<IUpdateThread>();
            AGSEditor.Platform.SetResolverForGame(gameResolver, editorResolver);
            var game = AGSGame.Create(gameResolver);
            editor.Game = game;
            editor.GameResolver = gameResolver;
            gameCreator.StartGame(game);

            var keyboardBindings = new KeyboardBindings(editor.Editor.Input);
            var actions = editorResolver.Container.Resolve<ActionManager>();
            var resourceLoader = gameResolver.Container.Resolve<IResourceLoader>();
            resourceLoader.ResourcePacks.Add(new ResourcePack(new FileSystemResourcePack(AGSGame.Device.FileSystem, assembly), 2));
            resourceLoader.ResourcePacks.Add(new ResourcePack(new EmbeddedResourcesPack(assembly), 3));

            EditorShouldBlockEngineInput blocker = gameResolver.Container.Resolve<EditorShouldBlockEngineInput>();

            var toolbar = new GameToolbar(blocker, editor.Editor.Input, editor.Editor.State, editor.Editor.Settings);
            toolbar.Init(editor.Editor.Factory, editor);

            game.Events.OnLoad.Subscribe(() =>
            {
                editor.Init();

                var gameDebugView = new GameDebugView(editor, keyboardBindings, actions, toolbar);
                toolbar.SetGame(game, editor.GameResolver.Container.Resolve<IWindowInfo>(), gameDebugView.Tree);
                var canvas = new GameCanvas(editor, toolbar, gameDebugView.Tree);
                canvas.Init();
                gameDebugView.Load();
                gameDebugView.Show();

                keyboardBindings.OnKeyboardShortcutPressed.Subscribe(async action =>
                {
                    if (action == KeyboardBindings.GameView)
                    {
                        if (gameDebugView.Visible) gameDebugView.Hide();
                        else await gameDebugView.Show();
                    }
                });
            });

            game.Start();
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
