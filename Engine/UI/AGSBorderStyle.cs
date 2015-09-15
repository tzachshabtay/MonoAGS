using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSBorderStyle : IBorderStyle
	{
		public AGSBorderStyle()
		{
			LineWidth = 10f;
			Color = Color.Black;
		}

		#region IBorderStyle implementation

		public float LineWidth { get; set; }

		public Color Color { get; set; }

		#endregion
	}
}

