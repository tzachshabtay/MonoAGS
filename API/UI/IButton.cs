using System;

namespace AGS.API
{
	public interface IButton : ILabel<IButton>
	{
		IAnimation IdleAnimation { get; set; }
		IAnimation HoverAnimation { get; set; }
		IAnimation PressedAnimation { get; set; }
	}
}

