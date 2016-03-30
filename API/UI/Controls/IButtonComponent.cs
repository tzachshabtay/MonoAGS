using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IUIEvents))]
	[RequiredComponent(typeof(IAnimationContainer))]
	public interface IButtonComponent : IComponent
	{
		IAnimation IdleAnimation { get; set; }
		IAnimation HoverAnimation { get; set; }
		IAnimation PushedAnimation { get; set; }
	}
}

