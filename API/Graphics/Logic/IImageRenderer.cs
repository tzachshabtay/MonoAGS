using System;

namespace AGS.API
{
	public interface IImageRenderer
	{
		void Render(IObject obj, IViewport viewport, IPoint areaScaling);
	}
}

