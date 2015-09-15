using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRenderLayer : IRenderLayer
	{
		public AGSRenderLayer(int z)
		{
			Z = z;
		}

		#region IRenderLayer implementation

		public int Z { get; private set; }

		#endregion
	}
}

