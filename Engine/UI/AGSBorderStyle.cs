using System;
using API;
using System.Drawing;

namespace Engine
{
	public class AGSBorderStyle : IBorderStyle
	{
		public AGSBorderStyle()
		{
		}

		#region IBorderStyle implementation

		public float LineWidth { get; set; }

		public Color Color { get; set; }

		#endregion
	}
}

