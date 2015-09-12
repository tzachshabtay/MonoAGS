using System;
using API;

namespace Engine
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

