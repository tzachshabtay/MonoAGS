using System;
using AGS.API;

namespace AGS.Engine
{
	public class GLLineRenderer : IImageRenderer
	{
        private readonly IGLUtils _glUtils;

		public GLLineRenderer (IGLUtils glUtils, float x1, float y1, float x2, float y2)
		{
            _glUtils = glUtils;
			X1 = x1;
			X2 = x2;
			Y1 = y1;
			Y2 = y2;
		}

		#region ICustomRenderer implementation

		public void Prepare(IObject obj, IDrawableInfo drawable, IViewport viewport)
		{
		}

		public void Render (IObject obj, IViewport viewport)
		{
			float x1 = obj.IgnoreViewport ? X1 : X1 - viewport.X;
			float x2 = obj.IgnoreViewport ? X2 : X2 - viewport.X;
			_glUtils.DrawLine (x1, Y1, x2, Y2, 1f, 1f, 0f, 0f, 1f);
		}

		public float X1 { get; private set; }
		public float Y1 { get; private set; }
		public float X2 { get; private set; }
		public float Y2 { get; private set; }

        public SizeF? CustomImageSize => null;

        public PointF? CustomImageResolutionFactor => null;

        #endregion
    }
}

