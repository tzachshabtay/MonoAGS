using System;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	public class GLViewportMatrixFactory : IGLViewportMatrixFactory
	{
		private readonly Dictionary<int, IGLViewportMatrix> _viewports;
		private readonly Resolver _resolver;
        private readonly Func<int, IGLViewportMatrix> _createMatrixFunc;

		public GLViewportMatrixFactory(Resolver resolver)
		{
			_resolver = resolver;
			_viewports = new Dictionary<int, IGLViewportMatrix> (10);
            _createMatrixFunc = _ => createMatrix(); //Creating a delegate in advance to avoid memory allocation in critical path
		}

		#region IGLViewportMatrixFactory implementation

		public IGLViewportMatrix GetViewport(int layer)
		{
            return _viewports.GetOrAdd(layer, _createMatrixFunc);
		}

        #endregion

        private IGLViewportMatrix createMatrix()
        {
            return _resolver.Container.Resolve<IGLViewportMatrix>();
        }
	}
}

