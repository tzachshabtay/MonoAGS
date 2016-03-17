namespace AGS.API
{
    public interface IUIFactory
	{
		IPanel GetPanel(string id, IImage image, float x, float y, bool addToUi = true);
		IPanel GetPanel(string id, float width, float height, float x, float y, bool addToUi = true);
		IPanel GetPanel(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true);
		IPanel GetPanel(string id, IAnimationContainer innerContainer, IImage image, float x, float y);

		ILabel GetLabel(string id, string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true);
		ILabel GetLabel(IPanel innerPanel, string text, float width, float height, float x, float y, ITextConfig config = null);

		IButton GetButton(string id, IAnimation idle, IAnimation hovered, IAnimation pushed, float x, 
			float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		IButton GetButton(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath, 
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		IButton GetButton(ILabel innerLabel, IAnimation idle, IAnimation hovered, IAnimation pushed);

		ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);
	}
}

