using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class Beman
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Beman/";
		private BemanDialogs _dialogs = new BemanDialogs();
		private IGame _game;

		public ICharacter Load(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new AGS.API.Point (0, 0) 
			};

			IOutfit outfit = factory.Outfit.LoadOutfitFromFolders(_baseFolder, 
				walkLeftFolder: "Walk/left", walkDownFolder: "Walk/down", walkRightFolder: "Walk/right", walkUpFolder: "Walk/up", 
				idleLeftFolder: "Idle/left", idleDownFolder: "Idle/down", idleRightFolder: "Idle/right", idleUpFolder: "Idle/up", 
				speakLeftFolder: "Talk/left", speakDownFolder: "Talk/down", speakRightFolder: "Talk/right", speakUpFolder: "Talk/up", 
				loadConfig: loadConfig);

			_character = factory.Object.GetCharacter("Beman", outfit).Remember(_game, character => 
			{
				_character = character;
				subscribeEvents();
			});

			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Left);
			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Right);
			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Down);
			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Up);

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Beman";
			_character.PixelPerfect(true);

			Characters.Beman = _character;

			_dialogs.Load(game);
			return _character;
		}

		private void subscribeEvents()
		{
			_character.Interactions.OnCustomInteract.SubscribeToAsync(async (sender, e) =>
			{ 
				if (e.InteractionName == MouseCursors.TALK_MODE) await _dialogs.StartDialog.RunAsync();
				else await _game.State.Player.Character.SayAsync("I don't think he'd appreciate that.");
			});
		}
	}
}

