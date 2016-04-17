using System;
using AGS.API;

namespace AGS.Engine
{
	public class HasOutfitComponent : AGSComponent, IHasOutfit
	{
		public IOutfit Outfit { get; set; }
	}
}

