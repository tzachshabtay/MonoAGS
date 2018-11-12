using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class Beman
	{
		private ICharacter _character;
		private const string _baseFolder = "Characters/Beman/";
		private BemanDialogs _dialogs = new BemanDialogs();
		private IGame _game;

		public async Task<ICharacter> LoadAsync(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;
            AGSLoadImageConfig loadConfig = new AGSLoadImageConfig(new Point(0, 0));

			IOutfit outfit = await factory.Outfit.LoadOutfitFromFoldersAsync(_baseFolder, 
				walkLeftFolder: "Walk/left", walkDownFolder: "Walk/down", walkRightFolder: "Walk/right", walkUpFolder: "Walk/up", 
				idleLeftFolder: "Idle/left", idleDownFolder: "Idle/down", idleRightFolder: "Idle/right", idleUpFolder: "Idle/up", 
				speakLeftFolder: "Talk/left", speakDownFolder: "Talk/down", speakRightFolder: "Talk/right", speakUpFolder: "Talk/up", 
				loadConfig: loadConfig);

			_character = factory.Object.GetCharacter("Beman", outfit).Remember(_game, character => 
			{
				_character = character;
				subscribeEvents();
			});
            _character.SpeechConfig.TextConfig = AGSTextConfig.ChangeColor(_character.SpeechConfig.TextConfig, Colors.CornflowerBlue, Colors.Black, 1f);
            _character.SpeechConfig.TextOffset = (0f, -10f);

            //Uncomment for portrait
            /*
            var portrait = game.Factory.Object.GetObject("BemanPortrait");
            portrait.StartAnimation(game.Factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Talk/down"));
            portrait.Border = AGSBorders.SolidColor(Colors.AliceBlue, 3f, true);
            portrait.Visible = false;
            portrait.RenderLayer = AGSLayers.Speech;
            portrait.IgnoreViewport = true;
            portrait.IgnoreScalingArea = true;
            game.State.UI.Add(portrait);
            _character.SpeechConfig.PortraitConfig = new AGSPortraitConfig { Portrait = portrait, Positioning = PortraitPositioning.Alternating };
            */

            var speakAnimation = _character.Outfit[AGSOutfit.Speak];
            Characters.RandomAnimationDelay(speakAnimation.Left);
			Characters.RandomAnimationDelay(speakAnimation.Right);
			Characters.RandomAnimationDelay(speakAnimation.Down);
			Characters.RandomAnimationDelay(speakAnimation.Up);

            _character.StartAnimation (_character.Outfit[AGSOutfit.Idle].Down);
			_character.DisplayName = "Beman";
            _character.IsPixelPerfect = true;

			Characters.Beman = _character;

			_dialogs.Load(game);
			return _character;
		}

		private void subscribeEvents()
		{
            _character.Interactions.OnInteract(MouseCursors.TALK_MODE).SubscribeToAsync(async _ =>
			{ 
				await _dialogs.StartDialog.RunAsync();
			});
            _character.Interactions.OnInventoryInteract(AGSInteractions.DEFAULT).SubscribeToAsync(async _ =>
            {
                await _game.State.Player.SayAsync("I don't think he'd appreciate that."); 
            });
		}
	}
}