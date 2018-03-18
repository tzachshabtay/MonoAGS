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

        #endregion

        private IGLViewportMatrix createMatrix()
        {
            return _resolver.Container.Resolve<IGLViewportMatrix>();
        }
    }
}

