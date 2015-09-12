using System;
using API;
using Engine;
using System.Threading.Tasks;

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
			AGSDirectionalAnimation walk = new AGSDirectionalAnimation
			{
				Left = factory.LoadAnimationFromFolder(_baseFolder + "Walk/left"),
				Down = factory.LoadAnimationFromFolder(_baseFolder + "Walk/front"),
			};
			walk.Right = walk.Left;

			AGSDirectionalAnimation idle = new AGSDirectionalAnimation 
			{
				Left = factory.LoadAnimationFromFolder(_baseFolder + "Idle/left"),
				Down = factory.LoadAnimationFromFolder(_baseFolder + "Idle/front"),
			};
			idle.Right = idle.Left;

			_character.WalkAnimation = walk;
			_character.IdleAnimation = idle;
			_character.StartAnimation (idle.Down);
			_character.Z = 50;
			_character.Hotspot = "Cris";

			return _character;
		}
	}
}

