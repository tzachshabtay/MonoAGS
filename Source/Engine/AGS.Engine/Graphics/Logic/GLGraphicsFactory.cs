﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class GLGraphicsFactory : IGraphicsFactory
	{
        private readonly ITextureCache _textures;
        private readonly Resolver _resolver;
		private readonly IResourceLoader _resources;
		private readonly IBitmapLoader _bitmapLoader;
		private readonly SpriteSheetLoader _spriteSheetLoader;
        private readonly IRenderThread _renderThread;

        public GLGraphicsFactory (ITextureCache textures, Resolver resolver,
                                  IGraphicsBackend graphics, IBitmapLoader bitmapLoader, IRenderThread renderThread,
                                  IResourceLoader resources, IIconFactory icons, IBrushLoader brushes, 
                                  IRenderMessagePump messagePump, IGameSettings settings, IBorderFactory borders)
		{
            Icons = icons;
            Brushes = brushes;
            Borders = borders;
            _renderThread = renderThread;
			_textures = textures;
			_resolver = resolver;
			_resources = resources;
			_bitmapLoader = bitmapLoader;
            _spriteSheetLoader = new SpriteSheetLoader (_resources, _bitmapLoader, addAnimationFrame, loadImage, graphics, messagePump);
            
            settings.Defaults.Skin = new AGSBlueSkin(this).CreateSkin();
		}

        public IBorderFactory Borders { get; }

        public IIconFactory Icons { get; }

        public IBrushLoader Brushes { get; }

		public ISprite GetSprite()
		{
			ISprite sprite = _resolver.Container.Resolve<ISprite>();
			return sprite;
		}

        public ISprite LoadSprite(string path, ILoadImageConfig loadConfig = null)
        {
            var sprite = GetSprite();
            var image = LoadImage(path, loadConfig);
            sprite.Image = image;
            return sprite;
        }

        public async Task<ISprite> LoadSpriteAsync(string path, ILoadImageConfig loadConfig = null)
        {
            var sprite = GetSprite();
            var image = await LoadImageAsync(path, loadConfig);
            sprite.Image = image;
            return sprite;
        }

        public ISprite LoadSprite(IBitmap bitmap, ILoadImageConfig loadConfig = null, string id = null)
        {
            var sprite = GetSprite();
            var image = LoadImage(bitmap, loadConfig, id);
            sprite.Image = image;
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
			return loadAnimationFromResources(files[0], _resources.LoadResourcesFromPaths(files), animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFilesAsync(IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string [] files)
		{
            if (files.Length == 0) throw new InvalidOperationException("No files given to LoadAnimationFromFilesAsync");
			return await loadAnimationFromResourcesAsync(files[0], await Task.Run(() => _resources.LoadResourcesFromPaths(files)), 
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

        public IBitmap GetBitmap(int width, int height)
        {
            return _bitmapLoader.Load(width, height);
        }

        public IBitmap LoadBitmap(string path)
        {
            IResource resource = _resources.LoadResource(path);
            return loadBitmap(resource);
        }

        public async Task<IBitmap> LoadBitmapAsync(string path)
        {
            IResource resource = await Task.Run(() => _resources.LoadResource(path));
            return await loadBitmapAsync(resource);
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
				frame.Sprite.Pivot = new PointF (0.5f, 0f);
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

        private IBitmap loadBitmap(IResource resource)
        {
            IBitmap bitmap = null;
            _renderThread.RunBlocking(() =>
            {
                try
                {
                    bitmap = _bitmapLoader.Load(resource.Stream);
                }
                catch (ArgumentException e)
                {
                    Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e);
                }
            });
            return bitmap;
        }

        private async Task<IBitmap> loadBitmapAsync(IResource resource)
        {
            if (resource == null)
                return _bitmapLoader.Load(1, 1);
            try
            {
                return await Task.Run(() => _bitmapLoader.Load(resource.Stream));
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e);
                return null;
            }
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
            if (resource == null)
            {
                return null;
            }
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
                    Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e);
                }
            });
            return image;
		}

        private async Task<IImage> loadImageAsync (IResource resource, ILoadImageConfig config = null)
		{
		    if (resource == null) return new EmptyImage(1f, 1f);
            IBitmap bitmap;
			try 
			{
				bitmap = await Task.Run(() => _bitmapLoader.Load (resource.Stream));
			} 
			catch (ArgumentException e) 
			{
				Debug.WriteLine ("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e);
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
            _textures?.GetTexture(imageId, _ => image.Texture);
            image.OnImageDisposed.Subscribe(() => _textures.RemoveTexture(imageId));
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
