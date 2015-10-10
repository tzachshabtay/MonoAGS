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

		public IPanel GetPanel(IImage image, float x, float y, bool addToUi = true)
		{
			TypedParameter typedParameter = new TypedParameter (typeof(IImage), image);
			IPanel panel = _resolver.Resolve<IPanel>(typedParameter);
			panel.X = x;
			panel.Y = y;
			if (addToUi)
				_gameState.UI.Add(panel);
			return panel;
		}

		public IPanel GetPanel(float width, float height, float x, float y, bool addToUi = true)
		{
			EmptyImage image = new EmptyImage (width, height);
			return GetPanel(image, x, y, addToUi);
		}

		public IPanel GetPanel(string imagePath, float x, float y, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IImage image = _graphics.LoadImage(imagePath, loadConfig);
			return GetPanel(image, x, y, addToUi);
		}

		public ILabel GetLabel(string text, float width, float height, float x, float y, ITextConfig config = null, bool addToUi = true)
		{
			SizeF baseSize = new SizeF(width, height);
			TypedParameter typedParameter = new TypedParameter (typeof(SizeF), baseSize);
			ILabel label = _resolver.Resolve<ILabel>(typedParameter);
			label.Text = text;
			label.X = x;
			label.Y = y;
			label.Tint = Color.Transparent;
			label.TextConfig = config ?? new AGSTextConfig();
			if (addToUi)
				_gameState.UI.Add(label);
			return label;
		}

		public IButton GetButton(IAnimation idle, IAnimation hovered, IAnimation pushed, 
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
			ILabel label = GetLabel(text, width, height, x, y, config, false);
			label.Tint = Color.White;

			TypedParameter typedParameter = new TypedParameter (typeof(ILabel), label);
			IButton button = _resolver.Resolve <IButton>(typedParameter);

			button.IdleAnimation = idle;
			button.HoverAnimation = hovered;
			button.PushedAnimation = pushed;

			button.StartAnimation(idle);

			if (addToUi)
				_gameState.UI.Add(button);

			return button;
		}

		public IButton GetButton(string idleImagePath, string hoveredImagePath, string pushedImagePath,
			float x, float y, string text = "", ITextConfig config = null, bool addToUi = true,
			float width = -1f, float height = -1f)
		{
			IAnimation idle = _graphics.LoadAnimationFromFiles(files: new[]{ idleImagePath });
			IAnimation hovered = _graphics.LoadAnimationFromFiles(files: new[]{ hoveredImagePath });
			IAnimation pushed = _graphics.LoadAnimationFromFiles(files: new[]{ pushedImagePath });

			return GetButton(idle, hovered, pushed, x, y, text, config, addToUi, width, height);
		}

		public ISlider GetSlider(string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IObject graphics = _object.GetObject();
			graphics.Image = _graphics.LoadImage(imagePath, loadConfig);
			ILabel label = null;
			if (config != null)
			{
				label = GetLabel("", graphics.Width, 30f, 0f, -30f, config, false);
				label.Anchor = new AGSPoint (0.5f, 0f);
			}

			IObject handle = _object.GetObject();
			handle.Image = _graphics.LoadImage(handleImagePath, loadConfig);

			ISlider slider = _resolver.Resolve<ISlider>();
			slider.Label = label;
			slider.MinValue = min;
			slider.MaxValue = max;
			slider.Value = value;
			slider.Graphics = graphics;
			slider.HandleGraphics = handle;

			if (addToUi)
				_gameState.UI.Add(slider);
			return slider;
		}


	}
}

