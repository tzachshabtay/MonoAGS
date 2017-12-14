using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autofac;
using System.Diagnostics;

namespace AGS.Engine
{
	public class GLGraphicsFactory : IGraphicsFactory
	{
        private readonly Dictionary<string, ITexture> _textures;
        private readonly Resolver _resolver;
		private readonly IResourceLoader _resources;
		private readonly IBitmapLoader _bitmapLoader;
		private readonly SpriteSheetLoader _spriteSheetLoader;
        private readonly IRenderThread _renderThread;

        public GLGraphicsFactory (Dictionary<string, ITexture> textures, Resolver resolver, IGLUtils glUtils, 
                                  IGraphicsBackend graphics, IBitmapLoader bitmapLoader, IRenderThread renderThread,
                                  IResourceLoader resources, IIconFactory icons, IBrushLoader brushes, IRenderMessagePump messagePump)
		{
            Icons = icons;
            Brushes = brushes;
            _renderThread = renderThread;
			_textures = textures;
			_resolver = resolver;
			_resources = resources;
			_bitmapLoader = bitmapLoader;
            _spriteSheetLoader = new SpriteSheetLoader (_resources, _bitmapLoader, addAnimationFrame, loadImage, graphics, messagePump);
            
            AGSGameSettings.CurrentSkin = new AGSBlueSkin(this, glUtils).CreateSkin();
		}

        public IIconFactory Icons { get; private set; }

        public IBrushLoader Brushes { get; private set; }

		public ISprite GetSprite()
		{
			ISprite sprite = _resolver.Container.Resolve<ISprite>();
			return sprite;
		}

		public IDirectionalAnimation LoadDirectionalAnimationFromFolders(string baseFolder, string leftFolder = null,
			string rightFolder = null, string downFolder = null, string upFolder = null,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			if (leftFolder == null && rightFolder == null && downFolder == null && upFolder == null) return null;

			AGSDirectionalAnimation dirAnimation = new AGSDirectionalAnimation ();
			if (leftFolder != null) dirAnimation.Left = LoadAnimationFromFolder(baseFolder + leftFolder, animationConfig, loadConfig);
			if (rightFolder != null) dirAnimation.Right = LoadAnimationFromFolder(baseFolder + rightFolder, animationConfig, loadConfig);
			if (downFolder != null) dirAnimation.Down = LoadAnimationFromFolder(baseFolder + downFolder, animationConfig, loadConfig);
			if (upFolder != null) dirAnimation.Up = LoadAnimationFromFolder(baseFolder + upFolder, animationConfig, loadConfig);

			if (dirAnimation.Left != null && dirAnimation.Right == null)
			{
				dirAnimation.Right = createLeftRightAnimation(dirAnimation.Left);
			}

			if (dirAnimation.Right != null && dirAnimation.Left == null)
			{
				dirAnimation.Left = createLeftRightAnimation(dirAnimation.Right);
			}

			return dirAnimation;
		}

		public async Task<IDirectionalAnimation> LoadDirectionalAnimationFromFoldersAsync(string baseFolder, string leftFolder = null,
			string rightFolder = null, string downFolder = null, string upFolder = null,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			if (leftFolder == null && rightFolder == null && downFolder == null && upFolder == null) return null;

			AGSDirectionalAnimation dirAnimation = new AGSDirectionalAnimation ();
			if (leftFolder != null) dirAnimation.Left = await LoadAnimationFromFolderAsync(baseFolder + leftFolder, animationConfig, loadConfig);
			if (rightFolder != null) dirAnimation.Right = await LoadAnimationFromFolderAsync(baseFolder + rightFolder, animationConfig, loadConfig);
			if (downFolder != null) dirAnimation.Down = await LoadAnimationFromFolderAsync(baseFolder + downFolder, animationConfig, loadConfig);
			if (upFolder != null) dirAnimation.Up = await LoadAnimationFromFolderAsync(baseFolder + upFolder, animationConfig, loadConfig);

			if (dirAnimation.Left != null && dirAnimation.Right == null) {
				dirAnimation.Right = createLeftRightAnimation (dirAnimation.Left);
			}

			if (dirAnimation.Right != null && dirAnimation.Left == null) {
				dirAnimation.Left = createLeftRightAnimation (dirAnimation.Right);
			}

			return dirAnimation;
		}

		public IAnimation LoadAnimationFromFiles(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files)
		{
            if (files.Length == 0) throw new InvalidOperationException("No files given to LoadAnimationFromFiles");
			return loadAnimationFromResources(files[0], _resources.LoadResources(files), animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFilesAsync(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string [] files)
		{
            if (files.Length == 0) throw new InvalidOperationException("No files given to LoadAnimationFromFilesAsync");
			return await loadAnimationFromResourcesAsync(files[0], await Task.Run(() => _resources.LoadResources (files)), 
			                                             animationConfig, loadConfig);
		}

		public IAnimation LoadAnimationFromFolder (string folderPath, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return loadAnimationFromResources(folderPath, _resources.LoadResources(folderPath), animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await loadAnimationFromResourcesAsync (folderPath, await Task.Run(() => _resources.LoadResources (folderPath)), 
			                                   animationConfig, loadConfig);
		}

		public IAnimation LoadAnimationFromSpriteSheet (ISpriteSheet spriteSheet, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return _spriteSheetLoader.LoadAnimationFromSpriteSheet (spriteSheet, animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromSpriteSheetAsync (ISpriteSheet spriteSheet, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await _spriteSheetLoader.LoadAnimationFromSpriteSheetAsync (spriteSheet, animationConfig, loadConfig);
		}
			
		public IImage LoadImage(IBitmap bitmap, ILoadImageConfig config = null, string id = null)
		{
            return loadImage(bitmap, config, id);
		}

        public IImage LoadImage(string path, ILoadImageConfig config = null)
		{
			IResource resource = _resources.LoadResource(path);
            return loadImage(resource, config);
		}

		public async Task<IImage> LoadImageAsync (string filePath, ILoadImageConfig config = null)
		{
			IResource resource = await Task.Run(() => _resources.LoadResource(filePath));
            return await loadImageAsync(resource, config);
		}

		private IAnimation createLeftRightAnimation(IAnimation animation)
		{
			foreach (var frame in animation.Frames)
			{
				frame.Sprite.Anchor = new PointF (0.5f, 0f);
			}
			IAnimation clone = animation.Clone();
			clone.FlipHorizontally();
			return clone;
		}
			
		private IAnimation loadAnimationFromResources(string samplePath, List<IResource> resources, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			AGSAnimation animation = getAnimation (samplePath, animationConfig, resources.Count);
			foreach (IResource resource in resources) 
			{
				var image = loadImage (resource, loadConfig);
				addAnimationFrame (image, animation);
			}
			animation.Setup ();
			return animation;
		}

		private async Task<IAnimation> loadAnimationFromResourcesAsync (string samplePath, List<IResource> resources,
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			AGSAnimation animation = getAnimation (samplePath, animationConfig, resources.Count);

			foreach (IResource resource in resources) 
			{
				var image = await loadImageAsync (resource, loadConfig);
				addAnimationFrame (image, animation);
			}
			animation.Setup ();
			return animation;
		}

		private AGSAnimation getAnimation (string samplePath, IAnimationConfiguration animationConfig, int resourcesCount)
		{
            if (resourcesCount == 0 && samplePath != null)
            {
                throw new InvalidOperationException($"Failed to load animation from: {samplePath}");
            }
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			AGSAnimationState state = new AGSAnimationState ();
			AGSAnimation animation = new AGSAnimation (animationConfig, state, resourcesCount);
			return animation;
		}

        private void addAnimationFrame (IImage image, AGSAnimation animation)
		{
			if (image == null) return;
			ISprite sprite = GetSprite ();
			sprite.Image = image;
			AGSAnimationFrame frame = new AGSAnimationFrame (sprite);
			animation.Frames.Add (frame);
		}

        private IImage loadImage(IBitmap bitmap, ILoadImageConfig config = null, string id = null)
        {
            id = id ?? Guid.NewGuid().ToString();
            IImage image = null;
            _renderThread.RunBlocking(() =>
            {
                ITexture tex = createTexture(config);
                image = loadImage(tex, bitmap, id, config, null);
            });
            return image;
        }

        private IImage loadImage(IResource resource, ILoadImageConfig config = null)
		{
            IImage image = null;
            _renderThread.RunBlocking(() =>
            {
                ITexture tex = createTexture(config);
                try
                {
                    IBitmap bitmap = _bitmapLoader.Load(resource.Stream);
                    image = loadImage(tex, bitmap, resource.ID, config, null);
                }
                catch (ArgumentException e)
                {
                    Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e.ToString());
                }
            });
            return image;
		}

        private async Task<IImage> loadImageAsync (IResource resource, ILoadImageConfig config = null)
		{
            IBitmap bitmap;
			try 
			{
                if (resource == null) return new EmptyImage(1f, 1f);
				bitmap = await Task.Run(() => _bitmapLoader.Load (resource.Stream));
			} 
			catch (ArgumentException e) 
			{
				Debug.WriteLine ("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e.ToString ());
				return null;
			}
            IImage image = null;
            _renderThread.RunBlocking(() =>
            {
                ITexture tex = createTexture(config);
                image = loadImage(tex, bitmap, resource.ID, config, null);
            });
            return image;
		}

        private IImage loadImage(ITexture texture, IBitmap bitmap, string id, ILoadImageConfig config, ISpriteSheet spriteSheet)
		{
			manipulateImage(bitmap, config);
			bitmap.LoadTexture(null);
			GLImage image = new GLImage (bitmap, id, texture, spriteSheet, config);

            string imageId = image.ID;
			_textures?.GetOrAdd (imageId, () => image.Texture);
            image.OnImageDisposed.Subscribe(() => _textures.Remove(imageId));
			return image;
		}

		private void manipulateImage(IBitmap bitmap, ILoadImageConfig config)
		{
			if (config == null) return;
			if (config.TransparentColorSamplePoint != null)
			{
				Color transparentColor = bitmap.GetPixel(config.TransparentColorSamplePoint.Value.X,
					config.TransparentColorSamplePoint.Value.Y);
				bitmap.MakeTransparent(transparentColor);
			}
		}

        private ITexture createTexture(ILoadImageConfig config)
        {
            ITextureConfig textureConfig = config == null ? null : config.TextureConfig;
            TypedParameter textureConfigParam = new TypedParameter(typeof(ITextureConfig), textureConfig);
            return _resolver.Container.Resolve<ITexture>(textureConfigParam);
        }
	}
}

