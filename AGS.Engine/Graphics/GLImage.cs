using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;


namespace AGS.Engine
{
	public class GLImage : IImage
	{
		public GLImage ()
		{
			Width = 1f;
			Height = 1f;
			ID = "";
		}

		public GLImage(IBitmap bitmap, string id, int texture, ISpriteSheet spriteSheet, ILoadImageConfig loadConfig)
		{
			OriginalBitmap = bitmap;
			Width = bitmap.Width;
			Height = bitmap.Height;
			ID = id;
			Texture = texture;
			SpriteSheet = spriteSheet;
			LoadConfig = loadConfig;
		}

		public static int CreateTexture()
		{
			if (Environment.CurrentManagedThreadId != AGSGame.UIThreadID)
			{
				throw new InvalidOperationException ("Must generate textures on the UI thread");
			}
			int tex;
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.GenTextures(1, out tex);
			GL.BindTexture(TextureTarget.Texture2D, tex);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);

			return tex;
		}

		public IBitmap OriginalBitmap { get; private set; }

		public float Width { get; private set; }
		public float Height { get; private set; }

		public string ID { get; private set; }
		public int Texture { get; private set; }

		public ISpriteSheet SpriteSheet { get; private set; }
		public ILoadImageConfig LoadConfig { get; private set; }

		public override string ToString()
		{
			return ID ?? base.ToString();
		}
	}
}

