using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface ICharacter : IObject, ISayBehavior, IWalkBehavior, IFaceDirectionBehavior
	{
		IInventory Inventory { get; set; }
		IOutfit Outfit { get; set; }
	}
}

