using System;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	public class GLViewportMatrixFactory : IGLViewportMatrixFactory
	{
		private Dictionary<int, IGLViewportMatrix> _viewports;
		private Resolver _resolver;

		public GLViewportMatrixFactory(Resolver resolver)
		{
			_resolver = resolver;
			_viewports = new Dictionary<int, IGLViewportMatrix> (10);
		}

		#region IGLViewportMatrixFactory implementation

		public IGLViewportMatrix GetViewport(int layer)
		{
			return _viewports.GetOrAdd(layer, () => _resolver.Container.Resolve<IGLViewportMatrix>());
		}

		#endregion
	}
}

