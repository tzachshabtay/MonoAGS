using System;
using AGS.API;
using Autofac;
using System.Drawing;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;

		public AGSGameFactory(IGraphicsFactory graphics, IGameState state, IContainer resolver)
		{
			Graphics = graphics;
			_resolver = resolver;
			_gameState = state;
		}

		#region IGameFactory implementation

		public IOutfit LoadOutfitFromFolders(string baseFolder, string walkLeftFolder = null, string walkRightFolder = null,
			string walkDownFolder = null, string walkUpFolder = null, string idleLeftFolder = null, string idleRightFolder = null,
			string idleDownFolder = null, string idleUpFolder = null, string speakLeftFolder = null, string speakRightFolder = null,
			string speakDownFolder = null, string speakUpFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			IOutfit outfit = _resolver.Resolve<IOutfit>();

			outfit.IdleAnimation = Graphics.LoadDirectionalAnimationFromFolders(baseFolder, idleLeftFolder, idleRightFolder, 
				idleDownFolder, idleUpFolder, delay, animationConfig, loadConfig);

			outfit.WalkAnimation = Graphics.LoadDirectionalAnimationFromFolders(baseFolder, walkLeftFolder, walkRightFolder, 
				walkDownFolder, walkUpFolder, delay, animationConfig, loadConfig);

			outfit.SpeakAnimation = Graphics.LoadDirectionalAnimationFromFolders(baseFolder, speakLeftFolder, speakRightFolder, 
				speakDownFolder, speakUpFolder, delay, animationConfig, loadConfig);

			return outfit;
		}

		public int GetInt(string name, int defaultValue = 0)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			throw new NotImplementedException();
		}

		public string GetString(string name, string defaultValue = null)
		{
			throw new NotImplementedException();
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
			IImage image = Graphics.LoadImage(imagePath, loadConfig);
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
			IAnimation idle = Graphics.LoadAnimationFromFiles(files: new[]{ idleImagePath });
			IAnimation hovered = Graphics.LoadAnimationFromFiles(files: new[]{ hoveredImagePath });
			IAnimation pushed = Graphics.LoadAnimationFromFiles(files: new[]{ pushedImagePath });

			return GetButton(idle, hovered, pushed, x, y, text, config, addToUi, width, height);
		}

		public ISlider GetSlider(string imagePath, string handleImagePath, float value, float min, float max, 
			ITextConfig config = null, ILoadImageConfig loadConfig = null, bool addToUi = true)
		{
			IObject graphics = GetObject();
			graphics.Image = Graphics.LoadImage(imagePath, loadConfig);
			ILabel label = null;
			if (config != null)
			{
				label = GetLabel("", graphics.Width, 30f, 0f, -30f, config, false);
				label.Anchor = new AGSPoint (0.5f, 0f);
			}

			IObject handle = GetObject();
			handle.Image = Graphics.LoadImage(handleImagePath, loadConfig);

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

		public IObject GetObject(string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			IObject obj = _resolver.Resolve<IObject>();

			subscribeSentences(sayWhenLook, obj.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, obj.Interactions.OnInteract);

			return obj;
		}

		public ICharacter GetCharacter(IOutfit outfit, string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			TypedParameter outfitParam = new TypedParameter (typeof(IOutfit), outfit);
			ICharacter character = _resolver.Resolve<ICharacter>(outfitParam);

			subscribeSentences(sayWhenLook, character.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, character.Interactions.OnInteract);

			return character;
		}

		public IObject GetHotspot(string maskPath, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			Bitmap image = (Bitmap)Image.FromFile(maskPath); 
			return GetHotspot(image, hotspot, sayWhenLook, sayWhenInteract);
		}

		public IObject GetHotspot(Bitmap maskBitmap, string hotspot, 
			string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			IMaskLoader maskLoader = _resolver.Resolve<IMaskLoader>();
			IMask mask = maskLoader.Load(maskBitmap, debugDrawColor: Color.White);
			mask.DebugDraw.PixelPerfect(true);
			mask.DebugDraw.Hotspot = hotspot;
			mask.DebugDraw.Opacity = 0;
			mask.DebugDraw.Z = mask.MinY;

			subscribeSentences(sayWhenLook, mask.DebugDraw.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, mask.DebugDraw.Interactions.OnInteract);

			return mask.DebugDraw;
		}

		public IEdge GetEdge(float value = 0f)
		{
			IEdge edge = _resolver.Resolve<IEdge>();
			edge.Value = value;
			return edge;
		}

		public IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float topEdge = 0f, float bottomEdge = 0f)
		{
			AGSEdges edges = new AGSEdges (GetEdge(leftEdge), GetEdge(rightEdge), GetEdge(topEdge), GetEdge(bottomEdge));
			TypedParameter edgeParam = new TypedParameter (typeof(IAGSEdges), edges);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			IRoom room = _resolver.Resolve<IRoom>(idParam, edgeParam);
			room.Viewport.Follower = _resolver.Resolve<IFollower>();
			IPlayer player = _resolver.Resolve<IPlayer>();
			room.Viewport.Follower.Target = () => player.Character;
			return room;
		}

		public IInventoryWindow GetInventoryWindow(float width, float height, float itemWidth, float itemHeight, float x, float y,
			ICharacter character = null, bool addToUi = true)
		{
			IPanel panel = GetPanel(width, height, x, y, false);
			TypedParameter parameter = new TypedParameter (typeof(IPanel), panel);
			IInventoryWindow inventory = _resolver.Resolve<IInventoryWindow>(parameter);
			inventory.Tint = Color.Transparent;
			inventory.ItemSize = new SizeF (itemWidth, itemHeight);
			inventory.CharacterToUse = character ?? _resolver.Resolve<IPlayer>().Character;

			if (addToUi)
				_gameState.UI.Add(inventory);

			return inventory;
		}

		public IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false)
		{
			IInventoryItem item = _resolver.Resolve<IInventoryItem>();
			item.Graphics = graphics;
			item.CursorGraphics = cursorGraphics;

			if (playerStartsWithItem)
			{
				IPlayer player = _resolver.Resolve<IPlayer>();
				if (player.Character == null)
				{
					Debug.WriteLine("Character was not assigned to player yet, cannot add inventory item", graphics.ToString());
				}
				else player.Character.Inventory.Items.Add(item);
			}
			return item;
		}

		public IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, 
			ILoadImageConfig loadConfig = null, bool playerStartsWithItem = false)
		{
			IObject graphics = GetObject();
			graphics.Image = Graphics.LoadImage(graphicsFile, loadConfig);
			graphics.RenderLayer = AGSLayers.UI;
			graphics.IgnoreViewport = true;
			graphics.IgnoreScalingArea = true;
			graphics.Anchor = new AGSPoint (0.5f, 0.5f);
			graphics.Hotspot = hotspot;

			IObject cursor = GetObject();
			cursor.Image = cursorFile == null ? graphics.Image : Graphics.LoadImage(cursorFile, loadConfig);

			return GetInventoryItem(graphics, cursor, playerStartsWithItem);
		}

		public IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			bool speakOption = true, bool showOnce = false)
		{
			IGame game = _resolver.Resolve<IGame>();
			if (config == null) config = new AGSTextConfig (autoFit: AutoFit.TextShouldWrap,
					brush: Brushes.White, font: new Font(SystemFonts.DefaultFont.FontFamily, 10f));
			if (hoverConfig == null) hoverConfig = new AGSTextConfig (autoFit: AutoFit.TextShouldWrap,
				brush: Brushes.Yellow, font: new Font(SystemFonts.DefaultFont.FontFamily, 10f));
			ILabel label = GetLabel(text, game.VirtualResolution.Width, 20f, 0f, 0f, config);
			label.Enabled = true;
			TypedParameter labelParam = new TypedParameter (typeof(ILabel), label);
			NamedParameter speakParam = new NamedParameter ("speakOption", speakOption);
			NamedParameter showOnceParam = new NamedParameter ("showOnce", showOnce);
			TypedParameter hoverParam = new TypedParameter (typeof(ITextConfig), hoverConfig);
			IDialogOption option = _resolver.Resolve<IDialogOption>(labelParam, speakParam, showOnceParam, hoverParam);
			return option;
		}

		public IDialog GetDialog(float x = 0f, float y = 0f, IObject graphics = null, bool showWhileOptionsAreRunning = false, 
			params IDialogOption[] options)
		{
			TypedParameter showParam = new TypedParameter (typeof(bool), showWhileOptionsAreRunning);
			if (graphics == null)
			{
				graphics = GetObject();
				graphics.Tint = Color.Black;
				graphics.Anchor = new AGSPoint ();
				_gameState.UI.Add(graphics);
			}
			TypedParameter graphicsParam = new TypedParameter (typeof(IObject), graphics);
			IDialog dialog = _resolver.Resolve<IDialog>(showParam, graphicsParam);
			foreach (IDialogOption option in options)
			{
				dialog.Options.Add(option);
			}
			return dialog;
		}

		public void RegisterCustomData(ICustomSerializable customData)
		{
			throw new NotImplementedException();
		}

		public IGraphicsFactory Graphics { get; private set; }

		public ISoundFactory Sound
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		private void subscribeSentences(string[] sentences, IEvent<ObjectEventArgs> e)
		{
			if (sentences == null || e == null) return;

			e.SubscribeToAsync(async (sender, args) =>
			{
				foreach (string sentence in sentences)
				{
					await _gameState.Player.Character.SayAsync(sentence);
				}
			});
		}
	}
}

