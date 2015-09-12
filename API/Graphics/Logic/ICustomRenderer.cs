using System;

namespace API
{
	public interface ICustomRenderer
	{
		//float Z { get; set; }
		//IRenderLayer RenderLayer { get; set; }
		void Render(ISprite sprite, IViewport viewport);
	}
}

