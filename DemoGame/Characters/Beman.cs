using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

namespace DemoGame
{
	public class Beman
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Beman/";
		private BemanDialogs _dialogs = new BemanDialogs();

		public ICharacter Load(IGameFactory factory)
		{
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new Point (0, 0) 
			};

			IOutfit outfit = factory.Outfit.LoadOutfitFromFolders(_baseFolder, 
				walkLeftFolder: "Walk/left", walkDownFolder: "Walk/down", walkRightFolder: "Walk/right", walkUpFolder: "Walk/up", 
				idleLeftFolder: "Idle/left", idleDownFolder: "Idle/down", idleRightFolder: "Idle/right", idleUpFolder: "Idle/up", 
				speakLeftFolder: "Talk/left", speakDownFolder: "Talk/down", speakRightFolder: "Talk/right", speakUpFolder: "Talk/up", 
				loadConfig: loadConfig);

			_character = factory.Object.GetCharacter(outfit);

			Cris.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Left);
			Cris.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Right);
			Cris.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Down);
			Cris.RandomAnimationDelay(_character.Outfit.SpeakAnimation.Up);

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Beman";
			_character.PixelPerfect(true);

			_character.Interactions.OnInteract.SubscribeToAsync(async (sender, e) => await _dialogs.StartDialog.RunAsync());

			Characters.Beman = _character;
			_dialogs.Load(factory);
			return _character;
		}
	}
}

