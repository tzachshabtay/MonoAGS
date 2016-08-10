using System.Threading.Tasks;
using AGS.API;

using Autofac;
using System;

namespace AGS.Engine
{
	public class AGSUIFactory : IUIFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;
		private IGraphicsFactory _graphics;
		private IObjectFactory _object;

		public AGSUIFactory(IContainer resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj)
		{
			_resolver = resolver;
			_gameState = gameState;
			_graphics = graphics;
			_object = obj;
		}

		public IPanel GetPanel(string id, IImage image, float x, float y, bool addToUi = true)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter imageParameter = new TypedParameter (typeof(IImage), image);
			IPanel panel = _resolver.Resolve<IPanel>(idParam, imageParameter);
			panel.X = x;
			panel.Y = y;
			if (addToUi)
				_gameState.UI.Add(panel);
			return panel;
		}

		public IPanel GetPanel(string id, float width, float height, float x, float y, bool addToUi = true)
		{
			EmptyImage image = new EmptyImage (width, height);
			return GetPanel(id, image, x, y, addToUi);
		}

		public IPanel GetPanel(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IImage image = _graphics.LoadImage(imagePath, loadConfig);
			return GetPanel(id, image, x, y, addToUi);
		}

		public async Task<IPanel> GetPanelAsync(string id, string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IImage image = await _graphics.LoadImageAsync(imagePath, loadConfig);
			return GetPanel (id, image, x, y, addToUi);
		}

		public ILabel GetLabel(string id, string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true)
		{
			AGS.API.SizeF baseSize = new AGS.API.SizeF(width, height);			
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			ILabel label = _resolver.Resolve<ILabel>(idParam);
            label.LabelRenderSize = baseSize;
			label.Text = text;
			label.X = x;
			label.Y = y;
			label.Tint =  Colors.Transparent;
			label.TextConfig = config ?? new AGSTextConfig();
			if (addToUi)
				_gameState.UI.Add(label);
			return label;
		}

		public IButton GetButton(string id, IAnimation idle, IAnimation hovered, IAnimation pushed, 
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true,
			float width = -1f, float height = -1f)
		{
			if (width == -1f && idle != null && idle.Frames.Count > 0)
			{
				width = idle.Frames[0].Sprite.Width;
			}
			if (height == -1f && idle != null && idle.Frames.Count > 0)
			{
				height = idle.Frames[0].Sprite.Height;
			}
			TypedParameter idParam = new TypedParameter (typeof(string), id);			
			IButton button = _resolver.Resolve <IButton>(idParam);
            button.LabelRenderSize = new AGS.API.SizeF(width, height);
            if (idle != null && idle.Frames.Count > 0) button.IdleAnimation = idle;
            if (hovered != null && hovered.Frames.Count > 0) button.HoverAnimation = hovered;
            if (pushed != null && pushed.Frames.Count > 0) button.PushedAnimation = pushed;

            button.StartAnimation(button.IdleAnimation);
			button.Tint =  Colors.White;
			button.X = x;
			button.Y = y;
			button.TextConfig = config;
			button.Text = text;

			if (addToUi)
				_gameState.UI.Add(button);

			return button;
		}

		public IButton GetButton(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true,
			float width = -1f, float height = -1f)
		{
			IAnimation idle = _graphics.LoadAnimationFromFiles(files: new[]{ idleImagePath });
			IAnimation hovered = _graphics.LoadAnimationFromFiles(files: new[]{ hoveredImagePath });
			IAnimation pushed = _graphics.LoadAnimationFromFiles(files: new[]{ pushedImagePath });

			return GetButton(id, idle, hovered, pushed, x, y, text, config, addToUi, width, height);
		}

		public async Task<IButton> GetButtonAsync(string id, string idleImagePath, string hoveredImagePath, string pushedImagePath,
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true,
			float width = -1f, float height = -1f)
		{
			IAnimation idle = await _graphics.LoadAnimationFromFilesAsync(files: new [] { idleImagePath });
			IAnimation hovered = await _graphics.LoadAnimationFromFilesAsync(files: new [] { hoveredImagePath });
			IAnimation pushed = await _graphics.LoadAnimationFromFilesAsync(files: new [] { pushedImagePath });

			return GetButton (id, idle, hovered, pushed, x, y, text, config, addToUi, width, height);
		}

        public ITextBox GetTextBox(string id, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, 
            float width = -1F, float height = -1F)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);            
            ITextBox textbox = _resolver.Resolve<ITextBox>(idParam);
            textbox.LabelRenderSize = new SizeF(width, height);
            textbox.X = x;
            textbox.Y = y;
            if (width < 0f && config == null)
            {
                config = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText);
            }
            textbox.TextConfig = config;
            textbox.Text = text;
            
            if (addToUi)
                _gameState.UI.Add(textbox);

            return textbox;
        }

        public ICheckBox GetCheckBox(string id, IAnimation notChecked, IAnimation notCheckedHovered, IAnimation @checked, IAnimation checkedHovered, 
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            if (width == -1f && notChecked != null && notChecked.Frames.Count > 0)
            {
                width = notChecked.Frames[0].Sprite.Width;
            }
            if (height == -1f && notChecked != null && notChecked.Frames.Count > 0)
            {
                height = notChecked.Frames[0].Sprite.Height;
            }
            TypedParameter idParam = new TypedParameter(typeof(string), id);            
            ICheckBox checkbox = _resolver.Resolve<ICheckBox>(idParam);
            checkbox.TextConfig = config;
            checkbox.Text = text;
            if (!isCheckButton)
            {
                checkbox.SkinTags.Add(AGSSkin.CheckBoxTag);
                checkbox.Skin.Apply(checkbox);
            }
            checkbox.LabelRenderSize = new AGS.API.SizeF(width, height);
            if (notChecked != null) checkbox.NotCheckedAnimation = notChecked;
            if (notCheckedHovered != null) checkbox.HoverNotCheckedAnimation = notCheckedHovered;
            if (@checked != null) checkbox.CheckedAnimation = @checked;
            if (checkedHovered != null) checkbox.HoverCheckedAnimation = checkedHovered;
            
            checkbox.StartAnimation(checkbox.NotCheckedAnimation);
            checkbox.Tint = Colors.White;
            checkbox.X = x;
            checkbox.Y = y;
            
            if (addToUi)
                _gameState.UI.Add(checkbox);

            return checkbox;
        }

        public ICheckBox GetCheckBox(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath, 
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            IAnimation notChecked = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = _graphics.LoadAnimationFromFiles(files: new[] { checkedPath });
            IAnimation checkedHovered = _graphics.LoadAnimationFromFiles(files: new[] { checkedHoveredPath });

            return GetCheckBox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, text, config, addToUi, width, height, isCheckButton);
        }

        public async Task<ICheckBox> GetCheckBoxAsync(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath, 
            float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F, bool isCheckButton = false)
        {
            IAnimation notChecked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedPath });
            IAnimation checkedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedHoveredPath });

            return GetCheckBox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, text, config, addToUi, width, height, isCheckButton);
        }

        public IComboBox GetComboBox(string id, IButton dropDownButton, ITextBox textBox, 
            Func<IButton> itemButtonFactory, bool addToUi = true)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IComboBox comboBox = _resolver.Resolve<IComboBox>(idParam);
            float defaultHeight = dropDownButton != null ? dropDownButton.Height : textBox != null ? textBox.Height : 20f;
            float itemWidth = textBox != null ? textBox.Width : 100f;
            dropDownButton = dropDownButton ?? GetButton(id + "_DropDownButton", (string)null, null, null, 0f, 0f, width: 20f, height: defaultHeight);
            dropDownButton.SkinTags.Add(AGSSkin.DropDownButtonTag);
            dropDownButton.Skin.Apply(dropDownButton);
            textBox = textBox ?? GetTextBox(id + "_TextBox", 0f, 0f, width: itemWidth, height: defaultHeight);
            textBox.Enabled = false;
            itemButtonFactory = itemButtonFactory ?? (() => GetButton(id + "_" + Guid.NewGuid().ToString(), (string)null, null, null, 0f, 0f, width: itemWidth,
                height: defaultHeight));
            comboBox.DropDownButton = dropDownButton;
            comboBox.TextBox = textBox;            
            comboBox.ItemButtonFactory = itemButtonFactory;

            comboBox.TreeNode.AddChild(textBox);
            comboBox.TreeNode.AddChild(dropDownButton);

            if (addToUi)
            {
                _gameState.UI.Add(comboBox);
            }

            return comboBox;
        }

        public ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			var image = _graphics.LoadImage(imagePath, loadConfig);
			var handleImage = _graphics.LoadImage(handleImagePath, loadConfig);
			return getSlider(id, image, handleImage, value, min, max, config, addToUi);
		}

		public async Task<ISlider> GetSliderAsync(string id, string imagePath, string handleImagePath, float value, float min, float max,
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			var image = await _graphics.LoadImageAsync(imagePath, loadConfig);
			var handleImage = await _graphics.LoadImageAsync(handleImagePath, loadConfig);
			return getSlider(id, image, handleImage, value, min, max, config, addToUi);
		}

		private ISlider getSlider (string id, IImage image, IImage handleImage, float value, float min, float max,
			ITextConfig config = null, bool addToUi = true)
		{
			IObject graphics = _object.GetObject (string.Format ("{0}(graphics)", id));
			graphics.Image = image;
			graphics.IgnoreViewport = true;
			ILabel label = null;
			if (config != null) {
				label = GetLabel (string.Format ("{0}(label)", id), "", graphics.Width, 30f, 0f, -30f, config, false);
				label.Anchor = new PointF (0.5f, 0f);
			}

			IObject handle = _object.GetObject (string.Format ("{0}(handle)", id));
			handle.Image = handleImage;
			handle.IgnoreViewport = true;

			TypedParameter idParam = new TypedParameter (typeof (string), id);
			ISlider slider = _resolver.Resolve<ISlider> (idParam, idParam);
			slider.Label = label;
			slider.MinValue = min;
			slider.MaxValue = max;
			slider.Value = value;
			slider.Graphics = graphics;
			slider.HandleGraphics = handle;
			slider.IgnoreViewport = true;

			if (addToUi)
				_gameState.UI.Add (slider);
			return slider;
		}       
    }
}

