using System;

namespace AGS.API
{
	public interface IHasOutfit : IComponent
	{
		IOutfit Outfit { get; set; }
	}
}

