using System;

namespace API
{
	public interface IImageRenderer
	{
		void Render(IObject obj, IViewport viewport);
	}
}

