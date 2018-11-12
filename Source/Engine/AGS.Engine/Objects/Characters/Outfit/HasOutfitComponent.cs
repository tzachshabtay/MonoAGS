using AGS.API;

namespace AGS.Engine
{
	public class HasOutfitComponent : AGSComponent, IOutfitComponent
	{
		public IOutfit Outfit { get; set; }
	}
}

