using System;
using API;
using Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class Cris
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Cris/";

		public Cris()
		{
			_character = new AGSCharacter ();
		}

		public ICharacter Load(IGraphicsFactory factory)
		{
			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig
			{ 
				TransparentColorSamplePoint = new Point (0, 0) 
			};
			AGSDirectionalAnimation walk = new AGSDirectionalAnimation
			{
				Left = factory.LoadAnimationFromFolder(_baseFolder + "Walk/left", loadConfig : loadConfig),
				Down = factory.LoadAnimationFromFolder(_baseFolder + "Walk/front", loadConfig : loadConfig),
			};
			walk.Right = walk.Left.Clone();
			walk.Right.FlipHorizontally();

			AGSDirectionalAnimation idle = new AGSDirectionalAnimation 
			{
				Left = factory.LoadAnimationFromFolder(_baseFolder + "Idle/left", loadConfig : loadConfig),
				Down = factory.LoadAnimationFromFolder(_baseFolder + "Idle/front", loadConfig : loadConfig),
			};
			idle.Right = idle.Left.Clone();
			idle.Right.FlipHorizontally();

			_character.WalkAnimation = walk;
			_character.IdleAnimation = idle;
			_character.StartAnimation (idle.Down);
			_character.Z = 50;
			_character.Hotspot = "Cris";

			return _character;
		}
	}
}

