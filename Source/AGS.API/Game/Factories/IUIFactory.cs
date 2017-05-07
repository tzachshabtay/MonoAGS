using System;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Factory for creating UI controls (buttons, textboxes, etc).
    /// </summary>
    public interface IUIFactory
	{
        /// <summary>
        /// Create a panel.
        /// </summary>
        /// <returns>The panel.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="image">A background image for the panel.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="addToUi">If set to <c>true</c> add to the game's GUI list for rendering.</param>
		IPanel GetPanel(string id, IImage image, float x, float y, bool addToUi = true);

        /// <summary>
        /// Create a panel.
        /// </summary>
        /// <returns>The panel.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="width">The panel's width.</param>
        /// <param name="height">The panel's height.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="addToUi">If set to <c>true</c> add to the game's GUI list for rendering.</param>
		IPanel GetPanel(string id, float width, float height, float x, float y, bool addToUi = true);

        /// <summary>
        /// Create a panel.
        /// </summary>
        /// <returns>The panel.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="imagePath">A resource/file path (see cref="IResourceLoader") for the panel's background image/>.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="loadConfig">The configuration for loading the image.</param>
        /// <param name="addToUi">If set to <c>true</c> add to the game's GUI list for rendering.</param>
		IPanel GetPanel(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true);

        /// <summary>
        /// Create a panel asynchronously.
        /// </summary>
        /// <returns>The panel.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="imagePath">A resource/file path (see cref="IResourceLoader") for the panel's background image/>.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="loadConfig">The configuration for loading the image.</param>
        /// <param name="addToUi">If set to <c>true</c> add to the game's GUI list for rendering.</param>
		Task<IPanel> GetPanelAsync(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true);

        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <returns>The label.</returns>
        /// <param name="id">A unique identifer for the label (it has to be globally unique across all entities).</param>
        /// <param name="text">The text that appears on the label.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="config">The configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
		ILabel GetLabel(string id, string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true);

        /// <summary>
        /// Creats a button.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="id">A unique identifer for the button (it has to be globally unique across all entities).</param>
        /// <param name="idle">Idle animation (when the button is not clicked on or hovered).</param>
        /// <param name="hovered">Hovered animation (when the mouse is hovering the button).</param>
        /// <param name="pushed">Pushed animation (when clicking the button).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the button.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the idle animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the idle animation's first frame).</param>
		IButton GetButton(string id, IAnimation idle, IAnimation hovered, IAnimation pushed, float x, 
			float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);

        /// <summary>
        /// Creats a button.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="idleImagePath">Idle image resourece/file path (when the button is not clicked on or hovered).</param>
        /// <param name="hoveredImagePath">Hovered image resource/file path (when the mouse is hovering the button).</param>
        /// <param name="pushedImagePath">Pushed image resource/file path (when clicking the button).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the button.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the idle animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the idle animation's first frame)</param>
		IButton GetButton(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath, 
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);

        /// <summary>
        /// Creats a button asynchronously.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="id">A unique identifer for the panel (it has to be globally unique across all entities).</param>
        /// <param name="idleImagePath">Idle image resourece/file path (when the button is not clicked on or hovered).</param>
        /// <param name="hoveredImagePath">Hovered image resource/file path (when the mouse is hovering the button).</param>
        /// <param name="pushedImagePath">Pushed image resource/file path (when clicking the button).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the button.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the idle animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the idle animation's first frame)</param>
		Task<IButton> GetButtonAsync(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f);

        /// <summary>
        /// Creats a textbox.
        /// </summary>
        /// <returns>The textbox.</returns>
        /// <param name="id">A unique identifer for the textbox (it has to be globally unique across all entities).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the button.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height</param>/param>
        ITextBox GetTextBox(string id, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, 
                            float width = -1, float height = -1); //todo: remove the optional -1 from width and height (need to reorder parameters)

        /// <summary>
        /// Creats a checkbox.
        /// </summary>
        /// <returns>The checkbox.</returns>
        /// <param name="id">A unique identifer for the checkbox (it has to be globally unique across all entities).</param>
        /// <param name="notChecked">Not checked animation (when the checkbox is not checked).</param>
        /// <param name="notCheckedHovered">Hovered and not checked animation (when the checkbox is hovered by the mouse but not checked).</param>
        /// <param name="checked">Checked animation (when the checkbox is checked).</param>
        /// <param name="checkedHovered">Hovered and checked animation (when the checkbox is hovered by the mouse and checked).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the checkbox.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the not checked animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the not checked animation's first frame).</param>
        ICheckBox GetCheckBox(string id, IAnimation notChecked, IAnimation notCheckedHovered, IAnimation @checked, IAnimation checkedHovered,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f, bool isCheckButton = false);

        /// <summary>
        /// Creats a checkbox.
        /// </summary>
        /// <returns>The checkbox.</returns>
        /// <param name="id">A unique identifer for the checkbox (it has to be globally unique across all entities).</param>
        /// <param name="notCheckedPath">Not checked image resource/file path (when the checkbox is not checked).</param>
        /// <param name="notCheckedHoveredPath">Hovered and not checked image resource/file path (when the checkbox is hovered by the mouse but not checked).</param>
        /// <param name="checkedPath">Checked image resource/file path (when the checkbox is checked).</param>
        /// <param name="checkedHoveredPath">Hovered and checked image resource/file path (when the checkbox is hovered by the mouse and checked).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the checkbox.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the not checked animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the not checked animation's first frame).</param>
        ICheckBox GetCheckBox(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f, bool isCheckButton = false);

        /// <summary>
        /// Creats a checkbox asynchronously.
        /// </summary>
        /// <returns>The checkbox.</returns>
        /// <param name="id">A unique identifer for the checkbox (it has to be globally unique across all entities).</param>
        /// <param name="notCheckedPath">Not checked image resource/file path (when the checkbox is not checked).</param>
        /// <param name="notCheckedHoveredPath">Hovered and not checked image resource/file path (when the checkbox is hovered by the mouse but not checked).</param>
        /// <param name="checkedPath">Checked image resource/file path (when the checkbox is checked).</param>
        /// <param name="checkedHoveredPath">Hovered and checked image resource/file path (when the checkbox is hovered by the mouse and checked).</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="text">Text for the checkbox.</param>
        /// <param name="config">Configuration for rendering the text.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        /// <param name="width">Width (if not supplied the engine takes it from the not checked animation's first frame).</param>
        /// <param name="height">Height (if not supplied the engine takes it from the not checked animation's first frame).</param>
        Task<ICheckBox> GetCheckBoxAsync(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath,
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1f, float height = -1f, bool isCheckButton = false);

        /// <summary>
        /// Creates a combo box (drop down).
        /// </summary>
        /// <returns>The combo box.</returns>
        /// <param name="id">A unique identifer for the combo box (it has to be globally unique across all entities).</param>
        /// <param name="dropDownButton">Drop down button.</param>
        /// <param name="textBox">The text box for showing the selected choice.</param>
        /// <param name="itemButtonFactory">A function for creating a button for the drop down list.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        IComboBox GetComboBox(string id, IButton dropDownButton, ITextBox textBox, Func<IButton> itemButtonFactory, bool addToUi = true);

        /// <summary>
        /// Creates a slider
        /// </summary>
        /// <returns>The slider.</returns>
        /// <param name="id">A unique identifer for the slider (it has to be globally unique across all entities).</param>
        /// <param name="imagePath">Image resource/file path for the slider.</param>
        /// <param name="handleImagePath">Resource/file path for the handle's image (the item which you drag across the sliding line).</param>
        /// <param name="value">The slider's initial value.</param>
        /// <param name="min">The slider's minimum value.</param>
        /// <param name="max">The slider's maximum value.</param>
        /// <param name="config">A configuration for rendering the slider's value as text (if null than the slider's value will not be rendered as text, but just the handle will be shown).</param>
        /// <param name="loadConfig">Load configuration for the slider's images.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
        ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);

        /// <summary>
        /// Creates a slider asynchronously.
        /// </summary>
        /// <returns>The slider.</returns>
        /// <param name="id">A unique identifer for the slider (it has to be globally unique across all entities).</param>
        /// <param name="imagePath">Image resource/file path for the slider.</param>
        /// <param name="handleImagePath">Resource/file path for the handle's image (the item which you drag across the sliding line).</param>
        /// <param name="value">The slider's initial value.</param>
        /// <param name="min">The slider's minimum value.</param>
        /// <param name="max">The slider's maximum value.</param>
        /// <param name="config">A configuration for rendering the slider's value as text (if null than the slider's value will not be rendered as text, but just the handle will be shown).</param>
        /// <param name="loadConfig">Load configuration for the slider's images.</param>
        /// <param name="addToUi">If set to <c>true</c> add to game's GUI list for rendering.</param>
		Task<ISlider> GetSliderAsync(string id, string imagePath, string handleImagePath, float value, float min, float max,
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true);
	}
}

