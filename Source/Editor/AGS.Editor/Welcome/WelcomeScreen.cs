using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    public class WelcomeScreen
    {
        private readonly IGame _game;
        private readonly RecentGames _recentGames;
        private readonly Resolver _resolver;
        private IPanel _panel;

        public WelcomeScreen(IGame game)
        {
            _game = game;
            _resolver = ((AGSGame)game).GetResolver();
            _recentGames = _resolver.Container.Resolve<RecentGames>();
        }

        public void Load()
        {
            _recentGames.Load();
            _panel = _game.Factory.UI.GetPanel("WelcomeScreenPanel", 1280, 800, 0, 0, addToUi: false);
            _panel.Tint = GameViewColors.Panel;

            var border = AGSBorders.SolidColor(GameViewColors.Border, 2f);

            var idle = new ButtonAnimation(border, GameViewColors.TextConfig, GameViewColors.Button);
            var hovered = new ButtonAnimation(border, GameViewColors.HoverTextConfig, GameViewColors.Button);
            var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.Black, 2f), GameViewColors.TextConfig, GameViewColors.Button);
            var loadButton = _game.Factory.UI.GetButton("LoadGameButton", idle, hovered, pushed, 200f, 700f, _panel, 
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
            _game.State.UI.Add(_panel);
        }

        private async Task onLoadGameClicked(MouseButtonEventArgs args)
        {
            AGSGameSettings.CurrentSkin = new AGSSilverSkin(_game.Factory.Graphics, AGSGame.GLUtils).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly, isGame);
            AGSGameSettings.CurrentSkin = null;
            if (string.IsNullOrEmpty(file)) return;
            var messagePump = _resolver.Container.Resolve<IRenderMessagePump>();
            _panel.Visible = false;
            _recentGames.AddGame(file);
            await Task.Delay(100);
            GameLoader.Load(messagePump, file);
        }

        private bool isGame(string file)
        {
            if (!file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) &&
                !file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) return false;
            try
            {
                return GameLoader.GetGames(file).games.Count > 0;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
        }

    }
}
