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
			_character = factory.GetCharacter();
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new Point (0, 0) 
			};

			factory.Graphics.LoadToOutfitFromFolders(_baseFolder, _character.Outfit, walkLeftFolder: "Walk/left",
				walkDownFolder: "Walk/front", idleLeftFolder: "Idle/left", idleDownFolder: "Idle/front", 
				speakLeftFolder: "Talk", loadConfig: loadConfig);

			foreach (var frame in _character.Outfit.SpeakAnimation.Left.Frames)
			{
				frame.MinDelay = 5;
				frame.MaxDelay = 30;
			}
			foreach (var frame in _character.Outfit.SpeakAnimation.Right.Frames)
			{
				frame.MinDelay = 5;
				frame.MaxDelay = 30;
			}

			_character.StartAnimation (_character.Outfit.IdleAnimation.Down);
			_character.Hotspot = "Cris";
			_character.PixelPerfect(true);

			return _character;
		}
	}
}

