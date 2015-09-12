using System;
using API;
using System.Drawing;

namespace Engine
{
	public interface IGLColorBuilder
	{
		IGLColor Build(params ISprite[] sprites);
		IGLColor Build(Color color);
	}
}

