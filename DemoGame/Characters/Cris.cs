using AGS.API;
using AGS.Engine;
using System.Drawing;

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
				TransparentColorSamplePoint = new AGSPoint (0, 0) 
			};

			IOutfit outfit = game.Factory.Outfit.LoadOutfitFromFolders(_baseFolder, walkLeftFolder: "Walk/left",
				walkDownFolder: "Walk/front", idleLeftFolder: "Idle/left", idleDownFolder: "Idle/front", 
				speakLeftFolder: "Talk", loadConfig: loadConfig);

			_character = game.Factory.Object.GetCharacter("Cris", outfit).Remember(game, c => _character = c);

			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Left);
			Characters.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Right);

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Cris";
			_character.PixelPerfect(true);

			Characters.Cris = _character;
			return _character;
		}
	}
}

