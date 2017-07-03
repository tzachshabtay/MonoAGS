using System;
using AGS.API;

namespace AGS.Engine
{
	public class GLFrameBuffer : IFrameBuffer, IDisposable
	{
		private readonly int _fbo, _width, _height;
        private readonly IGraphicsBackend _graphics;
        private readonly IMessagePump _messagePump;

        public GLFrameBuffer(Size size, IGraphicsBackend graphics, IMessagePump messagePump)
		{
            _width = size.Width;
            _height = size.Height;
            Texture = new GLTexture(null, graphics, messagePump);
            _graphics = graphics;
            _graphics.TexImage2D(_width, _height, IntPtr.Zero);

			_fbo = _graphics.GenFrameBuffer();
		}

        public ITexture Texture { get; private set; }

		public bool Begin()
		{
            _graphics.BindTexture2D(Texture.ID);
			_graphics.BindFrameBuffer(_fbo);
            _graphics.FrameBufferTexture2D(Texture.ID);
            _graphics.BindTexture2D(0);
            if (!_graphics.DrawFrameBuffer()) return false;

            _graphics.Viewport(0, 0, _width, _height);
			return true;
		}

		public void End()
		{
            _graphics.BindFrameBuffer(0);
            AGSGame.Game.Settings.ResetViewport();
		}

		#region IDisposable implementation

		public void Dispose()
		{
            _graphics.DeleteFrameBuffer(_fbo);
		}

		#endregion
	}
}

