using System;
using AGS.API;
using System.Drawing;
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
			IPanel panel = GetPanel(id, _resolver.Resolve<IAnimationContainer>(), image, x, y);
			if (addToUi)
				_gameState.UI.Add(panel);
			return panel;
		}

		public IPanel GetPanel(string id, IAnimationContainer innerContainer, IImage image, float x, float y)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter imageParameter = new TypedParameter (typeof(IImage), image);
			TypedParameter containerParameter = new TypedParameter (typeof(IAnimationContainer), innerContainer);
			IPanel panel = _resolver.Resolve<IPanel>(idParam, containerParameter, imageParameter);
			panel.X = x;
			panel.Y = y;
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

		public ILabel GetLabel(string id, string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true)
		{
			ILabel label = GetLabel(id, _resolver.Resolve<IAnimationContainer>(), text, width, height, x, y, config);
			if (addToUi)
				_gameState.UI.Add(label);
			return label;
		}

		public ILabel GetLabel(string id, IAnimationContainer innerContainer, string text, float width, float height, float x, float y, ITextConfig config = null)
		{
			SizeF baseSize = new SizeF(width, height);
			TypedParameter sizeParam = new TypedParameter (typeof(SizeF), baseSize);
			TypedParameter containerParam = new TypedParameter (typeof(IAnimationContainer), innerContainer);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			ILabel label = _resolver.Resolve<ILabel>(idParam, sizeParam, containerParam);
			label.Text = text;
			label.X = x;
			label.Y = y;
			label.Tint = Color.Transparent;
			label.TextConfig = config ?? new AGSTextConfig();
			return label;
		}

		public IButton GetButton(ILabel innerLabel, IAnimation idle, IAnimation hovered, IAnimation pushed)
		{
			TypedParameter typedParameter = new TypedParameter (typeof(ILabel), innerLabel);
			TypedParameter idParameter = new TypedParameter (typeof(string), innerLabel.ID);
			IButton button = _resolver.Resolve <IButton>(idParameter, typedParameter);

			button.IdleAnimation = idle;
			button.HoverAnimation = hovered;
			button.PushedAnimation = pushed;

			button.StartAnimation(idle);
			return button;
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
			ILabel label = GetLabel(id, text, width, height, x, y, config, false);
			label.Tint = Color.White;

			IButton button = GetButton(label, idle, hovered, pushed);

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

		public ISlider GetSlider(string id, string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IObject graphics = _object.GetObject(string.Format("{0}(graphics)", id));
			graphics.Image = _graphics.LoadImage(imagePath, loadConfig);
			graphics.IgnoreViewport = true;
			ILabel label = null;
			if (config != null)
			{
				label = GetLabel(string.Format("{0}(label)", id), "", graphics.Width, 30f, 0f, -30f, config, false);
				label.Anchor = new AGSPoint (0.5f, 0f);
			}

			IObject handle = _object.GetObject(string.Format("{0}(handle)", id));
			handle.Image = _graphics.LoadImage(handleImagePath, loadConfig);
			handle.IgnoreViewport = true;

			TypedParameter idParam = new TypedParameter (typeof(string), id);
			IPanel panelInner = _resolver.Resolve<IPanel>(idParam);
			TypedParameter panelParam = new TypedParameter (typeof(IPanel), panelInner);
			ISlider slider = _resolver.Resolve<ISlider>(idParam, panelParam);
			slider.Label = label;
			slider.MinValue = min;
			slider.MaxValue = max;
			slider.Value = value;
			slider.Graphics = graphics;
			slider.HandleGraphics = handle;
			slider.IgnoreViewport = true;

			if (addToUi)
				_gameState.UI.Add(slider);
			return slider;
		}
	}
}

