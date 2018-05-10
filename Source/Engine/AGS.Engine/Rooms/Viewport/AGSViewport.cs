using System;
using System.Collections.Generic;
using System.ComponentModel;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSViewport : IViewport
	{
        private readonly Resolver _resolver;
        private readonly Func<int, IGLViewportMatrix> _createMatrixFunc;
        private readonly Dictionary<int, IGLViewportMatrix> _viewports;
        private readonly static IRenderLayer _dummyLayer = new AGSRenderLayer(0);

        public AGSViewport(IDisplayListSettings displayListSettings, ICamera camera, Resolver resolver = null)
		{
            _resolver = resolver ?? AGSGame.Resolver;
            _createMatrixFunc = _ => createMatrix(); //Creating a delegate in advance to avoid memory allocation in critical path
            _viewports = new Dictionary<int, IGLViewportMatrix>(10);
			ScaleX = 1f;
			ScaleY = 1f;
            Camera = camera;
            ProjectionBox = new RectangleF(0f, 0f, 1f, 1f);
            DisplayListSettings = displayListSettings;
            Interactive = true;
		}

		#region IViewport implementation

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float ScaleX { get; set; }

        public float ScaleY { get; set; }

        public float Angle { get; set; }

        public PointF Pivot { get; set; }

        public bool Interactive { get; set; }

        public RectangleF ProjectionBox { get; set; }

		public ICamera Camera { get; set; }

        public IObject Parent { get; set; }

        public IRoomProvider RoomProvider { get; set; }
        public IDisplayListSettings DisplayListSettings { get; set; }

        public Rectangle ScreenArea { get; set; }

        public bool IsObjectVisible(IObject obj)
        {
            return obj.Visible && !DisplayListSettings.RestrictionList.IsRestricted(obj.ID)
                      && !DisplayListSettings.DepthClipping.IsObjectClipped(obj);
        }

        public Matrix4 GetMatrix(IRenderLayer renderLayer)
        {
            renderLayer = renderLayer ?? _dummyLayer;
            var matrixProvider = _viewports.GetOrAdd(renderLayer.Z, _createMatrixFunc);
            return matrixProvider.GetMatrix(this, renderLayer.ParallaxSpeed);
        }

        public Rectangle GetScreenArea(IGameSettings settings, IWindowInfo window, bool updateScreenArea)
        {
            float viewX = 0;
            float viewY = 0;
            float width = window.GameSubWindow.Width;
            float height = window.GameSubWindow.Height;
            if (settings.PreserveAspectRatio) //http://www.david-amador.com/2013/04/opengl-2d-independent-resolution-rendering/
            {
                float targetAspectRatio = (float)settings.VirtualResolution.Width / settings.VirtualResolution.Height;
                Size screen = new Size(window.GameSubWindow.Width, window.GameSubWindow.Height);
                width = screen.Width;
                height = width / targetAspectRatio + 0.5f;
                if (height > screen.Height)
                {
                    //It doesn't fit our height, we must switch to pillarbox then
                    height = screen.Height;
                    width = height * targetAspectRatio + 0.5f;
                }

                // set up the new viewport centered in the backbuffer
                viewX = (screen.Width / 2) - (width / 2);
                viewY = (screen.Height / 2) - (height / 2);
            }

            var projectionBox = ProjectionBox;
            viewX = viewX + (viewX + width) * projectionBox.X;
            viewY = viewY + (viewY + height) * projectionBox.Y;
            var parent = Parent;
            if (parent != null)
            {
                var box = parent.WorldBoundingBox;

                if (box.IsValid)
                {
                    float parentX = MathUtils.Lerp(0f, 0f, settings.VirtualResolution.Width, width, box.MinX);
                    float parentY = MathUtils.Lerp(0f, 0f, settings.VirtualResolution.Height, height, box.MinY);
                    viewX += parentX;
                    viewY += parentY;
                }
            }
            int widthInt = (int)Math.Round((float)width * projectionBox.Width);
            int heightInt = (int)Math.Round((float)height * projectionBox.Height);
            var viewXInt = (int)Math.Round(viewX + window.GameSubWindow.X);
            var viewYInt = (int)Math.Round(viewY + window.GameSubWindow.Y);
            var screenArea = new Rectangle(viewXInt, viewYInt, widthInt, heightInt);
            if (updateScreenArea) ScreenArea = screenArea;
            return screenArea;
        }

        #endregion

        private IGLViewportMatrix createMatrix()
        {
            return _resolver.Container.Resolve<IGLViewportMatrix>();
        }
    }
}