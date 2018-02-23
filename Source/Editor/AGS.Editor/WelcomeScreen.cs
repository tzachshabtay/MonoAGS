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
        private IPanel _panel;

        public WelcomeScreen(IGame game)
        {
            _game = game;
        }

        public void Load()
        {
            _panel = _game.Factory.UI.GetPanel("WelcomeScreenPanel", 1280, 800, 0, 0, addToUi: false);
            _panel.Tint = GameViewColors.Panel;

            var border = AGSBorders.SolidColor(GameViewColors.Border, 2f);

            var idle = new ButtonAnimation(border, GameViewColors.TextConfig, GameViewColors.Button);
            var hovered = new ButtonAnimation(border, GameViewColors.HoverTextConfig, GameViewColors.Button);
            var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.Black, 2f), GameViewColors.TextConfig, GameViewColors.Button);
            var loadButton = _game.Factory.UI.GetButton("LoadGameButton", idle, hovered, pushed, 200f, 600f, _panel, 
                    "Load Game...", new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText), width: 100f, height: 100f);

            loadButton.MouseClicked.SubscribeToAsync(onLoadGameClicked);
        }

        public void Show()
        {
            _game.State.UI.Add(_panel);
        }

        private async Task onLoadGameClicked(MouseButtonEventArgs args)
        {
            AGSGameSettings.CurrentSkin = new AGSBlueSkin(_game.Factory.Graphics, AGSGame.GLUtils).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly, isGame);
            AGSGameSettings.CurrentSkin = null;
            if (string.IsNullOrEmpty(file)) return;
            var messagePump = ((AGSGame)_game).GetResolver().Container.Resolve<IRenderMessagePump>();
            messagePump.Post(_ => GameLoader.Load(file), null);
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
