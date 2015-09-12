using System;
using System.Drawing;

namespace API
{
	public interface IBorderStyle
	{
		float LineWidth { get; set; }
		Color Color { get; set; }
	}
}

