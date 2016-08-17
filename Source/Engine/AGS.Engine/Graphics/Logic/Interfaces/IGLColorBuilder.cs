using System;
using AGS.API;


namespace AGS.Engine
{
	public interface IGLColorBuilder
	{
		IGLColor Build(params IHasImage[] sprites);
		IGLColor Build(Color color);
	}
}

