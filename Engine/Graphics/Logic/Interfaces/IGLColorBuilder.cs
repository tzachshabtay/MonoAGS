using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public interface IGLColorBuilder
	{
		IGLColor Build(params ISprite[] sprites);
		IGLColor Build(Color color);
	}
}

