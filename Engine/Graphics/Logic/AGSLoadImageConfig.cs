using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
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

