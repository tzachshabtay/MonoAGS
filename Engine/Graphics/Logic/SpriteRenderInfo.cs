using System;
using AGS.API;

namespace AGS.Engine
{
	public struct SpriteRenderInfo
	{
		public SpriteRenderInfo(ISprite sprite, IObject obj)
		{
			Sprite = sprite;
			Object = obj;
		}

		public ISprite Sprite { get; private set; }
		public IObject Object { get; private set; }
	}
}

