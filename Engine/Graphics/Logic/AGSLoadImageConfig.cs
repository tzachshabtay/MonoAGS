using System;
using AGS.API;


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

