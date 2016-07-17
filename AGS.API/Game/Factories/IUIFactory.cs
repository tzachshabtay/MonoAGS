using System;
using System.Threading.Tasks;

namespace AGS.API
{
    public interface IUIFactory
	{
		IPanel GetPanel(string id, IImage image, float x, float y, bool addToUi = true);
		IPanel GetPanel(string id, float width, float height, float x, float y, bool addToUi = true);
		IPanel GetPanel(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true);
		Task<IPanel> GetPanelAsync(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true);

		ILabel GetLabel(string id, string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true);

		IButton GetButton(string id, IAnimation idle, IAnimation hovered, IAnimation pushed, float x, 
			float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		IButton GetButton(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath, 
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
		Task<IButton> GetButtonAsync(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);

        ITextBox GetTextbox(string id, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, 
            float width = -1f, float height = -1f);

        ICheckbox GetCheckbox(string id, IAnimation notChecked, IAnimation notCheckedHovered, IAnimation @checked, IAnimation checkedHovered,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
        ICheckbox GetCheckbox(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);
        Task<ICheckbox> GetCheckboxAsync(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);

        IComboBox GetComboBox(string id, IButton dropDownButton, ITextBox textBox, Func<IButton> itemButtonFactory, bool addToUi = true);

        ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);
		Task<ISlider> GetSliderAsync(string id, string imagePath, string handleImagePath, float value, float min, float max,
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);
	}
}

