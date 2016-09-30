using System;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
	public class GLFrameBuffer : IFrameBuffer, IDisposable
	{
		private readonly int _fbo, _width, _height;

		public GLFrameBuffer(Size size)
		{
            _width = size.Width;
            _height = size.Height;
			Texture = GLImage.CreateTexture();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

			_fbo = GL.GenFramebuffer();
		}

		public int Texture { get; private set; }

		public bool Begin()
		{
			GL.BindTexture(TextureTarget.Texture2D, Texture);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
			GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture, 0);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			DrawBuffersEnum[] attachments = new[]{ DrawBuffersEnum.ColorAttachment0 };
			GL.DrawBuffers(1, attachments);

			var errorCode = GL.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
			if (errorCode != FramebufferErrorCode.FramebufferComplete || errorCode != FramebufferErrorCode.FramebufferCompleteExt)
			{
				Debug.WriteLine("Cannot create frame buffer. Error: " + errorCode.ToString());
				return false;
			}

			GL.Viewport(0, 0, _width, _height);
			return true;
		}

		public void End()
		{
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            AGSGame.Game.Settings.ResetViewport();
		}

		#region IDisposable implementation

		public void Dispose()
		{
			GL.DeleteFramebuffer(_fbo);
		}

		#endregion
	}
}

