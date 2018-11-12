using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    public class WelcomeScreen
    {
        private readonly AGSEditor _editor;
        private readonly RecentGames _recentGames;
        private readonly Resolver _resolver;
        private IPanel _panel;

        public WelcomeScreen(AGSEditor editor)
        {
            _editor = editor;
            _resolver = editor.EditorResolver;
            _recentGames = _resolver.Container.Resolve<RecentGames>();
        }

        public void Load()
        {
            _recentGames.Load();
            _panel = _editor.Editor.Factory.UI.GetPanel("WelcomeScreenPanel", 1280, 800, 0, 0, addToUi: false);
            _panel.Tint = GameViewColors.Panel;

            var border = AGSBorders.SolidColor(GameViewColors.Border, 2f);

            var idle = new ButtonAnimation(border, GameViewColors.TextConfig, GameViewColors.Button);
            var hovered = new ButtonAnimation(border, GameViewColors.HoverTextConfig, GameViewColors.Button);
            var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.Black, 2f), GameViewColors.TextConfig, GameViewColors.Button);
            var loadButton = _editor.Editor.Factory.UI.GetButton("LoadGameButton", idle, hovered, pushed, 200f, 700f, _panel, 
                    "Load Game...", new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText), width: 100f, height: 100f);
            loadButton.Pivot = new PointF(0f, 1f);

            loadButton.MouseClicked.SubscribeToAsync(onLoadGameClicked);
        }

        public void Show()
        {
            var recentGamesPanel = _recentGames.Show(_panel);
            recentGamesPanel.X = 400f;
            recentGamesPanel.Y = 700f;
            recentGamesPanel.Pivot = new PointF(0f, 1f);
            _editor.Editor.State.UI.Add(_panel);
        }

        private async Task onLoadGameClicked(MouseButtonEventArgs args)
        {
            _editor.Editor.Settings.Defaults.Skin = new AGSSilverSkin(AGSGame.GLUtils, _editor.Editor.Settings).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly, isGame);
            _editor.Editor.Settings.Defaults.Skin = null;
            if (string.IsNullOrEmpty(file)) return;
            var messagePump = _resolver.Container.Resolve<IRenderMessagePump>();
            _panel.Visible = false;
            AGSProject agsProj = AGSProject.Load(file);
            _recentGames.AddGame(agsProj.Name, file);
            await Task.Delay(100);
            GameLoader.Load(messagePump, agsProj, _editor);
        }

        private bool isGame(string file)
        {
            return file.EndsWith(".agsproj.json", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
