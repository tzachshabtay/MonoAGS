using System;
using System.Drawing;

namespace AGS.API
{
	public interface ITextConfig
	{
		Color Color { get; }
		Font Font { get; }

		Color OutlineColor { get; }
		float OutlineWidth { get; }
	}
}

