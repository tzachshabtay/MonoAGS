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
			Color = (AGSColor)System.Drawing.Color.Black;
		}

		#region IBorderStyle implementation

		public float LineWidth { get; set; }

		public IColor Color { get; set; }

		#endregion
	}
}

