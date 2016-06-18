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
		private readonly Dictionary<string, GLImage> _textures;
		private readonly IContainer _resolver;
		private readonly IResourceLoader _resources;
		private readonly IBitmapLoader _bitmapLoader;
		private readonly SpriteSheetLoader _spriteSheetLoader;

		public GLGraphicsFactory (Dictionary<string, GLImage> textures, IContainer resolver)
		{
			this._textures = textures;
			this._resolver = resolver;
			this._resources = resolver.Resolve<IResourceLoader>();
			this._bitmapLoader = Hooks.BitmapLoader;
			this._spriteSheetLoader = new SpriteSheetLoader (_resources, _bitmapLoader, addAnimationFrame, loadImage);
		}

		public ISprite GetSprite()
		{
			ISprite sprite = _resolver.Resolve<ISprite>();
			return sprite;
		}

		public IDirectionalAnimation LoadDirectionalAnimationFromFolders(string baseFolder, string leftFolder = null,
			string rightFolder = null, string downFolder = null, string upFolder = null,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			if (leftFolder == null && rightFolder == null && downFolder == null && upFolder == null) return null;

			AGSDirectionalAnimation dirAnimation = new AGSDirectionalAnimation ();
			if (leftFolder != null) dirAnimation.Left = LoadAnimationFromFolder(baseFolder + leftFolder, delay, animationConfig, loadConfig);
			if (rightFolder != null) dirAnimation.Right = LoadAnimationFromFolder(baseFolder + rightFolder, delay, animationConfig, loadConfig);
			if (downFolder != null) dirAnimation.Down = LoadAnimationFromFolder(baseFolder + downFolder, delay, animationConfig, loadConfig);
			if (upFolder != null) dirAnimation.Up = LoadAnimationFromFolder(baseFolder + upFolder, delay, animationConfig, loadConfig);

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
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			if (leftFolder == null && rightFolder == null && downFolder == null && upFolder == null) return null;

			AGSDirectionalAnimation dirAnimation = new AGSDirectionalAnimation ();
			if (leftFolder != null) dirAnimation.Left = await LoadAnimationFromFolderAsync(baseFolder + leftFolder, delay, animationConfig, loadConfig);
			if (rightFolder != null) dirAnimation.Right = await LoadAnimationFromFolderAsync(baseFolder + rightFolder, delay, animationConfig, loadConfig);
			if (downFolder != null) dirAnimation.Down = await LoadAnimationFromFolderAsync(baseFolder + downFolder, delay, animationConfig, loadConfig);
			if (upFolder != null) dirAnimation.Up = await LoadAnimationFromFolderAsync(baseFolder + upFolder, delay, animationConfig, loadConfig);

			if (dirAnimation.Left != null && dirAnimation.Right == null) {
				dirAnimation.Right = createLeftRightAnimation (dirAnimation.Left);
			}

			if (dirAnimation.Right != null && dirAnimation.Left == null) {
				dirAnimation.Left = createLeftRightAnimation (dirAnimation.Right);
			}

			return dirAnimation;
		}

		public IAnimation LoadAnimationFromFiles(int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string[] files)
		{
			return loadAnimationFromResources(_resources.LoadResources(files), delay, animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFilesAsync(int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null, params string [] files)
		{
			return await loadAnimationFromResourcesAsync(await Task.Run(() => _resources.LoadResources (files)), 
			                                             delay, animationConfig, loadConfig);
		}

		public IAnimation LoadAnimationFromFolder (string folderPath, int delay = 4, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return loadAnimationFromResources(_resources.LoadResources(folderPath), delay, animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromFolderAsync (string folderPath, int delay = 4, 
			IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await loadAnimationFromResourcesAsync (await Task.Run(() => _resources.LoadResources (folderPath)), 
			                                   delay, animationConfig, loadConfig);
		}

		public IAnimation LoadAnimationFromSpriteSheet (ISpriteSheet spriteSheet, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return _spriteSheetLoader.LoadAnimationFromSpriteSheet (spriteSheet, delay, animationConfig, loadConfig);
		}

		public async Task<IAnimation> LoadAnimationFromSpriteSheetAsync (ISpriteSheet spriteSheet, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			return await _spriteSheetLoader.LoadAnimationFromSpriteSheetAsync (spriteSheet, delay, animationConfig, loadConfig);
		}
			
		public GLImage LoadImageInner(string path, ILoadImageConfig config = null)
		{
			IResource resource = _resources.LoadResource(path);
			return loadImage(resource, config);
		}

		public async Task<GLImage> LoadImageInnerAsync(string path, ILoadImageConfig config = null)
		{
			IResource resource = await Task.Run(() => _resources.LoadResource (path));
			return await loadImageAsync (resource, config);
		}

		public IImage LoadImage(IBitmap bitmap, ILoadImageConfig config = null, string id = null)
		{
			id = id ?? Guid.NewGuid ().ToString ();
			int tex = GLImage.CreateTexture();
			return loadImage (tex, bitmap, id, config, null);
		}

		public IImage LoadImage(string path, ILoadImageConfig config = null)
		{
			return LoadImageInner (path, config);
		}

		public async Task<IImage> LoadImageAsync (string filePath, ILoadImageConfig config = null)
		{
			return await LoadImageInnerAsync (filePath, config);
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
			
		private IAnimation loadAnimationFromResources(List<IResource> resources, 
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			AGSAnimation animation = getAnimation (animationConfig, resources.Count);

			foreach (IResource resource in resources) 
			{
				var image = loadImage (resource, loadConfig);
				addAnimationFrame (image, animation, delay);
			}
			animation.Setup ();
			return animation;
		}

		private async Task<IAnimation> loadAnimationFromResourcesAsync (List<IResource> resources,
			int delay = 4, IAnimationConfiguration animationConfig = null, ILoadImageConfig loadConfig = null)
		{
			AGSAnimation animation = getAnimation (animationConfig, resources.Count);

			foreach (IResource resource in resources) 
			{
				var image = await loadImageAsync (resource, loadConfig);
				addAnimationFrame (image, animation, delay);
			}
			animation.Setup ();
			return animation;
		}

		private AGSAnimation getAnimation (IAnimationConfiguration animationConfig, int resourcesCount)
		{
			animationConfig = animationConfig ?? new AGSAnimationConfiguration ();
			AGSAnimationState state = new AGSAnimationState ();
			AGSAnimation animation = new AGSAnimation (animationConfig, state, resourcesCount);
			return animation;
		}

		private void addAnimationFrame (GLImage image, AGSAnimation animation, int delay)
		{
			if (image == null) return;
			ISprite sprite = GetSprite ();
			sprite.Image = image;
			AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = delay };
			animation.Frames.Add (frame);
		}
			
		private GLImage loadImage(IResource resource, ILoadImageConfig config = null)
		{
			int tex = GLImage.CreateTexture();
			try
			{
				IBitmap bitmap = _bitmapLoader.Load(resource.Stream);
				return loadImage (tex, bitmap, resource.ID, config, null);
			}
			catch (ArgumentException e)
			{
				Debug.WriteLine("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e.ToString());
				return null;
			}
		}

		private async Task<GLImage> loadImageAsync (IResource resource, ILoadImageConfig config = null)
		{
			try 
			{
				IBitmap bitmap = await Task.Run(() => _bitmapLoader.Load (resource.Stream));
				int tex = GLImage.CreateTexture ();
				return loadImage(tex, bitmap, resource.ID, config, null);
			} 
			catch (ArgumentException e) 
			{
				Debug.WriteLine ("Failed to load image from {0}, is it really an image?\r\n{1}", resource.ID, e.ToString ());
				return null;
			}
		}

		private GLImage loadImage(int texture, IBitmap bitmap, string id, ILoadImageConfig config, ISpriteSheet spriteSheet)
		{
			manipulateImage(bitmap, config);
			bitmap.LoadTexture(null);
			GLImage image = new GLImage (bitmap, id, texture, spriteSheet, config);

			if (_textures != null)
				_textures.GetOrAdd (image.ID, () => image);
			return image;
		}

		private void manipulateImage(IBitmap bitmap, ILoadImageConfig config)
		{
			if (config == null) return;
			if (config.TransparentColorSamplePoint != null)
			{
				Color transparentColor = bitmap.GetPixel((int)config.TransparentColorSamplePoint.Value.X,
					(int)config.TransparentColorSamplePoint.Value.Y);
				bitmap.MakeTransparent(transparentColor);
			}
		}
	}
}

