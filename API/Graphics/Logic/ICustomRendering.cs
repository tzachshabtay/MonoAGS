using System;
using System.Collections.Generic;

namespace API
{
	public interface ICustomRendering
	{
		IEnumerable<ICustomRenderer> CustomRenderers { get; }
		void AddCustomRenderer (ICustomRenderer renderer);
		void RemoveCustomRenderer (ICustomRenderer renderer);

		void BindSpriteToRenderer(ISprite sprite, ICustomRenderer renderer);
	}
}

