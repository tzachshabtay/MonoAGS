using System.Threading.Tasks;
using AGS.API;

using Autofac;

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
			if (width == -1f)
			{
				width = idle.Frames[0].Sprite.Width;
			}
			if (height == -1f)
			{
				height = idle.Frames[0].Sprite.Height;
			}
			TypedParameter idParam = new TypedParameter (typeof(string), id);			
			IButton button = _resolver.Resolve <IButton>(idParam);
            button.LabelRenderSize = new AGS.API.SizeF(width, height);
            button.IdleAnimation = idle;
			button.HoverAnimation = hovered;
			button.PushedAnimation = pushed;

			button.StartAnimation(idle);
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

        public ITextbox GetTextbox(string id, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, 
            float width = -1F, float height = -1F)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);            
            ITextbox textbox = _resolver.Resolve<ITextbox>(idParam);
            textbox.LabelRenderSize = new SizeF(width, height);
            textbox.X = x;
            textbox.Y = y;
            textbox.TextConfig = config;
            textbox.Text = text;

            if (addToUi)
                _gameState.UI.Add(textbox);

            return textbox;
        }

        public ICheckbox GetCheckbox(string id, IAnimation notChecked, IAnimation notCheckedHovered, IAnimation @checked, IAnimation checkedHovered, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F)
        {
            if (width == -1f)
            {
                width = notChecked.Frames[0].Sprite.Width;
            }
            if (height == -1f)
            {
                height = notChecked.Frames[0].Sprite.Height;
            }
            TypedParameter idParam = new TypedParameter(typeof(string), id);            
            ICheckbox checkbox = _resolver.Resolve<ICheckbox>(idParam);
            checkbox.LabelRenderSize = new AGS.API.SizeF(width, height);
            checkbox.NotCheckedAnimation = notChecked;
            checkbox.HoverNotCheckedAnimation = notCheckedHovered;
            checkbox.CheckedAnimation = @checked;
            checkbox.HoverCheckedAnimation = checkedHovered;

            checkbox.StartAnimation(notChecked);
            checkbox.Tint = Colors.White;
            checkbox.X = x;
            checkbox.Y = y;
            checkbox.TextConfig = config;
            checkbox.Text = text;

            if (addToUi)
                _gameState.UI.Add(checkbox);

            return checkbox;
        }

        public ICheckbox GetCheckbox(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F)
        {
            IAnimation notChecked = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = _graphics.LoadAnimationFromFiles(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = _graphics.LoadAnimationFromFiles(files: new[] { checkedPath });
            IAnimation checkedHovered = _graphics.LoadAnimationFromFiles(files: new[] { checkedHoveredPath });

            return GetCheckbox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, text, config, addToUi, width, height);
        }

        public async Task<ICheckbox> GetCheckboxAsync(string id, string notCheckedPath, string notCheckedHoveredPath, string checkedPath, string checkedHoveredPath, float x, float y, string text = "", ITextConfig config = null, bool addToUi = true, float width = -1F, float height = -1F)
        {
            IAnimation notChecked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedPath });
            IAnimation notCheckedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { notCheckedHoveredPath });
            IAnimation @checked = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedPath });
            IAnimation checkedHovered = await _graphics.LoadAnimationFromFilesAsync(files: new[] { checkedHoveredPath });

            return GetCheckbox(id, notChecked, notCheckedHovered, @checked, checkedHovered, x, y, text, config, addToUi, width, height);
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

