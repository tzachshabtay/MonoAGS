using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSAnimationFrame : IAnimationFrame
	{
		public AGSAnimationFrame (ISprite sprite)
		{
			Sprite = sprite;
		}

		#region IAnimationFrame implementation

		public ISprite Sprite { get; set; }

		public int Delay { get; set; }

		public ISound Sound { get; set; }

		#endregion

		public IAnimationFrame Clone()
		{
			AGSAnimationFrame clone = (AGSAnimationFrame)MemberwiseClone();
			clone.Sprite = Sprite.Clone();
			return clone;
		}
	}
}

