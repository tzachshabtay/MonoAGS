using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class Cris
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Cris/";

		public ICharacter Load(IGameFactory factory)
		{
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new Point (0, 0) 
			};

			IOutfit outfit = factory.LoadOutfitFromFolders(_baseFolder, walkLeftFolder: "Walk/left",
				walkDownFolder: "Walk/front", idleLeftFolder: "Idle/left", idleDownFolder: "Idle/front", 
				speakLeftFolder: "Talk", loadConfig: loadConfig);

			_character = factory.GetCharacter(outfit);

			RandomAnimationDelay(_character.Outfit.SpeakAnimation.Left);
			RandomAnimationDelay(_character.Outfit.SpeakAnimation.Right);

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Cris";
			_character.PixelPerfect(true);

			Characters.Cris = _character;
			return _character;
		}

		public static void RandomAnimationDelay(IAnimation animation)
		{
			foreach (var frame in animation.Frames)
			{
				frame.MinDelay = 5;
				frame.MaxDelay = 30;
			}
		}
	}
}

