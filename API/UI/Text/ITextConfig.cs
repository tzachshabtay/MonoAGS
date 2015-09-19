using System;
using System.Drawing;

namespace AGS.API
{
	public interface ITextConfig
	{
		Brush Brush { get; }
		Font Font { get; }

		Brush OutlineBrush { get; }
		float OutlineWidth { get; }

		Brush ShadowBrush  { get; }
		float ShadowOffsetX { get; }
		float ShadowOffsetY { get; }
	}
}

