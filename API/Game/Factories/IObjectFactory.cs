using System;
using System.Drawing;

namespace AGS.API
{
	public interface IObjectFactory
	{
		IObject GetObject(string[] sayWhenLook = null, string[] sayWhenInteract = null);
		IObject GetObject(IAnimationContainer innerContainer);

		ICharacter GetCharacter(IOutfit outfit, string[] sayWhenLook = null, string[] sayWhenInteract = null);
		ICharacter GetCharacter(IObject innerObject, IOutfit outfit);

		IObject GetHotspot(string maskPath, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null);
		IObject GetHotspot(Bitmap maskBitmap, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null);
	}
}

