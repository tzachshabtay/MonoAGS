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
			AGSDirectionalAnimation walk = new AGSDirectionalAnimation
			{
				Left = factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Walk/left", loadConfig : loadConfig),
				Down = factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Walk/front", loadConfig : loadConfig),
			};
			foreach (var frame in walk.Left.Frames)
			{
				frame.Sprite.Anchor = new AGSPoint (0.5f, 0f);
			}
			walk.Right = walk.Left.Clone();
			walk.Right.FlipHorizontally();

			AGSDirectionalAnimation idle = new AGSDirectionalAnimation 
			{
				Left = factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Idle/left", loadConfig : loadConfig),
				Down = factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Idle/front", loadConfig : loadConfig),
			};
			foreach (var frame in idle.Left.Frames)
			{
				frame.Sprite.Anchor = new AGSPoint (0.5f, 0f);
			}
			idle.Right = idle.Left.Clone();
			idle.Right.FlipHorizontally();

			_character.WalkAnimation = walk;
			_character.IdleAnimation = idle;
			_character.StartAnimation (idle.Down);
			_character.Hotspot = "Cris";
			_character.PixelPerfect(true);

			return _character;
		}
	}
}

