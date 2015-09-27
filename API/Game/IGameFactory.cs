using System;
using System.Drawing;

namespace AGS.API
{
	public interface IGameFactory
	{
		IGraphicsFactory Graphics { get; }
		ISoundFactory Sound { get; }

		IOutfit LoadOutfitFromFolders(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null);

		int GetInt(string name, int defaultValue = 0);
		float GetFloat(string name, float defaultValue = 0f);
		string GetString(string name, string defaultValue = null);

		ILabel GetLabel(string text, float width, float height, float x, float y, ITextConfig config = null);
		IObject GetObject(string[] sayWhenLook = null, string[] sayWhenInteract = null);
		ICharacter GetCharacter(IOutfit outfit, string[] sayWhenLook = null, string[] sayWhenInteract = null);
		IObject GetHotspot(string maskPath, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null);
		IObject GetHotspot(Bitmap maskBitmap, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null);

		IEdge GetEdge(float value = 0f);
		IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float bottomEdge = 0f, float topEdge = 0f);

		void RegisterCustomData(ICustomSerializable customData);
	}
}

