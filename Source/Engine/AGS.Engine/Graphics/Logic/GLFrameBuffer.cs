using System;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
	public class GLFrameBuffer : IFrameBuffer
	{
		private readonly int _fbo, _width, _height;
        private readonly IGraphicsBackend _graphics;

        public GLFrameBuffer(Size size, IGraphicsBackend graphics, IRenderMessagePump messagePump)
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
            if (!_graphics.DrawFrameBuffer())
            {
                _graphics.BindFrameBuffer(0);
                return false;
            }

            _graphics.Viewport(0, 0, _width, _height);
            _graphics.ClearColor(0f,0f,0f,0f);
            _graphics.ClearScreen();
			return true;
		}

		public void End()
		{
            _graphics.BindFrameBuffer(0);
            _graphics.UndoLastViewport();
		}

		#region IDisposable implementation

		public void Dispose()
		{
            _graphics.DeleteFrameBuffer(_fbo);
		}

		#endregion
	}
}

