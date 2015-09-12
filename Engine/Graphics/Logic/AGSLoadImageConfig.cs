using System;
using API;
using System.Drawing;

namespace Engine
{
	public class AGSLoadImageConfig : ILoadImageConfig
	{
		public AGSLoadImageConfig()
		{
		}

		#region ILoadImageConfig implementation

		public Point? TransparentColorSamplePoint { get; set; }

		#endregion
	}
}

