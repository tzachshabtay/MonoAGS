using System;
using AGS.API;


namespace AGS.Engine
{
	public class AGSBorderStyle : IBorderStyle
	{
		public AGSBorderStyle()
		{
			LineWidth = 10f;
			Color = Colors.Black;
		}

		#region IBorderStyle implementation

		public float LineWidth { get; set; }

		public Color Color { get; set; }

		#endregion
	}
}

