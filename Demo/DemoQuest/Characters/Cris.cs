using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class Cris
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Cris/";

		public ICharacter Load(IGame game)
		{
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new AGS.API.Point (0, 0) 
			};

			var footstep = game.Factory.Sound.LoadAudioClip("../../Assets/Sounds/151238__owlstorm__hard-female-footstep-2.wav");
			ISoundEmitter emitter = new AGSSoundEmitter (game);
			emitter.AudioClip = footstep;

			IOutfit outfit = game.Factory.Outfit.LoadOutfitFromFolders(_baseFolder, walkLeftFolder: "Walk/left",
				walkDownFolder: "Walk/front", idleLeftFolder: "Idle/left", idleDownFolder: "Idle/front", 
				speakLeftFolder: "Talk", loadConfig: loadConfig);

			_character = game.Factory.Object.GetCharacter("Cris", outfit).Remember(game, c => _character = c);
			emitter.Object = _character;

			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Left);
			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Right);
			emitter.Assign(_character.Outfit.WalkAnimation, 1, 5);

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Cris";
			_character.PixelPerfect(true);

			Characters.Cris = _character;
			return _character;
		}
	}
}

