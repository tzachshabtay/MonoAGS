using System;

namespace AGS.API
{
	public interface IUIFactory
	{
		IPanel GetPanel(IImage image, float x, float y, bool addToUi = true);
		IPanel GetPanel(float width, float height, float x, float y, bool addToUi = true);
		IPanel GetPanel(string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true); 

		ILabel GetLabel(string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true);
		IButton GetButton(IAnimation idle, IAnimation hovered, IAnimation pushed, float x, 
			float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		IButton GetButton(string idleImagePath, string hoveredImagePath, string pushedImagePath, 
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		ISlider GetSlider(string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);
	}
}

